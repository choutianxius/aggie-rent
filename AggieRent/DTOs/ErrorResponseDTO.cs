namespace AggieRent.DTOs
{
    public record class ErrorResponseDTO
    {
        public string Message { get; init; } = "";

        public ErrorResponseDTO() { }

        public ErrorResponseDTO(string message)
        {
            Message = message;
        }

        public ErrorResponseDTO(Exception exception)
        {
            Message = exception.Message;
        }
    }
}
