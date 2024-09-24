using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication_Students_Teachers_.Models
{
    public class TimeTable
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public ICollection<Lesson> Lessons { get; set; }
    }
}