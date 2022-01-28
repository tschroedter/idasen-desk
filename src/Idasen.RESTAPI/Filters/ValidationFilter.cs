using System.Threading.Tasks ;
using Microsoft.AspNetCore.Mvc ;
using Microsoft.AspNetCore.Mvc.Filters ;

namespace Idasen.RESTAPI.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public Task OnActionExecutionAsync ( ActionExecutingContext  context ,
                                             ActionExecutionDelegate next )
        {
            if ( ! context.ModelState.IsValid )
            {
                context.Result = new BadRequestObjectResult ( context.ModelState ) ;

                return Task.CompletedTask;
            }

            return next ( ) ;
        }
    }
}