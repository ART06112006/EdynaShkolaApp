using System.Web;
using System.Web.Mvc;
using WebApplication_Students_Teachers_.Filters;

namespace WebApplication_Students_Teachers_
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new GlobalExceptionFilter());
        }
    }
}
