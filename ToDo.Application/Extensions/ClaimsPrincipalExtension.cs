using System.Security.Claims;

namespace ToDo.Application.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userIdString = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
                throw new UnauthorizedAccessException("No valid user ID was found in this request");

            return int.Parse(userIdString);
        }
    }
}

