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
                var testRunAnswers = context.TestRuns.Include("Answer")
                    .Where(x => x.TestRunID == id)
                    .SelectMany(x => x.TestRunAnswers);

                var questionCount = testRunAnswers.Count();

                if (ordinal > questionCount)
                {
                    throw new Exception("Not a valid ordinal.");
                }
                else if (ordinal == questionCount)
                {
                    // Loop around again
                    ordinal = 0;
                }

                // TODO: Get the answer user's anaswer

                var testRunAnswer = testRunAnswers.First(x => x.Ordinal == ordinal);

                var selectedAnswers = context.TestRunAnswers.Where(x => x.TestRunAnswerID == testRunAnswer.TestRunAnswerID).SelectMany(x => x.Answer).ToList();

                var question = context.Questions.Include("Answers")
                    .First(x => x.QuestionID == testRunAnswer.QuestionID);

                QuestionViewModel model = new QuestionViewModel
                {
                    QuestionID = question.QuestionID,
                    Description = question.Description,
                    TestRunID = id,
                    Answers = question.Answers.Shuffle().Select(x => new AnswerViewModel { AnswerID = x.AnswerID, Description = x.Description, IsChecked = selectedAnswers.Any(y => y.AnswerID == x.AnswerID) })
                };

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Question(QuestionViewModel model)
        {
            using (CertificationTrainingContext context = new CertificationTrainingContext())
            {
                var testRunAnswers = context.TestRuns.Include("Answer")
                    .Where(x => x.TestRunID == model.TestRunID)
                    .SelectMany(x => x.TestRunAnswers)
                    .First(x => x.Ordinal == model.Ordinal);

                var checkIds = model.Answers.Where(x => x.IsChecked).Select(x => x.AnswerID);

                var answers = context.Answers.Where(x => checkIds.Contains(x.AnswerID));

                testRunAnswers.Answer = answers.ToList();

                context.SaveChanges();

                return RedirectToAction("Question", new { id = model.TestRunID, ordinal = model.Ordinal + 1 });
            }
        }
    }
}
