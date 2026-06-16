
namespace ToDo.Application.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base(message, 404, "Registration has not found")
        {
        }
    }
}
