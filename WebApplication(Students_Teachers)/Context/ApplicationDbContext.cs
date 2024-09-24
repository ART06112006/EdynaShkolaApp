using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApplication_Students_Teachers_.Models;

namespace WebApplication_Students_Teachers_.Context
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext() : base("StudentsTeacherDbConnectionString", throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //User
            modelBuilder.Entity<User>().HasMany(x => x.TimeTables).WithRequired(x => x.User).WillCascadeOnDelete(true);

            //TimeTable
            modelBuilder.Entity<TimeTable>().HasMany(x => x.Lessons).WithOptional(x => x.TimeTable).WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<TimeTable> TimeTables { get; set; }
        public DbSet<Lesson> Lessons { get; set; }

        public static ApplicationDbContext Create() => new ApplicationDbContext();
    }
}