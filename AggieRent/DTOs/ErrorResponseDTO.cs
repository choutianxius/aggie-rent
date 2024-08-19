namespace AggieRent.DTOs
{
    public class ErrorResponseDTO
    {
        public string Message { get; set; } = "";

        public ErrorResponseDTO() { }

        public ErrorResponseDTO(string message)
        {
            Message = message;
        }
    }
}
