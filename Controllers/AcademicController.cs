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
    public class AcademicController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AcademicController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Transkript()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var student = await _context.Students.FindAsync(userId);
            if (student != null)
            {
                ViewBag.StudentGpa = student.Gpa;
                ViewBag.IsDoubleMajorActive = student.IsDoubleMajorActive;
                ViewBag.DoubleMajorDepartment = student.DoubleMajorDepartment;
                ViewBag.DoubleMajorGpa = student.DoubleMajorGpa;
            }
            else
            {
                ViewBag.StudentGpa = 0.0;
                ViewBag.IsDoubleMajorActive = false;
                ViewBag.DoubleMajorDepartment = string.Empty;
                ViewBag.DoubleMajorGpa = 0.0;
            }

            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == userId && e.Status == "Approved")
                .ToListAsync();

            return View(enrollments);
        }
    
        public IActionResult Calendar()
        {
            return View();
        }

        public async Task<IActionResult> Attendance()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == userId && (e.Status == "Approved" || e.Status == "Pending"))
                .ToListAsync();

            return View(enrollments);
        }

        public async Task<IActionResult> Materials()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == userId && (e.Status == "Approved" || e.Status == "Pending"))
                .ToListAsync();

            return View(enrollments);
        }

        public async Task<IActionResult> Grades()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == userId && e.Status == "Approved")
                .ToListAsync();

            return View(enrollments);
        }

        public async Task<IActionResult> Schedule()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == userId && (e.Status == "Approved" || e.Status == "Pending"))
                .ToListAsync();

            return View(enrollments);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitSurvey(int courseId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.StudentId == userId && e.CourseId == courseId);
            if (enrollment != null)
            {
                enrollment.IsSurveyCompleted = true;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Değerlendirme anketi başarıyla kaydedildi. Ders notunuzun kilidi açıldı.";
            }

            return RedirectToAction("Grades");
        }

        public async Task<IActionResult> GraduationStatus()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var student = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == userId);

            if (student == null) return RedirectToAction("Logout", "Auth");

            var allCourses = await _context.Courses.ToListAsync();

            var enrollments = student.Enrollments.ToList();

            var completedEnrollments = enrollments
                .Where(e => e.Status == "Approved" && !string.IsNullOrEmpty(e.LetterGrade) &&
                            e.LetterGrade != "FF" && e.LetterGrade != "FD" && e.LetterGrade != "FZ")
                .ToList();

            int completedEcts = completedEnrollments.Sum(e => e.Course.Ects);

            var courseStatuses = new List<CourseStatusViewModel>();
            foreach (var course in allCourses)
            {
                var enrollment = completedEnrollments.FirstOrDefault(e => e.CourseId == course.Id);
                courseStatuses.Add(new CourseStatusViewModel
                {
                    Course = course,
                    IsCompleted = enrollment != null,
                    Grade = enrollment?.LetterGrade ?? string.Empty
                });
            }

            bool allMandatoryCompleted = courseStatuses.All(c => c.IsCompleted);
            bool isGraduationEligible = completedEcts >= 240 && student.Gpa >= 2.00 && 
                                       student.IsInternship1Completed && student.IsInternship2Completed && 
                                       allMandatoryCompleted;

            var viewModel = new GraduationViewModel
            {
                Student = student,
                CompletedEcts = completedEcts,
                RequiredEcts = 240,
                CurrentGpa = student.Gpa,
                RequiredGpa = 2.00,
                CourseStatuses = courseStatuses,
                IsGraduationEligible = isGraduationEligible
            };

            return View(viewModel);
        }

        public async Task<IActionResult> GradeObjections()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var objections = await _context.GradeObjections
                .Include(o => o.Course)
                .Where(o => o.StudentId == userId)
                .OrderByDescending(o => o.RequestDate)
                .ToListAsync();

            return View(objections);
        }

        [HttpPost]
        public async Task<IActionResult> CreateObjection(int courseId, string examType, string explanation)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            // Check enrollment exists
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == userId && e.CourseId == courseId && e.Status == "Approved");
            
            if (enrollment == null)
            {
                TempData["ErrorMessage"] = "Bu ders kaydınız bulunamadı.";
                return RedirectToAction("Grades");
            }

            // Check if there's already a pending objection for same course+exam
            var existingObjection = await _context.GradeObjections
                .AnyAsync(o => o.StudentId == userId && o.CourseId == courseId && o.ExamType == examType && o.Status == "Pending");
            
            if (existingObjection)
            {
                TempData["ErrorMessage"] = "Bu ders ve sınav türü için zaten bekleyen bir itirazınız var.";
                return RedirectToAction("Grades");
            }

            double currentGrade = examType == "Midterm" 
                ? (enrollment.MidtermGrade ?? 0) 
                : (enrollment.FinalGrade ?? 0);

            var objection = new GradeObjection
            {
                StudentId = userId,
                CourseId = courseId,
                ExamType = examType,
                CurrentGrade = currentGrade,
                Explanation = explanation,
                Status = "Pending",
                RequestDate = DateTime.Now
            };

            _context.GradeObjections.Add(objection);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Not itiraz talebiniz başarıyla oluşturuldu. Danışmanınız inceleyecektir.";
            return RedirectToAction("Grades");
        }
    }
}
