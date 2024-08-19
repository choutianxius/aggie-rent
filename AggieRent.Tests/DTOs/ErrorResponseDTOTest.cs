using AggieRent.DTOs;
using Xunit;

namespace AggieRent.Tests.DTOs
{
    public class ErrorResponseDTO_ConstructorShould
    {
        [Fact]
        public void Constructor_ThenSetMessage()
        {
            var testErrorMessage = "Some error message";
            var errorResponseDto = new ErrorResponseDTO(testErrorMessage);
            Assert.Equal(testErrorMessage, errorResponseDto.Message);
        }
    }
}
