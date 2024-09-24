using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication_Students_Teachers_.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Topic { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? TimeTableId { get; set; }
        public TimeTable TimeTable { get; set; }
    } 
}