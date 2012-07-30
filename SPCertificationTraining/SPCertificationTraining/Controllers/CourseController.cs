using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

            this.Context.Courses.Remove(course);
            this.Context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
