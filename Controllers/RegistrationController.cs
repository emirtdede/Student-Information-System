using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_Information_System.Data;
using Student_Information_System.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace StudentInformationSystem.Controllers
{
    [Authorize]
    public class RegistrationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegistrationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Logout", "Auth");

            var systemSetting = await _context.SystemSettings.FirstOrDefaultAsync() ?? new SystemSetting();
            ViewBag.IsRegistrationActive = systemSetting.IsRegistrationActive;
            ViewBag.ActiveSemester = systemSetting.ActiveSemester;

            var student = await _context.Students.FindAsync(userId);
            if (student != null)
            {
                ViewBag.TuitionDebt = student.TuitionDebt;
                ViewBag.StudentGpa = student.Gpa;
                ViewBag.IsWarning = student.Gpa < 2.00;
                ViewBag.MaxEcts = student.Gpa < 2.00 ? 22 : 30;
                ViewBag.IsDoubleMajorActive = student.IsDoubleMajorActive;
                ViewBag.DoubleMajorDepartment = student.DoubleMajorDepartment;
            }
            else
            {
                ViewBag.StudentGpa = 4.00;
                ViewBag.IsWarning = false;
                ViewBag.MaxEcts = 30;
            }

            // All courses in the DB
            var allCourses = await _context.Courses.Include(c => c.PrerequisiteCourse).ToListAsync();

            // My enrollments (both past and current) to check prerequisites
            var myAllEnrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == userId)
                .ToListAsync();

            var myCurrentEnrollments = myAllEnrollments.Where(e => e.Status == "Pending" || (e.Status == "Approved" && string.IsNullOrEmpty(e.LetterGrade))).ToList();

            var activeEnrollmentIds = myAllEnrollments.Where(e => e.Status == "Pending" || (e.Status == "Approved" && string.IsNullOrEmpty(e.LetterGrade))).Select(e => e.CourseId).ToHashSet();

            var availableCourses = allCourses.Where(c => {
                if (activeEnrollmentIds.Contains(c.Id)) return false;

                var completed = myAllEnrollments.FirstOrDefault(e => e.CourseId == c.Id && e.Status == "Approved" && !string.IsNullOrEmpty(e.LetterGrade));
                if (completed != null)
                {
                    if (completed.LetterGrade != "FF" && completed.LetterGrade != "FD" && completed.LetterGrade != "FZ" && completed.LetterGrade != "DD" && completed.LetterGrade != "DC")
                    {
                        return false;
                    }
                }
                return true;
            }).ToList();

            ViewBag.CompletedEnrollments = myAllEnrollments
                .Where(e => e.Status == "Approved" && !string.IsNullOrEmpty(e.LetterGrade))
                .ToDictionary(e => e.CourseId, e => e.LetterGrade);

            var enrollmentCounts = await _context.Enrollments
                .Where(e => e.Status == "Pending" || e.Status == "Approved")
                .GroupBy(e => e.CourseId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
            ViewBag.EnrollmentCounts = enrollmentCounts;
            
            var lockedCourses = new Dictionary<int, string>();
            foreach(var c in availableCourses)
            {
                if (c.PrerequisiteCourseId.HasValue)
                {
                    // Check if student passed this prerequisite
                    var prereqEnrollment = myAllEnrollments.FirstOrDefault(e => e.CourseId == c.PrerequisiteCourseId.Value && e.Status == "Approved");
                    if (prereqEnrollment == null || prereqEnrollment.LetterGrade == "FF" || prereqEnrollment.LetterGrade == "FD" || prereqEnrollment.LetterGrade == "FZ")
                    {
                        lockedCourses[c.Id] = $"Ön Koşul Sağlanmadı: {c.PrerequisiteCourse?.Code}";
                    }
                }
            }

            var viewModel = new RegistrationViewModel
            {
                AvailableCourses = availableCourses,
                MyEnrollments = myCurrentEnrollments,
                LockedCoursesReason = lockedCourses
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddCourse(int courseId, string enrollmentType = "Major")
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Logout", "Auth");

            var systemSetting = await _context.SystemSettings.FirstOrDefaultAsync() ?? new SystemSetting();
            if (!systemSetting.IsRegistrationActive)
            {
                TempData["ErrorMessage"] = "Ders kayıt dönemi aktif değildir.";
                return RedirectToAction("Index");
            }

            var student = await _context.Students.FindAsync(userId);
            if (student != null && student.TuitionDebt > 0)
            {
                TempData["ErrorMessage"] = "Ödenmemiş harç borcunuz bulunmaktadır. Lütfen önce borcunuzu ödeyin.";
                return RedirectToAction("Index");
            }

            var courseToAdd = await _context.Courses.FindAsync(courseId);
            if (courseToAdd == null) return RedirectToAction("Index");

            // Server-side capacity check
            int currentEnrollmentsCount = await _context.Enrollments
                .CountAsync(e => e.CourseId == courseId && (e.Status == "Pending" || e.Status == "Approved"));
            if (currentEnrollmentsCount >= courseToAdd.Capacity)
            {
                TempData["ErrorMessage"] = $"Kontenjan dolu: '{courseToAdd.Name}' dersinin kontenjanı dolmuştur ({currentEnrollmentsCount}/{courseToAdd.Capacity}).";
                return RedirectToAction("Index");
            }

            // Server-side schedule conflict check
            if (!string.IsNullOrEmpty(courseToAdd.ClassDay) && !string.IsNullOrEmpty(courseToAdd.StartTime) && !string.IsNullOrEmpty(courseToAdd.EndTime))
            {
                var myCurrentEnrollments = await _context.Enrollments
                    .Include(e => e.Course)
                    .Where(e => e.StudentId == userId && (e.Status == "Pending" || e.Status == "Approved"))
                    .ToListAsync();

                var newStart = TimeSpan.Parse(courseToAdd.StartTime);
                var newEnd = TimeSpan.Parse(courseToAdd.EndTime);

                foreach (var enrollment in myCurrentEnrollments)
                {
                    var existingCourse = enrollment.Course;
                    if (existingCourse.ClassDay == courseToAdd.ClassDay && 
                        !string.IsNullOrEmpty(existingCourse.StartTime) && 
                        !string.IsNullOrEmpty(existingCourse.EndTime))
                    {
                        var existStart = TimeSpan.Parse(existingCourse.StartTime);
                        var existEnd = TimeSpan.Parse(existingCourse.EndTime);

                        // Overlap condition: (StartA < EndB) and (EndA > StartB)
                        if (newStart < existEnd && newEnd > existStart)
                        {
                            TempData["ErrorMessage"] = $"Ders programı çakışması: Seçtiğiniz '{courseToAdd.Name}' dersi, sepetinizdeki '{existingCourse.Name}' dersi ile çakışmaktadır ({courseToAdd.ClassDay} {existingCourse.StartTime}-{existingCourse.EndTime} / {courseToAdd.StartTime}-{courseToAdd.EndTime}).";
                            return RedirectToAction("Index");
                        }
                    }
                }
            }

            // Server-side prerequisite check
            if (courseToAdd.PrerequisiteCourseId.HasValue)
            {
                var myAllEnrollments = await _context.Enrollments.Where(e => e.StudentId == userId).ToListAsync();
                var prereqEnrollment = myAllEnrollments.FirstOrDefault(e => e.CourseId == courseToAdd.PrerequisiteCourseId.Value && e.Status == "Approved");
                if (prereqEnrollment == null || prereqEnrollment.LetterGrade == "FF" || prereqEnrollment.LetterGrade == "FD" || prereqEnrollment.LetterGrade == "FZ")
                {
                    TempData["ErrorMessage"] = "Ön koşul sağlanmadığı için bu dersi alamazsınız.";
                    return RedirectToAction("Index");
                }
            }

            var currentEnrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == userId && (e.Status == "Pending" || e.Status == "Approved"))
                .ToListAsync();

            int maxEcts = (student != null && student.Gpa < 2.00) ? 22 : 30;
            int totalEcts = currentEnrollments.Sum(e => e.Course.Ects);
            if (totalEcts + courseToAdd.Ects > maxEcts)
            {
                TempData["ErrorMessage"] = $"Maksimum AKTS limitini ({maxEcts} AKTS) aştınız. Akademik durumunuz sınamalı (GNO < 2.00) olduğu için limitiniz kısıtlanmıştır.";
                return RedirectToAction("Index");
            }

            var existing = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == userId && e.CourseId == courseId);

            if (existing == null)
            {
                var enrollment = new Enrollment
                {
                    StudentId = userId,
                    CourseId = courseId,
                    Status = "Pending", // Awaiting advisor approval
                    LetterGrade = "",
                    EnrollmentType = enrollmentType
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();
            }
            else
            {
                existing.Status = "Pending";
                existing.MidtermGrade = null;
                existing.FinalGrade = null;
                existing.LetterGrade = string.Empty;
                existing.AbsentCount = 0;
                existing.EnrollmentType = enrollmentType;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCourse(int courseId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Logout", "Auth");

            var systemSetting = await _context.SystemSettings.FirstOrDefaultAsync() ?? new SystemSetting();
            if (!systemSetting.IsRegistrationActive)
            {
                TempData["ErrorMessage"] = "Ders kayıt dönemi aktif değildir.";
                return RedirectToAction("Index");
            }

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == userId && e.CourseId == courseId);

            if (enrollment != null && enrollment.Status == "Pending")
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Tuition()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Logout", "Auth");

            var student = await _context.Students.FindAsync(userId);
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> PayTuition()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Logout", "Auth");

            var student = await _context.Students.FindAsync(userId);
            if (student != null)
            {
                student.TuitionDebt = 0; // Simulate payment success
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Harç ödemeniz başarıyla gerçekleştirildi.";
            }

            return RedirectToAction("Tuition");
        }
    }
}
