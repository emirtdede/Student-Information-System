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
    [Authorize(Roles = "Advisor")]
    public class AdvisorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdvisorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int advisorId)) return RedirectToAction("Logout", "Auth");

            var students = await _context.Students
                .Where(s => s.AdvisorId == advisorId)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .ToListAsync();

            ViewBag.TotalStudents = students.Count;
            ViewBag.PendingApprovals = students.SelectMany(s => s.Enrollments).Count(e => e.Status == "Pending");

            // Calculate statistics
            double averageGpa = students.Any() ? students.Average(s => s.Gpa) : 0.0;
            ViewBag.AverageGpa = Math.Round(averageGpa, 2);

            int highHonorCount = students.Count(s => s.Gpa >= 3.50);
            int honorCount = students.Count(s => s.Gpa >= 3.00 && s.Gpa < 3.50);
            int warningCount = students.Count(s => s.Gpa < 2.00);

            ViewBag.HighHonorCount = highHonorCount;
            ViewBag.HonorCount = honorCount;
            ViewBag.WarningCount = warningCount;

            var topStudent = students.OrderByDescending(s => s.Gpa).FirstOrDefault();
            ViewBag.TopStudentName = topStudent != null ? $"{topStudent.FirstName} {topStudent.LastName} ({topStudent.Gpa:0.00})" : "-";

            // Grade distributions
            var allGrades = students.SelectMany(s => s.Enrollments).Where(e => !string.IsNullOrEmpty(e.LetterGrade)).ToList();
            int totalGradesCount = allGrades.Count;
            int failingGradesCount = allGrades.Count(e => e.LetterGrade == "FF" || e.LetterGrade == "FD" || e.LetterGrade == "FZ");
            int passingGradesCount = totalGradesCount - failingGradesCount;

            ViewBag.TotalGradesCount = totalGradesCount;
            ViewBag.FailingGradesCount = failingGradesCount;
            ViewBag.PassingGradesCount = passingGradesCount;
            
            double passingRate = totalGradesCount > 0 ? (double)passingGradesCount / totalGradesCount * 100 : 0.0;
            ViewBag.PassingRate = Math.Round(passingRate, 1);

            return View();
        }

        public async Task<IActionResult> CourseApproval()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int advisorId)) return RedirectToAction("Logout", "Auth");

            var students = await _context.Students
                .Where(s => s.AdvisorId == advisorId)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .Where(s => s.Enrollments.Any(e => e.Status == "Pending"))
                .ToListAsync();

            return View(students);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveCourse(int studentId, int courseId)
        {
            var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
            if (enrollment != null)
            {
                enrollment.Status = "Approved";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("CourseApproval");
        }

        [HttpPost]
        public async Task<IActionResult> RejectCourse(int studentId, int courseId)
        {
            var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
            if (enrollment != null)
            {
                enrollment.Status = "Rejected"; // Instead of Remove, change status to Rejected
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("CourseApproval");
        }

        [HttpPost]
        public async Task<IActionResult> ApproveAll(int studentId)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == "Pending")
                .ToListAsync();

            foreach (var enrollment in enrollments)
            {
                enrollment.Status = "Approved";
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("CourseApproval");
        }

        public async Task<IActionResult> StudentManagement()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int advisorId)) return RedirectToAction("Logout", "Auth");

            var students = await _context.Students
                .Where(s => s.AdvisorId == advisorId)
                .Include(s => s.Enrollments)
                .ToListAsync();

            return View(students);
        }

        public async Task<IActionResult> GradeEntry()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int advisorId)) return RedirectToAction("Logout", "Auth");

            var systemSetting = await _context.SystemSettings.FirstOrDefaultAsync() ?? new SystemSetting();
            ViewBag.IsGradeEntryActive = systemSetting.IsGradeEntryActive;

            // Fetch students assigned to this advisor, with their approved enrollments
            var students = await _context.Students
                .Where(s => s.AdvisorId == advisorId)
                .Include(s => s.Enrollments.Where(e => e.Status == "Approved"))
                    .ThenInclude(e => e.Course)
                .ToListAsync();

            return View(students);
        }

        [HttpPost]
        public async Task<IActionResult> SaveGrades(int studentId, int courseId, int? midtermGrade, int? finalGrade)
        {
            var systemSetting = await _context.SystemSettings.FirstOrDefaultAsync() ?? new SystemSetting();
            if (!systemSetting.IsGradeEntryActive)
            {
                TempData["ErrorMessage"] = "Not giriş dönemi aktif değildir.";
                return RedirectToAction("GradeEntry");
            }

            var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
            if (enrollment != null)
            {
                if (midtermGrade.HasValue) enrollment.MidtermGrade = midtermGrade.Value;
                if (finalGrade.HasValue) enrollment.FinalGrade = finalGrade.Value;
                
                if (enrollment.AbsentCount > 4)
                {
                    enrollment.LetterGrade = "FZ";
                }
                else if (enrollment.MidtermGrade.HasValue && enrollment.FinalGrade.HasValue)
                {
                    double average = (enrollment.MidtermGrade.Value * 0.4) + (enrollment.FinalGrade.Value * 0.6);
                    enrollment.LetterGrade = CalculateLetterGrade(average);
                }

                await _context.SaveChangesAsync();
                await UpdateStudentGpa(studentId);
            }
            return RedirectToAction("GradeEntry");
        }

        private string CalculateLetterGrade(double average)
        {
            if (average >= 90) return "AA";
            if (average >= 85) return "BA";
            if (average >= 80) return "BB";
            if (average >= 75) return "CB";
            if (average >= 65) return "CC";
            if (average >= 60) return "DC";
            if (average >= 50) return "DD";
            if (average >= 40) return "FD";
            return "FF";
        }

        public async Task<IActionResult> AttendanceEntry()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int advisorId)) return RedirectToAction("Logout", "Auth");

            var systemSetting = await _context.SystemSettings.FirstOrDefaultAsync() ?? new SystemSetting();
            ViewBag.IsGradeEntryActive = systemSetting.IsGradeEntryActive;

            var students = await _context.Students
                .Where(s => s.AdvisorId == advisorId)
                .Include(s => s.Enrollments.Where(e => e.Status == "Approved"))
                    .ThenInclude(e => e.Course)
                .ToListAsync();

            return View(students);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAttendance(int studentId, int courseId, int absentCount)
        {
            var systemSetting = await _context.SystemSettings.FirstOrDefaultAsync() ?? new SystemSetting();
            if (!systemSetting.IsGradeEntryActive)
            {
                TempData["ErrorMessage"] = "Yoklama/not giriş dönemi aktif değildir.";
                return RedirectToAction("AttendanceEntry");
            }

            var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
            if (enrollment != null)
            {
                enrollment.AbsentCount = absentCount;
                if (enrollment.AbsentCount > 4)
                {
                    enrollment.LetterGrade = "FZ";
                }
                else if (enrollment.LetterGrade == "FZ" && enrollment.AbsentCount <= 4)
                {
                    // Recalculate if fixed
                    if (enrollment.MidtermGrade.HasValue && enrollment.FinalGrade.HasValue)
                    {
                        double average = (enrollment.MidtermGrade.Value * 0.4) + (enrollment.FinalGrade.Value * 0.6);
                        enrollment.LetterGrade = CalculateLetterGrade(average);
                    }
                    else
                    {
                        enrollment.LetterGrade = string.Empty;
                    }
                }

                await _context.SaveChangesAsync();
                await UpdateStudentGpa(studentId);
            }
            return RedirectToAction("AttendanceEntry");
        }

        public async Task<IActionResult> SystemSettings()
        {
            var systemSetting = await _context.SystemSettings.FirstOrDefaultAsync() ?? new SystemSetting();
            return View(systemSetting);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSystemSettings(SystemSetting setting)
        {
            var existing = await _context.SystemSettings.FirstOrDefaultAsync();
            if (existing != null)
            {
                existing.IsRegistrationActive = setting.IsRegistrationActive;
                existing.IsGradeEntryActive = setting.IsGradeEntryActive;
                existing.ActiveSemester = setting.ActiveSemester ?? "2024-2025 Bahar";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Sistem ayarları başarıyla güncellendi.";
            }
            return RedirectToAction("SystemSettings");
        }

        public async Task<IActionResult> Substitutions()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var requests = await _context.SubstitutionRequests
                .Include(r => r.Student)
                .Include(r => r.OldCourse)
                .Include(r => r.NewCourse)
                .Where(r => r.Student.AdvisorId == userId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(requests);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveSubstitution(int id)
        {
            var request = await _context.SubstitutionRequests.FindAsync(id);
            if (request != null && request.Status == "Pending")
            {
                request.Status = "Approved";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Ders muafiyet/intibak talebi onaylandı.";
            }
            return RedirectToAction("Substitutions");
        }

        [HttpPost]
        public async Task<IActionResult> RejectSubstitution(int id)
        {
            var request = await _context.SubstitutionRequests.FindAsync(id);
            if (request != null && request.Status == "Pending")
            {
                request.Status = "Rejected";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Ders muafiyet/intibak talebi reddedildi.";
            }
            return RedirectToAction("Substitutions");
        }

        public async Task<IActionResult> Objections()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var objections = await _context.GradeObjections
                .Include(o => o.Student)
                .Include(o => o.Course)
                .Where(o => o.Student.AdvisorId == userId)
                .OrderByDescending(o => o.RequestDate)
                .ToListAsync();

            return View(objections);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveObjection(int id, double? newGrade)
        {
            var objection = await _context.GradeObjections
                .Include(o => o.Student)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (objection != null && objection.Status == "Pending")
            {
                objection.Status = "Approved";
                objection.ProposedGrade = newGrade;

                // Update the enrollment grade if a new grade is provided
                if (newGrade.HasValue)
                {
                    var enrollment = await _context.Enrollments
                        .FirstOrDefaultAsync(e => e.StudentId == objection.StudentId && e.CourseId == objection.CourseId);

                    if (enrollment != null)
                    {
                        if (objection.ExamType == "Midterm")
                        {
                            enrollment.MidtermGrade = (int)Math.Round(newGrade.Value);
                        }
                        else
                        {
                            enrollment.FinalGrade = (int)Math.Round(newGrade.Value);
                        }

                        // Recalculate letter grade
                        if (enrollment.MidtermGrade.HasValue && enrollment.FinalGrade.HasValue)
                        {
                            double average = (enrollment.MidtermGrade.Value * 0.4) + (enrollment.FinalGrade.Value * 0.6);
                            enrollment.LetterGrade = CalculateLetterGrade(average);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await UpdateStudentGpa(objection.StudentId);
                TempData["SuccessMessage"] = "Not itirazı onaylandı ve not güncellendi.";
            }
            return RedirectToAction("Objections");
        }

        [HttpPost]
        public async Task<IActionResult> RejectObjection(int id)
        {
            var objection = await _context.GradeObjections.FindAsync(id);
            if (objection != null && objection.Status == "Pending")
            {
                objection.Status = "Rejected";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Not itirazı reddedildi.";
            }
            return RedirectToAction("Objections");
        }

        public async Task<IActionResult> Documents()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var requests = await _context.DocumentRequests
                .Include(d => d.Student)
                .Where(d => d.Student.AdvisorId == userId)
                .OrderByDescending(d => d.RequestDate)
                .ToListAsync();

            return View(requests);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveDocument(int id, string? adminNote)
        {
            var request = await _context.DocumentRequests.FindAsync(id);
            if (request != null && request.Status == "Pending")
            {
                request.Status = "Ready";
                request.CompletedDate = DateTime.Now;
                request.AdminNote = string.IsNullOrWhiteSpace(adminNote) 
                    ? "Belgeniz hazırdır, öğrenci işlerinden teslim alabilirsiniz." 
                    : adminNote;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Belge talebi onaylandı ve hazır olarak işaretlendi.";
            }
            return RedirectToAction("Documents");
        }

        [HttpPost]
        public async Task<IActionResult> RejectDocument(int id, string? adminNote)
        {
            var request = await _context.DocumentRequests.FindAsync(id);
            if (request != null && request.Status == "Pending")
            {
                request.Status = "Rejected";
                request.CompletedDate = DateTime.Now;
                request.AdminNote = string.IsNullOrWhiteSpace(adminNote) 
                    ? "Talebiniz uygun görülmemiştir." 
                    : adminNote;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Belge talebi reddedildi.";
            }
            return RedirectToAction("Documents");
        }

        private async Task UpdateStudentGpa(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student != null)
            {
                var gradedEnrollments = student.Enrollments
                    .Where(e => !string.IsNullOrEmpty(e.LetterGrade))
                    .ToList();

                double totalCredits = gradedEnrollments.Sum(e => e.Course.Credits);
                double totalPoints = gradedEnrollments.Sum(e => e.Course.Credits * GetGradePoint(e.LetterGrade));

                student.Gpa = totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : 0.0;
                await _context.SaveChangesAsync();
            }
        }

        private static double GetGradePoint(string letterGrade)
        {
            return letterGrade switch
            {
                "AA" => 4.0,
                "BA" => 3.5,
                "BB" => 3.0,
                "CB" => 2.5,
                "CC" => 2.0,
                "DC" => 1.5,
                "DD" => 1.0,
                "FD" => 0.5,
                "FF" => 0.0,
                "FZ" => 0.0,
                _ => 0.0
            };
        }
    }
}
