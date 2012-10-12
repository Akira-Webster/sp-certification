using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPCertificationTraining.DataModels;
using System.Web.Security;

namespace SPCertificationTraining.Controllers
{
    [Authorize(Roles="Admin")]
    public class CourseController : Controller
    {
        private Models.TrainingContext Context { get; set; }

        public CourseController()
        {
            this.Context = new Models.TrainingContext();
        }

        public ActionResult Index()
        {
            var course = this.Context.Courses.ToList();
            return View(course);
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                // Verify that the user selected a file
                if (file != null && file.ContentLength > 0)
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(DataModels.Course));
                    var data = serializer.Deserialize(file.InputStream) as DataModels.Course;

                    var course = new Models.Course();
                    course.ID = Guid.NewGuid();
                    course.Title = data.Title;
                    course.Description = data.Description;
                    course.Problems = new List<Models.Problem>();

                    foreach (var p in data.Problems)
                    {
                        var problem = new Models.Problem();
                        problem.ID = Guid.NewGuid();
                        problem.Identity = p.Identity;
                        problem.Description = p.Description;
                        problem.Choices = new List<Models.Choice>();

                        foreach (var c in p.Choices)
                        {
                            problem.Choices.Add(new Models.Choice()
                            {
                                ID = Guid.NewGuid(),
                                Identity = c.Identity,
                                Description = c.Description,
                                IsAnswer = c.IsAnswer
                            });
                        }

                        course.Problems.Add(problem);
                    }

                    this.Context.Courses.Add(course);
                    this.Context.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                return View();
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex);
                return View();
            }
        }

        public ActionResult Delete(Guid id)
        {
            var course = this.Context.Courses.FirstOrDefault(t => t.ID == id);
            if (course == null)
                throw new HttpException(404, "The Course you are looking for has been lost.");

            var tests = this.Context.Tests.Where(t => t.Course.ID == course.ID);
            foreach (var test in tests)
                this.Context.Tests.Remove(test);

            this.Context.Courses.Remove(course);
            this.Context.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Statistics(Guid id)
        {
            var testForCourseQuery = this.Context.Tests.Where(x => x.Course.ID == id);

            var userStatistics = testForCourseQuery
                .GroupBy(x => x.UserKey)
                .ToList()
                .Select(x =>
                {
                    var user = Membership.GetUser(x.Key, false);
                    return new
                    {
                        Name = user.UserName,
                        Average = Math.Round(x.Average(y => y.Avgerage) * 100)
                    };
                })
                .OrderBy(x => x.Name)
                .ToDictionary(k => k.Name, v => v.Average);

            var problemLayout = this.Context.Courses
                .Where(x => x.ID == id)
                .SelectMany(x => x.Problems)
                .Select(x => new ProblemStatistics
                {
                    ID = x.ID,
                    Identity = x.Identity,
                    Description = x.Description,
                    Answers = x.Choices.Select(c => new AnswerStatistics
                    {
                        ID = c.ID,
                        IsCorrect = c.IsAnswer,
                        Identity = c.Identity,
                        Description = c.Description
                    })
                })
                .ToList();

            var questions = testForCourseQuery
                .SelectMany(x => x.Questions);

            foreach (var q in questions)
            {
                var p = problemLayout.Single(x => x.ID == q.Problem.ID);
                p.NumberOfTimeOccurred++;
                if(q.IsCorrect)
                    p.NumberOfTimeAnswerRight++;

                foreach (var a in q.Answers)
                {
                    var pa = p.Answers.Single(x => x.Identity == a.Identity);
                    pa.NumberOfTimesChoosen++;
                }
            }

            var courseStatistics = new SPCertificationTraining.DataModels.CourseStatistics
            {
                UserStatistics = userStatistics,
                ProblemStatistics = problemLayout.OrderBy(x => x.Identity)
            };

            return View(courseStatistics);
        }
    }
}
