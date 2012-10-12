using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPCertificationTraining.DataModels
{
    public class CourseStatistics
    {
        public CourseStatistics()
        {
            UserStatistics = new Dictionary<string, double>();
            ProblemStatistics = new List<ProblemStatistics>();
        }

        public Dictionary<string, double> UserStatistics { get; set; }
        public IEnumerable<ProblemStatistics> ProblemStatistics { get; set; }
    }

    public class ProblemStatistics
    {
        public ProblemStatistics()
        {
            Answers = new List<AnswerStatistics>();
            NumberOfTimeOccurred = 0;
            NumberOfTimeAnswerRight = 0;
        }

        public Guid ID { get; set; }
        public int Identity { get; set; }
        public string Description { get; set; }
        public long NumberOfTimeOccurred { get; set; }
        public long NumberOfTimeAnswerRight { get; set; }

        public double PercentRight 
        {
            get
            {
                if (this.NumberOfTimeOccurred == 0)
                    return 0;

                return Math.Round(this.NumberOfTimeAnswerRight / (this.NumberOfTimeOccurred * 1.0) * 100);
            }
        }

        public IEnumerable<AnswerStatistics> Answers { get; set; }
    }

    public class AnswerStatistics
    {
        public AnswerStatistics()
        {
            NumberOfTimesChoosen = 0;
        }

        public Guid ID { get; set; }
        public string Identity { get; set; }
        public string Description { get; set; }
        public bool IsCorrect { get; set; }
        public long NumberOfTimesChoosen { get; set; }
    }
}