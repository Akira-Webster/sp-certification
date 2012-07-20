using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPCertificationTraining.ViewModels
{
    public class QuestionViewModel
    {
        public Guid QuestionID { get; set; }
        public string Description { get; set; }
        public int Ordinal { get; set; }

        public Guid TestID { get; set; }

        public IEnumerable<AnswerViewModel> Answers { get; set; }
    }

    public class AnswerViewModel
    {
        public Guid AnswerID { get; set; }
        public string Description { get; set; }
        public bool IsChecked { get; set; }
    }
}