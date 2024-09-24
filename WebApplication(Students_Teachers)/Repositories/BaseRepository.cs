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
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private DbSet<T> entities;
        protected DbSet<T> Entities => this.entities ?? _dbContext.Set<T>();

        protected ApplicationDbContext _dbContext;
        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public abstract Task<OperationDetailsResponse> AddAsync(T entity);

        public abstract Task<OperationDetailsResponse> DeleteAsync(int id);

        public IQueryable<T> GetQuery() => Entities.AsQueryable();

        public abstract Task<OperationDetailsResponse> UpdateAsync(T newEntity);
    }
}