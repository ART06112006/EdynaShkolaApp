using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Extensions.Configuration;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;
using Unity.Injection;
using Unity.Lifetime;
using WebApplication_Students_Teachers_.Context;
using WebApplication_Students_Teachers_.Infrastructure.Identity;
using WebApplication_Students_Teachers_.Models;
using WebApplication_Students_Teachers_.Repositories;
using WebApplication_Students_Teachers_.Services;

namespace WebApplication_Students_Teachers_.App_Start
{
    public class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            //Context
            container.RegisterType<ApplicationDbContext>();

            //Repositories
            container.RegisterType<TimeTableRepository>();
            container.RegisterType<UserRepository>();

            //Services
            container.RegisterType<IUserStore<User>, UserStore<User>>(new InjectionConstructor(typeof(ApplicationDbContext)));
            container.RegisterType<IAuthenticationManager>(new InjectionFactory(x => HttpContext.Current.GetOwinContext().Authentication));

            container.RegisterType<TimeTableService>();
            container.RegisterType<EmailService>();
            container.RegisterType<ApplicationUserManager>(new InjectionFactory(x => HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()));
            container.RegisterType<ApplicationSignInManager>(new InjectionFactory(x => HttpContext.Current.GetOwinContext().GetUserManager<ApplicationSignInManager>()));
            container.RegisterType<RoleManager<IdentityRole>>(new InjectionFactory(c => new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(c.Resolve<ApplicationDbContext>()))));

            //Create Roles
            CreateRoles(container.Resolve<RoleManager<IdentityRole>>());

            //
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static void CreateRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExists(Role.Student.ToString()))
            {
                roleManager.Create(new IdentityRole(Role.Student.ToString()));
            }

            if (!roleManager.RoleExists(Role.Teacher.ToString()))
            {
                roleManager.Create(new IdentityRole(Role.Teacher.ToString()));
            }
        }
    }
}