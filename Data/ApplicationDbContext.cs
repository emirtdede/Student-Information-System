using Microsoft.EntityFrameworkCore;
using Student_Information_System.Models;

namespace Student_Information_System.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Advisor> Advisors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<SubstitutionRequest> SubstitutionRequests { get; set; }
        public DbSet<GradeObjection> GradeObjections { get; set; }
        public DbSet<DocumentRequest> DocumentRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Student>("Student")
                .HasValue<Advisor>("Advisor")
                .HasValue<ApplicationUser>("Admin");

            modelBuilder.Entity<Enrollment>()
                .HasKey(e => new { e.StudentId, e.CourseId });

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Advisor)
                .WithMany(a => a.Students)
                .HasForeignKey(s => s.AdvisorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubstitutionRequest>()
                .HasOne(sr => sr.Student)
                .WithMany()
                .HasForeignKey(sr => sr.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubstitutionRequest>()
                .HasOne(sr => sr.OldCourse)
                .WithMany()
                .HasForeignKey(sr => sr.OldCourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubstitutionRequest>()
                .HasOne(sr => sr.NewCourse)
                .WithMany()
                .HasForeignKey(sr => sr.NewCourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GradeObjection>()
                .HasOne(go => go.Student)
                .WithMany()
                .HasForeignKey(go => go.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GradeObjection>()
                .HasOne(go => go.Course)
                .WithMany()
                .HasForeignKey(go => go.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DocumentRequest>()
                .HasOne(dr => dr.Student)
                .WithMany()
                .HasForeignKey(dr => dr.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
