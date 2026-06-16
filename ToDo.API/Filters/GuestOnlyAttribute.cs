using Microsoft.AspNetCore.Mvc.Filters;
using ToDo.Application.Exceptions;

namespace ToDo.API.Filters
{
    public class GuestOnlyAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            bool isAuthenticated = context.HttpContext!.User!.Identity!.IsAuthenticated;

            if (isAuthenticated)
                throw new BadRequestException("You already sign in");

            await next();
        }
    }
}
