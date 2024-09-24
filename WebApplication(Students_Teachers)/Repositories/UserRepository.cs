using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApplication_Students_Teachers_.Context;
using WebApplication_Students_Teachers_.Models;
using WebApplication_Students_Teachers_.Services;

namespace WebApplication_Students_Teachers_.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }

        public override async Task<OperationDetailsResponse> AddAsync(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return new OperationDetailsResponse { IsError = false };
        }

        public override async Task<OperationDetailsResponse> DeleteAsync(int id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == $"{id}");
            if (user == null)
            {
                return new OperationDetailsResponse { IsError = true, Message = "User was not found" };
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return new OperationDetailsResponse { IsError = false };
        }

        public override async Task<OperationDetailsResponse> UpdateAsync(User newUser)
        {
            _dbContext.Entry(newUser).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return new OperationDetailsResponse { IsError = false };
        }
    }
}