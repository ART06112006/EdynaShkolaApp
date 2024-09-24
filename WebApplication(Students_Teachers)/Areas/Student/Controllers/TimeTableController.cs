using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using WebApplication_Students_Teachers_.Filters;
using WebApplication_Students_Teachers_.Infrastructure.Identity;
using WebApplication_Students_Teachers_.Models;
using WebApplication_Students_Teachers_.Services;

namespace WebApplication_Students_Teachers_.Areas.Student.Controllers
{
    [Authorize(Roles = "Student")]
    public class TimeTableController : Controller
    {
        private readonly TimeTableService _timeTableService;
        private readonly ApplicationUserManager _userService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TimeTableController(TimeTableService timeTableService, ApplicationUserManager userService, RoleManager<IdentityRole> roleManager)
        {
            _timeTableService = timeTableService;
            _userService = userService;
            _roleManager = roleManager;
        }

        // GET: Student/TimeTable
        public async Task<ActionResult> Index(string teacherId)
        {
            var timeTables = await _timeTableService.GetQuery().Where(x => x.UserId == teacherId).Include(x => x.Lessons).OrderBy(x => x.Date).ToListAsync();

            foreach (var timeTable in timeTables)
            {
                timeTable.Lessons = timeTable.Lessons.OrderBy(x => x.StartTime).ToList();
            }

            var allUsers = await _userService.Users.ToListAsync();

            var teachers = new List<User>();
            foreach (var user in allUsers)
            {
                var roles = await _userService.GetRolesAsync(user.Id);
                if (roles.Contains(Role.Teacher.ToString()))
                {
                    teachers.Add(user);
                }
            }

            ViewBag.Teachers = teachers;

            return View(timeTables);
        }
    }
}