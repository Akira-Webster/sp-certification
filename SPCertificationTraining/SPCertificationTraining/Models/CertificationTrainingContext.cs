using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace SPCertificationTraining.Models
{
    public class CertificationTrainingContext : DbContext
    {
        public CertificationTrainingContext()
            : base("CertificationTraining")
        {

        }

        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<TestRun> TestRuns { get; set; }
        public DbSet<TestRunAnswer> TestRunAnswers { get; set; }
    }

    public class Test
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TestID { get; set; }

        [Required]
        public string Title { get; set; }

        public List<Question> Questions { get; set; }
        public List<TestRun> TestRuns { get; set; }
    }

    public class Question
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid QuestionID { get; set; }

        [Required]
        public string Description { get; set; }

        public List<Answer> Answers { get; set; }

        public Guid TestID { get; set; }
        public Test Test { get; set; }
    }

    public class Answer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AnswerID { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsCorrectAnswer { get; set; }

        public Guid QuestionID { get; set; }
        public Question Question { get; set; }
    }

    public class TestRun
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TestRunID { get; set; }

        [Required]
        public Guid AccountID { get; set; }

        public DateTime DateTaken { get; set; }

        public DateTime DateComplete { get; set; }

        public List<TestRunAnswer> TestRunAnswers { get; set; }

        public Guid TestID { get; set; }
        public Test Test { get; set; }
    }

    public class TestRunAnswer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TestRunAnaswerID { get; set; }

        public Guid QuestionID { get; set; }
        public Question Question { get; set; }

        public List<Answer> Answer { get; set; }
    }
}