using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApplication_Students_Teachers_.Models;
using WebApplication_Students_Teachers_.Repositories;

namespace WebApplication_Students_Teachers_.Services
{
    public class TimeTableService
    {
        private readonly TimeTableRepository _timeTableRepository;
        private readonly LessonRepository _lessonRepository;
        public TimeTableService(TimeTableRepository timeTableRepository, LessonRepository lessonRepository)
        {
            _timeTableRepository = timeTableRepository;
            _lessonRepository = lessonRepository;
        }

        public async Task<OperationDetailsResponse> AddAsync(TimeTable timeTable)
        {
            if (GetQuery().Any(x => x.Date == timeTable.Date && x.UserId == timeTable.UserId))
            {
                return new OperationDetailsResponse { IsError = true, Message = "A time table for this date already exists" };
            }
            
            return await _timeTableRepository.AddAsync(timeTable);
        }
       
        public async Task<OperationDetailsResponse> DeleteAsync(int id) => await _timeTableRepository.DeleteAsync(id);

        public IQueryable<TimeTable> GetQuery() => _timeTableRepository.GetQuery();

        public async Task<OperationDetailsResponse> AddLessonAsync(Lesson lesson)
        {
            var timeTable = await GetQuery().Where(x => x.Id == lesson.TimeTableId).Include(x => x.Lessons).FirstOrDefaultAsync();

            if (timeTable == null)
            {
                return new OperationDetailsResponse { IsError = true, Message = "Time Table was not found" };
            }

            if (lesson.StartTime >= lesson.EndTime ||
                timeTable.Lessons.Any(x => lesson.StartTime >= x.StartTime && lesson.StartTime <= x.EndTime) ||
                timeTable.Lessons.Any(x => lesson.EndTime >= x.StartTime && lesson.EndTime <= x.EndTime) ||
                timeTable.Lessons.Any(x => x.StartTime >= lesson.StartTime && x.StartTime <= lesson.EndTime) ||
                timeTable.Lessons.Any(x => x.EndTime >= lesson.StartTime && x.EndTime <= lesson.EndTime))
            {
                return new OperationDetailsResponse { IsError = true, Message = "Invalid time period" };
            }

            timeTable.Lessons.Add(lesson);

            return await _timeTableRepository.UpdateAsync(timeTable);
        }

        public async Task<OperationDetailsResponse> RemoveLessonFromTimeTableByLessonIdAsync(int timeTableId, int lessonId)
        {
            var timeTable = await GetQuery().Where(x => x.Id == timeTableId).Include(x => x.Lessons).FirstOrDefaultAsync();

            if (timeTable == null)
            {
                return new OperationDetailsResponse { IsError = true, Message = "Time Table was not found" };
            }

            var lesson = timeTable.Lessons.FirstOrDefault(x => x.Id == lessonId);

            if (lesson == null)
            {
                return new OperationDetailsResponse { IsError = true, Message = "Lesson was not found" };
            }

            timeTable.Lessons.Remove(lesson);

            var timeTableUpdateResult = await _timeTableRepository.UpdateAsync(timeTable);
            if (timeTableUpdateResult.IsError)
            {
                return timeTableUpdateResult;
            }

            return await _lessonRepository.RemoveRemainingLessonsAsync();  
        }
    }
}