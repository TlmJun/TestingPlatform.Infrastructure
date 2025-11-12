using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestingPlatform.Domain.Models;

namespace TestingPlatform.Infrastructure.Db;
    public class AppDbContext : DbContext
    {
        public DbSet<Answer> Answer => Set<Answer>();                                                                         // 1

        public DbSet<Attempt> Attempt => Set<Attempt>();                                                                     // 2

        public DbSet<Course> Courses => Set<Course>();                                                                      // 3

        public DbSet<Direction> Directions => Set<Direction>();                                                            // 4

        public DbSet<Group> Groups => Set<Group>();                                                                       // 5

        public DbSet<Project> Projects => Set<Project>();                                                                // 6 

        public DbSet<Question> Question => Set<Question>();                                                             // 7

        public DbSet<Student> Students => Set<Student>();                                                              // 8

        public DbSet<Test> Test => Set<Test>();                                                                       // 9

        public DbSet<TestResult> TestResult => Set<TestResult>();                                                    // 10

        public DbSet<User> User => Set<User>();                                                                     // 11

        public DbSet<UserAttemptAnswer> UserAttemptAnswer => Set<UserAttemptAnswer>();                             // 12

        public DbSet<UserSelectedOption> UserSelectedOption => Set<UserSelectedOption>();                         // 13 

        public DbSet<UserTextAnswer> UserTextAnswer => Set<UserTextAnswer>();                                    // 14


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()                                                                             //string превращает
            .Property(u => u.Role)
            .HasConversion<string>();
            modelBuilder.Entity<Test>()
                .Property(t => t.Type)
                .HasConversion<string>();
            modelBuilder.Entity<Question>()
                .Property(q => q.AnswerType)
                .HasConversion<string>();                                                                           //string превращает

            modelBuilder.Entity<Answer>(e =>                                                                        // Answer 1
            {
                e.HasKey(x => x.Id);
                e.HasOne(a => a.Question)
                 .WithMany(q => q.Answers)
                 .HasForeignKey(a => a.QuestionId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Attempt>(e =>                                                                        // Attempt 2
            {
                e.HasKey(x => x.Id);
                e.HasOne(a => a.Test)
                 .WithMany(t => t.Attempts)
                 .HasForeignKey(a => a.TestId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Student)
                .WithMany(e => e.Attempts)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(x => x.UserAttemptAnswers)
                .WithOne(q => q.Attempts)
                .HasForeignKey(f => f.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(x => x.TestResults)
                .WithOne(q => q.Attempt)
                .HasForeignKey(w => w.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Course>(e =>                                                                        //Course 3
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Name).IsUnique();
                e.Property(x => x.Name).IsRequired();

            });


            modelBuilder.Entity<Direction>(e =>                                                                      //Direction 4
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Name).IsUnique();
                e.Property(x => x.Name).IsRequired();
            });

            modelBuilder.Entity<Group>(e =>                                                                           //Group 5
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired();
                e.HasIndex(x => x.Name).IsUnique();

                e.HasOne(x => x.Direction)
                    .WithMany(d => d.Group)
                    .HasForeignKey(x => x.DirectionId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Course)
                    .WithMany(c => c.Groups)
                    .HasForeignKey(x => x.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Project)
                    .WithMany(p => p.Group)
                    .HasForeignKey(x => x.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Project>(e =>                                                                      //Project 6
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Name).IsUnique();
                e.Property(x => x.Name).IsRequired();

            });

            modelBuilder.Entity<Question>(e =>                                                                      //Question 7
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.TestId).IsUnique();
                e.HasIndex(x => x.Number).IsUnique();

                e.HasMany(x => x.UserAttemptAnswers)
                .WithOne(q => q.Questions)
                .HasForeignKey(f => f.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Student>(e =>                                                                        // Student 8
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.Phone).HasMaxLength(30).IsRequired(); ;
                e.Property(x => x.VkProfileLink).IsRequired();
                e.HasMany(s => s.TestResults)
                .WithOne(w => w.Student)
                .HasForeignKey(f => f.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Test>(e =>                                                                        // Test 9
            {
                e.HasKey(x => x.Id);

                e.HasMany(x => x.Questions)
                .WithOne(q => q.Test)
                .HasForeignKey(q => q.TestId)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(x => x.TestResults)
                .WithOne(q => q.Test)
                .HasForeignKey(w => w.TestId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasMany(x => x.Groups)
                .WithMany(s => s.Tests)
                .UsingEntity(l => l.ToTable("test_groups"));

                e.HasMany(x => x.Students)
                .WithMany(s => s.Tests)
                .UsingEntity(l => l.ToTable("test_students"));

                e.HasMany(x => x.Projects)
                .WithMany(s => s.Tests)
                .UsingEntity(l => l.ToTable("test_projects"));

                e.HasMany(x => x.Courses)
                .WithMany(s => s.Tests)
                .UsingEntity(l => l.ToTable("test_courses"));

                e.HasMany(x => x.Directions)
                .WithMany(s => s.Tests)
                .UsingEntity(l => l.ToTable("test_directions"));
            });

            modelBuilder.Entity<TestResult>(e =>                                                              //TestResult 10
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.TestId).IsUnique();
                e.HasIndex(x => x.StudentId).IsUnique();
                e.HasIndex(x => x.AttemptId).IsUnique();
            });

            modelBuilder.Entity<User>(e =>                                                                      //User 11
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Login).IsUnique();
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.Login).IsRequired();
                e.Property(x => x.Email).IsRequired();
                e.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                e.HasOne(x => x.Student)
                    .WithOne(s => s.User)
                    .HasForeignKey<Student>(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<UserAttemptAnswer>(e =>                                                     //  UserAttemptAnswer 12
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.AttemptId).IsUnique();
                e.HasIndex(x => x.QuestionId).IsUnique();
            });

            modelBuilder.Entity<UserSelectedOption>(e =>                                                     // UserSelectedOption 13
            {
                e.HasKey(x => x.Id);

                e.HasOne(x => x.UserAttemptAnswer)
                .WithMany(e => e.UserSelectedOptions)
                .HasForeignKey(w => w.UserAttemptAnswerId)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Answer)
                .WithMany(e => e.UserSelectedOptions)
                .HasForeignKey(w => w.AnswerId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserTextAnswer>(e =>                                                        // UserTextAnswer 14
            {
                e.HasKey(x => x.Id);

                e.HasOne(u => u.UserAttemptAnswer)
                .WithOne(a => a.UserTextAnswers)
                .HasForeignKey<UserTextAnswer>(u => u.UserAttemptAnswerId)
                .OnDelete(DeleteBehavior.Cascade);

            });

        }
    }






