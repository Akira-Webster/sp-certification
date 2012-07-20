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
    }

    public class Test
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TestID { get; set; }

        [Required]
        public string Title { get; set; }

        public List<Question> Questions { get; set; }
    }

    public class Question
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid QuestionID { get; set; }

        [Required]
        public string Question { get; set; }

        public List<Answer> Answers { get; set; }

        public Guid TestID { get; set; }
        public Test Test { get; set; }
    }

    public class Answer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AnswerID { get; set; }

        [Required]
        public string Answer { get; set; }

        [Required]
        public bool IsAnswer { get; set; }

        public Guid QuestionID { get; set; }
        public Question Question { get; set; }
    }
}