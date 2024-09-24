using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApplication_Students_Teachers_.Context;
using WebApplication_Students_Teachers_.Models;

namespace WebApplication_Students_Teachers_.Repositories
{
    public class TimeTableRepository : BaseRepository<TimeTable>
    {
        public TimeTableRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }

        public override async Task<OperationDetailsResponse> AddAsync(TimeTable timeTable)
        {
            _dbContext.TimeTables.Add(timeTable);
            await _dbContext.SaveChangesAsync();

            return new OperationDetailsResponse { IsError = false };
        }

        public override async Task<OperationDetailsResponse> DeleteAsync(int id)
        {
            var timeTable = await _dbContext.TimeTables.FirstOrDefaultAsync(x => x.Id == id);
            if (timeTable == null)
            {
                return new OperationDetailsResponse { IsError = true, Message = "TimeTable was not found" };
            }

            _dbContext.TimeTables.Remove(timeTable);
            await _dbContext.SaveChangesAsync();

            return new OperationDetailsResponse { IsError = false };
        }
        

        public override async Task<OperationDetailsResponse> UpdateAsync(TimeTable newTimeTable)
        {
            _dbContext.Entry(newTimeTable).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            return new OperationDetailsResponse { IsError = false };
        }
    }
}