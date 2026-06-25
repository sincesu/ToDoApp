using System.Security.Claims;
using ToDo.Application.Exceptions;

namespace ToDo.Application.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userIdString = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
                throw new UnAuthorizedAccessException("No valid user ID was found in this request");

            return Guid.Parse(userIdString);
        }
    }
}

