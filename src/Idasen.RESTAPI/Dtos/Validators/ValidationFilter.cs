using Microsoft.AspNetCore.Mvc ;
using Microsoft.AspNetCore.Mvc.Filters ;

namespace Idasen.RESTAPI.Dtos.Validators
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuted ( ActionExecutedContext context )
        {
        }

        public void OnActionExecuting ( ActionExecutingContext context )
        {
            if ( ! context.ModelState.IsValid )
                context.Result = new BadRequestObjectResult ( context.ModelState ) ;
        }
    }
}