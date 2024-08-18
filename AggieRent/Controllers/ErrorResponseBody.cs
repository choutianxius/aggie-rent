namespace AggieRent.Controllers
{
    public class ErrorResponseBody
    {
        public string Message { get; set; } = "";

        public ErrorResponseBody() { }

        public ErrorResponseBody(string message)
        {
            Message = message;
        }
    }
}
