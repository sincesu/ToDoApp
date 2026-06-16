
namespace ToDo.Application.Exceptions
{
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message) : base(message, 400, "Incomplete or extra data was entered")
        {
        }
    }
}
