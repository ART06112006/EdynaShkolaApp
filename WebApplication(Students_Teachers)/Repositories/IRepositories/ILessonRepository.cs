using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApplication_Students_Teachers_.Models;

namespace WebApplication_Students_Teachers_.Repositories.IRepositories
{
    public interface ILessonRepository
    {
        Task<OperationDetailsResponse> RemoveRemainingLessonsAsync();
    }
}