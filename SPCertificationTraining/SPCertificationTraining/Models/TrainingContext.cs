using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace SPCertificationTraining.Models
{
    public class TrainingContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Course>().HasMany(c => c.Problems).WithRequired(p => p.Course).WillCascadeOnDelete();
            //modelBuilder.Entity<Problem>().HasMany(p => p.Choices).WithRequired(c => c.Problem).WillCascadeOnDelete();

            //modelBuilder.Entity<Test>().HasMany(t => t.Questions).WithRequired(q => q.Test).WillCascadeOnDelete();
            //modelBuilder.Entity<Question>().HasMany(q => q.Answers).WithRequired(a => a.Question).WillCascadeOnDelete();

            //modelBuilder.Entity<Test>().Property(t => t.Date).HasColumnType("datetime2").HasPrecision(0);
        }
    }

    [Serializable]
    public class Course
    {
        // Primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public int TotalProblems { get { return this.Problems.Count(); } }

        // Navigation property
        public virtual ICollection<Problem> Problems { get; set; }
    }

    [Serializable]
    public class Problem
    {
        // Primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        public int Identity { get; set; }

        [Required]
        public string Description { get; set; }

        // Navigation property
        public virtual ICollection<Choice> Choices { get; set; }

        // Navigation properties
        public Guid CourseID { get; set; }

        // Navigation properties
        public virtual Course Course { get; set; }
    }

    public class Choice
    {
        // Primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        [MaxLength(1)]
        public string Identity { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsAnswer { get; set; }

        // Navigation properties
        public Guid ProblemID { get; set; }

        // Navigation properties
        public virtual Problem Problem { get; set; }
    }

    public class Test
    {
        // Primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        public Guid UserKey { get; set; }
        public DateTime? Date { get; set; }

        public int Progress { get; set; }
        public int Total { get { return this.Questions.Count(); } }
        public bool IsFinsihed { get { return this.Progress >= this.Total; } }
        public bool IsStarted { get { return this.Progress != 0; } }

        public double Avgerage { get { return this.Questions.Count(q => q.IsCorrect) / (double)this.Total; } }

        public string Status 
        { 
            get 
            {
                if (this.Progress == 0)
                    return "Not Started";
                else if (this.IsFinsihed)
                    return "Finsihed";
                else if (this.Progress > 0)
                    return "In Progress";
                else
                    return "Unknown";
            } 
        }

        // Navigation properties
        public virtual Course Course { get; set; }

        // Navigation property
        public virtual ICollection<Question> Questions { get; set; }

        public static int QuestionsPerTest { get { return 10; } }
    }

    public class Question
    {
        // Primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        public bool IsCorrect { get { return this.Problem.Choices.Where(c => c.IsAnswer).Select(c => c.Identity).Except(this.Answers.Select(a => a.Identity)).Count() > 0; } }
        //public List<string> AnswerIdentities { get { return this.Answers.Select(a => a.Identity).ToList(); } }

        // Navigation properties
        public virtual Problem Problem { get; set; }

        // Navigation property
        public virtual ICollection<Answer> Answers { get; set; }

        // Navigation properties
        public virtual Test Test { get; set; }

        // Navigation properties
        public Guid TestID { get; set; }
    }

    public class Answer
    {
        // Primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        [Required]
        [MaxLength(1)]
        public string Identity { get; set; }

        // Navigation properties
        public virtual Question Question { get; set; }

        public Guid QuestionID { get; set; }
    }

}