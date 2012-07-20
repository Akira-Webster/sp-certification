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
            using (CertificationTrainingContext context = new CertificationTrainingContext())
            {
                var testRun = new TestRun
                {
                    AccountID = Guid.NewGuid(),
                    DateTaken = DateTime.Now,
                    DateComplete = DateTime.Now,
                    TestID = id
                };

                context.TestRuns.Add(testRun);

                var questions = context.Questions.Include("Answers")
                    .Where(x => x.TestID == id)
                    .ToList().Shuffle();

                List<TestRunAnswer> assignedQuestions = new List<TestRunAnswer>();

                int ordinal = 0;
                foreach (var question in questions)
                {
                    assignedQuestions.Add(new TestRunAnswer
                    {
                        QuestionID = question.QuestionID,
                        Ordinal = ordinal
                    });
                    ordinal++;
                }

                testRun.TestRunAnswers = assignedQuestions;

                context.SaveChanges();

                // Send to first question
                return RedirectToAction("Question", new { id = testRun.TestRunID });
            }
        }

        public ActionResult Question(Guid id, int ordinal)
        {
            using (CertificationTrainingContext context = new CertificationTrainingContext())
            {
                var TestRunAnswers = context.TestRuns
                    .Where(x => x.TestRunID == id)
                    .SelectMany(x => x.TestRunAnswers)
                    .First(x => x.Ordinal == ordinal);

                var question = context.Questions.Include("Answers")
                    .First(x => x.QuestionID == TestRunAnswers.QuestionID);

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
        public ActionResult Question(QuestionViewModel model)
        {
            using (CertificationTrainingContext context = new CertificationTrainingContext())
            {

                return RedirectToAction("Question", model.Ordinal++);
            }
        }
    }
}
