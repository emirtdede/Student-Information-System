using System.Linq;
using Student_Information_System.Models;

namespace Student_Information_System.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Students.Any())
            {
                var testStudent = context.Students.FirstOrDefault();
                if (testStudent != null && (testStudent.FirstName != "Emir Tarık" || testStudent.LastName != "Dede"))
                {
                    testStudent.FirstName = "Emir Tarık";
                    testStudent.LastName = "Dede";
                    context.SaveChanges();
                }
                return;   // DB has been seeded
            }

            var advisor = new Advisor
            {
                FirstName = "Ahmet",
                LastName = "Yılmaz",
                TcKimlikNo = "12345678900",
                Password = "password",
                Email = "ahmet.yilmaz@oku.edu.tr",
                Role = "Advisor",
                Department = "Computer Engineering",
                Title = "Prof. Dr."
            };
            context.Advisors.Add(advisor);
            context.SaveChanges();

            var student = new Student
            {
                FirstName = "Emir Tarık",
                LastName = "Dede",
                TcKimlikNo = "11111111111",
                Password = "password",
                Email = "emir@oku.edu.tr",
                Role = "Student",
                StudentNumber = "20240101001",
                Department = "Computer Engineering",
                EnrollmentYear = 2020,
                Gpa = 3.5,
                AdvisorId = advisor.Id,
                TuitionDebt = 4250.00m,
                IsInternship1Completed = true,
                IsInternship2Completed = false,
                IsDoubleMajorActive = true,
                DoubleMajorDepartment = "Industrial Engineering",
                DoubleMajorGpa = 3.20
            };
            context.Students.Add(student);
            context.SaveChanges();

            var course1 = new Course
            {
                Code = "BİL203",
                Name = "Data Structures",
                Credits = 3,
                Ects = 5,
                Instructor = "Prof. Dr. Ahmet Yılmaz",
                ClassDay = "Salı",
                StartTime = "09:00",
                EndTime = "12:00"
            };
            context.Courses.Add(course1);
            context.SaveChanges();

            var course2 = new Course
            {
                Code = "BİL301",
                Name = "Algorithm Analysis",
                Credits = 3,
                Ects = 6,
                Instructor = "Prof. Dr. Ahmet Yılmaz",
                PrerequisiteCourseId = course1.Id,
                ClassDay = "Pazartesi",
                StartTime = "09:00",
                EndTime = "12:00"
            };
            context.Courses.Add(course2);
            context.SaveChanges();

            var course3 = new Course
            {
                Code = "BİL401",
                Name = "Advanced Algorithms",
                Credits = 3,
                Ects = 6,
                Instructor = "Prof. Dr. Ahmet Yılmaz",
                PrerequisiteCourseId = course2.Id,
                ClassDay = "Pazartesi",
                StartTime = "10:00",
                EndTime = "13:00" // Overlaps with BİL301!
            };
            context.Courses.Add(course3);
            context.SaveChanges();

            var course4 = new Course
            {
                Code = "BİL305",
                Name = "Database Management Systems",
                Credits = 3,
                Ects = 5,
                Instructor = "Prof. Dr. Ahmet Yılmaz",
                ClassDay = "Çarşamba",
                StartTime = "13:00",
                EndTime = "16:00",
                Capacity = 1
            };
            context.Courses.Add(course4);

            var course5 = new Course
            {
                Code = "BİL307",
                Name = "Operating Systems",
                Credits = 3,
                Ects = 6,
                Instructor = "Prof. Dr. Ahmet Yılmaz",
                ClassDay = "Perşembe",
                StartTime = "14:00",
                EndTime = "17:00",
                Capacity = 2
            };
            context.Courses.Add(course5);

            var course6 = new Course
            {
                Code = "BİL309",
                Name = "Web Programming",
                Credits = 3,
                Ects = 5,
                Instructor = "Prof. Dr. Ahmet Yılmaz",
                ClassDay = "Cuma",
                StartTime = "09:00",
                EndTime = "12:00",
                Capacity = 3
            };
            context.Courses.Add(course6);
            context.SaveChanges();

            var enrollment = new Enrollment
            {
                StudentId = student.Id,
                CourseId = course1.Id,
                Status = "Approved",
                MidtermGrade = 85,
                FinalGrade = 90,
                LetterGrade = "AA",
                EnrollmentType = "Major"
            };
            context.Enrollments.Add(enrollment);

            var capEnrollment = new Enrollment
            {
                StudentId = student.Id,
                CourseId = course4.Id,
                Status = "Approved",
                MidtermGrade = 75,
                FinalGrade = 80,
                LetterGrade = "BA",
                EnrollmentType = "DoubleMajor",
                IsSurveyCompleted = true
            };
            context.Enrollments.Add(capEnrollment);
            context.SaveChanges();

            var message1 = new Message
            {
                SenderId = advisor.Id,
                ReceiverId = student.Id,
                Subject = "OBS Sistemine Hoş Geldiniz",
                Content = "Merhaba Emir Tarık,\n\nBilgisayar Mühendisliği akademik danışmanlık sistemine hoş geldin. Ders kayıtların ile ilgili herhangi bir sorun olduğunda bana bu ekran üzerinden yazabilirsin.\n\nBaşarılar dilerim,\nProf. Dr. Ahmet Yılmaz",
                SentDate = System.DateTime.Now.AddDays(-1),
                IsRead = false
            };
            context.Messages.Add(message1);
            context.SaveChanges();

            var subRequest = new SubstitutionRequest
            {
                StudentId = student.Id,
                OldCourseId = course5.Id,
                NewCourseId = course6.Id,
                Reason = "BİL307 İşletim Sistemleri dersi yerine BİL309 Web Programlama dersinin saydırılması talebi.",
                Status = "Pending",
                RequestDate = System.DateTime.Now
            };
            context.SubstitutionRequests.Add(subRequest);
            context.SaveChanges();
            
            var announcement = new Announcement
            {
                Title = "Bahar Dönemi Ders Kayıtları Başladı",
                Content = "2024-2025 Bahar yarıyılı ders kayıtları başlamıştır. Kayıtlarınızı obs üzerinden yapabilirsiniz.",
                Category = "Academic"
            };
            context.Announcements.Add(announcement);
            context.SaveChanges();

            // Seed default system setting if not present
            if (!context.SystemSettings.Any())
            {
                var systemSetting = new SystemSetting
                {
                    IsRegistrationActive = true,
                    IsGradeEntryActive = true,
                    ActiveSemester = "2024-2025 Bahar"
                };
                context.SystemSettings.Add(systemSetting);
                context.SaveChanges();
            }

            // Seed grade objection
            if (!context.GradeObjections.Any())
            {
                var objection = new GradeObjection
                {
                    StudentId = student.Id,
                    CourseId = course1.Id,
                    ExamType = "Final",
                    CurrentGrade = 90,
                    Explanation = "Final sınavımda 3. sorunun değerlendirmesinde bir hata olduğunu düşünüyorum. Cevabımın doğru olduğuna eminim.",
                    Status = "Pending",
                    RequestDate = System.DateTime.Now
                };
                context.GradeObjections.Add(objection);
                context.SaveChanges();
            }

            // Seed document requests
            if (!context.DocumentRequests.Any())
            {
                var docRequest1 = new DocumentRequest
                {
                    StudentId = student.Id,
                    DocumentType = "Transkript",
                    Purpose = "Burs başvurusu için gerekli",
                    CopyCount = 2,
                    Status = "Pending",
                    RequestDate = System.DateTime.Now.AddDays(-1)
                };
                context.DocumentRequests.Add(docRequest1);

                var docRequest2 = new DocumentRequest
                {
                    StudentId = student.Id,
                    DocumentType = "OgrenciBelgesi",
                    Purpose = "İndirimli ulaşım kartı başvurusu",
                    CopyCount = 1,
                    Status = "Ready",
                    RequestDate = System.DateTime.Now.AddDays(-5),
                    CompletedDate = System.DateTime.Now.AddDays(-3),
                    AdminNote = "Belgeniz hazırdır, öğrenci işlerinden teslim alabilirsiniz."
                };
                context.DocumentRequests.Add(docRequest2);

                context.SaveChanges();
            }
        }
    }
}
