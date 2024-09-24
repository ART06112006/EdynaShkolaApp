using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApplication_Students_Teachers_.Context;
using WebApplication_Students_Teachers_.Models;
using WebApplication_Students_Teachers_.Repositories.IRepositories;

namespace WebApplication_Students_Teachers_.Repositories
{
    public class LessonRepository : ILessonRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public LessonRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationDetailsResponse> RemoveRemainingLessonsAsync()
        {
            var lessonsToDelete = await _dbContext.Lessons.Where(x => x.TimeTableId == null).ToListAsync();
            _dbContext.Lessons.RemoveRange(lessonsToDelete);
            await _dbContext.SaveChangesAsync();

            return new OperationDetailsResponse { IsError = false };
        }
    }
}