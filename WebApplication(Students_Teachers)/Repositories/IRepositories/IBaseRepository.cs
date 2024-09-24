using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApplication_Students_Teachers_.Models;

namespace WebApplication_Students_Teachers_.Repositories.IRepositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<OperationDetailsResponse> AddAsync(T entity);
        IQueryable<T> GetQuery();
        Task<OperationDetailsResponse> UpdateAsync(T newEntity);
        Task<OperationDetailsResponse> DeleteAsync(int id);
    }
}