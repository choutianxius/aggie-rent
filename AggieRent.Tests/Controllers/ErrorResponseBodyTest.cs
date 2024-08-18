using AggieRent.Controllers;
using Xunit;

namespace AggieRent.Tests.Controllers
{
    public class ErrorResponseBody_ConstructorShould
    {
        [Fact]
        public void Constructor_ThenSetMessage()
        {
            var testErrorMessage = "Some error message";
            var errorResponseBody = new ErrorResponseBody(testErrorMessage);
            Assert.Equal(testErrorMessage, errorResponseBody.Message);
        }
    }
}
