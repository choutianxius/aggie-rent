using AggieRent.DTOs;
using Xunit;

namespace AggieRent.Tests.DTOs
{
    public class ErrorResponseDTO_ConstructorShould
    {
        [Fact]
        public void ConstructorWithString_ThenSetMessage()
        {
            var testErrorMessage = "Some error message";
            var errorResponseDto = new ErrorResponseDTO(testErrorMessage);
            Assert.Equal(testErrorMessage, errorResponseDto.Message);
        }

        [Fact]
        public void ConstructorWithException_ThenSetMessage()
        {
            var testException = new Exception("Some error message");
            var errorResponseDto = new ErrorResponseDTO(testException);
            Assert.Equal(testException.Message, errorResponseDto.Message);
        }
    }
}
