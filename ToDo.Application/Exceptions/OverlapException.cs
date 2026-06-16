
namespace ToDo.Application.Exceptions
{
    public class OverlapException : BaseException
    {
        public OverlapException(string message) : base(message, 400, "The same data was found")
        {
        }
    }
}
