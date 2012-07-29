using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPCertificationTraining.DataModels
{
    [Serializable]
    public class Course
    {
        public Course()
        {
            this.Title = string.Empty;
            this.Description = string.Empty;
            this.Problems = new List<Problem>();
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public virtual List<Problem> Problems { get; set; }
    }

    [Serializable]
    public class Problem
    {
        public Problem()
        {
            this.Identity = 0;
            this.Description = string.Empty;
            this.Choices = new List<Choice>();
        }

        public int Identity { get; set; }
        public string Description { get; set; }
        public virtual List<Choice> Choices { get; set; }
    }

    [Serializable]
    public class Choice
    {
        public Choice()
        {
            this.Identity = "A";
            this.Description = string.Empty;
            this.IsAnswer = false;
        }

        public string Identity { get; set; }
        public string Description { get; set; }
        public bool IsAnswer { get; set; }
    }
}