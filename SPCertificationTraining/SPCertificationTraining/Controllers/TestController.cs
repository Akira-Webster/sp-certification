using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPCertificationTraining.Models;
using SPCertificationTraining.ViewModels;

namespace SPCertificationTraining.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {
            using(CertificationTrainingContext context = new CertificationTrainingContext())
            {
                var test = context.Tests.ToList();
                return View(test);
            }
        }

        public ActionResult StartTest(Guid id)
        {
            // Create Test run

            // Send to first question

            return RedirectToAction("Question", new { testID = id });
        }

        public ActionResult Question(Guid testID)
        {
            using (CertificationTrainingContext context = new CertificationTrainingContext())
            {
                var questions = context.Questions.Include("Answers")
                    .Where(x => x.TestID == testID)
                    .ToList();

                Random rnd = new Random();
                var questionIndex = rnd.Next(0, questions.Count);

                Question question = questions[questionIndex];

                QuestionViewModel model = new QuestionViewModel
                {
                    QuestionID = question.QuestionID,
                    Description = question.Description,
                    Answers = question.Answers.Shuffle().Select(x => new AnswerViewModel { AnswerID = x.AnswerID, Description = x.Description, IsChecked = false })
                };

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Question(Guid TestID, Guid questionID, Guid answerID)
        {
            return RedirectToAction("Question");
        }
    }
}
