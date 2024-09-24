using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication_Students_Teachers_.Filters;
using WebApplication_Students_Teachers_.Infrastructure.Identity;
using WebApplication_Students_Teachers_.Models;
using WebApplication_Students_Teachers_.Models.ViewModels;
using WebApplication_Students_Teachers_.Services;

namespace WebApplication_Students_Teachers_.Areas.Teacher.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TimeTableController : Controller
    {
        private readonly TimeTableService _timeTableService;

        public TimeTableController(TimeTableService timeTableService)
        {
            _timeTableService = timeTableService;
        }

        // GET: Teacher/TimeTable
        public async Task<ActionResult> Index(ModalMessageViewModel modalMessage)
        {
            var userId = User.Identity.GetUserId();
            var timeTables = await _timeTableService.GetQuery().Where(x => x.UserId == userId).Include(x => x.Lessons).OrderBy(x => x.Date).ToListAsync();

            foreach (var timeTable in timeTables)
            {
                timeTable.Lessons = timeTable.Lessons.OrderBy(x => x.StartTime).ToList();
            }

            if (modalMessage == null)
            {
                ViewBag.ModalMessage = new ModalMessageViewModel { Show = false };
            }
            else
            {
                ViewBag.ModalMessage = modalMessage;
            }

            return View(timeTables);
        }

        public async Task<ActionResult> AddTimeTable(DateTime date)
        {
            var userId = User.Identity.GetUserId();

            var timeTable = new TimeTable()
            {
                Date = date,
                UserId = userId
            };

            var result = await _timeTableService.AddAsync(timeTable);

            if (result.IsError)
            {
                return RedirectToAction("Index", new { Show = true, IsError = true, Header = "Error", Body = result.Message } );
            }

            return RedirectToRoute("Teacher_default", new { controller = "TimeTable", action = "Index"});
        }

        public async Task<ActionResult> RemoveTimeTable(int timeTableId)
        {
            var result = await _timeTableService.DeleteAsync(timeTableId);

            if (result.IsError)
            {
                return RedirectToAction("Index", new { Show = true, IsError = true, Header = "Error", Body = result.Message });
            }

            return RedirectToRoute("Teacher_default", new { controller = "TimeTable", action = "Index" });
        }

        public async Task<ActionResult> AddLesson(Lesson lesson)
        {
            var result = await _timeTableService.AddLessonAsync(lesson);

            if (result.IsError)
            {
                return RedirectToAction("Index", new { Show = true, IsError = true, Header = "Error", Body = result.Message });
            }

            return RedirectToRoute("Teacher_default", new { controller = "TimeTable", action = "Index" });
        }

        public async Task<ActionResult> RemoveLesson(int lessonId, int timeTableId)
        {
            var result = await _timeTableService.RemoveLessonFromTimeTableByLessonIdAsync(timeTableId, lessonId);

            if (result.IsError)
            {
                return RedirectToAction("Index", new { Show = true, IsError = true, Header = "Error", Body = result.Message });
            }

            return RedirectToRoute("Teacher_default", new { controller = "TimeTable", action = "Index" });
        }
    }
}