using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SPCertificationTraining.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        private Models.TrainingContext Context { get; set; }

        private Guid UserID { get { return (Guid)Membership.GetUser(HttpContext.User.Identity.Name).ProviderUserKey; } }

        public TestController()
        {
            this.Context = new Models.TrainingContext();
        }

        public ActionResult Index()
        {
            this.ViewBag.Courses = this.Context.Courses.ToList().Select(c => new SelectListItem() { Text = c.Title, Value = c.ID.ToString() });
            this.ViewBag.PastTests = this.Context.Tests.Where(t => t.UserKey == this.UserID).ToList();

            return View();
        }

        public ActionResult Problem(Guid id)
        {
            var test = this.Context.Tests.FirstOrDefault(t => t.ID == id && t.UserKey == this.UserID);
            if (test == null)
                throw new HttpException(404, "The Test you are looking for has been lost.");

            if (test.IsFinsihed)
                return RedirectToAction("Index");

            var question = test.Questions.ToList().ElementAt(test.Progress);
            if (question == null)
                throw new HttpException(404, "The Question you are looking for has been lost.");

            return View(question.Problem);
        }

        [HttpPost]
        public ActionResult Problem(Guid id, FormCollection collection)
        {
            var test = this.Context.Tests.FirstOrDefault(t => t.ID == id && t.UserKey == this.UserID);
            if (test == null)
                throw new HttpException(404, "The Test you are looking for has been lost.");

            var currentquestion = test.Questions.ToList().ElementAtOrDefault(test.Progress);
            if (currentquestion == null)
                throw new HttpException(404, "The Question you are looking for has been lost.");

            // Clear all answers
            currentquestion.Answers.Clear();

            // Map submitions
            var submitions = (collection["Identity"] == null) ? new List<string>() : collection["Identity"].Split(',').ToList();
            foreach (var identity in submitions)
                currentquestion.Answers.Add(new Models.Answer() { ID = Guid.NewGuid(), Identity = identity });

            // Update progress
            test.Progress++;

            this.Context.SaveChanges();

            if (test.IsFinsihed)
                return RedirectToAction("Index");

            var nextquestion = test.Questions.ToList().ElementAtOrDefault(test.Progress);
            if (nextquestion == null)
                throw new HttpException(404, "The Question you are looking for has been lost.");

            return View(nextquestion.Problem);
        }

        public ActionResult Details(Guid id)
        {
            var test = this.Context.Tests.FirstOrDefault(t => t.ID == id && t.UserKey == this.UserID);
            if (test == null)
                throw new HttpException(404, "The Test you are looking for has been lost.");

            return View(test);
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                var id = new Guid(collection["Course"]);
                var course = this.Context.Courses.FirstOrDefault(c => c.ID == id);
                var problems = course.Problems.Shuffle().Take(Models.Test.QuestionsPerTest).ToList();

                var test = new Models.Test();
                test.ID = Guid.NewGuid();
                test.Course = course;
                test.Date = DateTime.Now;
                test.Questions = new List<Models.Question>();
                test.UserKey = this.UserID;

                foreach (var problem in problems)
                {
                    var question = new Models.Question();
                    question.ID = Guid.NewGuid();
                    question.Problem = problem;
                    question.Answers = new List<Models.Answer>();

                    test.Questions.Add(question);
                }

                this.Context.Tests.Add(test);
                this.Context.SaveChanges();
            }
            catch
            {
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid id)
        {
            var test = this.Context.Tests.FirstOrDefault(t => t.ID == id && t.UserKey == this.UserID);
            if (test == null)
                throw new HttpException(404, "The Test you are looking for has been lost.");

            this.Context.Tests.Remove(test);
            this.Context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}