using AggregatorService.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace AggregatorServiceTests
{
    public class Tests
    {
        private ValidationService _validationService;
        [SetUp]
        public void Setup()
        {
            var mock = new Mock<ILogger<ValidationService>>();
            ILogger<ValidationService> logger = mock.Object;
            _validationService = new ValidationService(mock.Object);
        }

        [TestCase("1")]
        [TestCase("7")]
        public void IsUserIdValid_InputIs1_ReturnsTrue(string input)
        {
            var result = _validationService.IsUserIdValid(input);

            Assert.IsTrue(result);  
        }

        [Test]
        public void IsUserIdValid_InputIsLessThan0_ReturnsFalse()
        {
            var result = _validationService.IsUserIdValid("-1");

            Assert.IsFalse(result);
        }
        [Test]
        public void IsUserIdValid_InputIsEmpty_ReturnsFalse()
        {
            var result = _validationService.IsUserIdValid("");

            Assert.IsFalse(result);
        }
        [Test]
        public void IsUserIdValid_InputIsNonNumeric_ReturnsFalse()
        {
            var result = _validationService.IsUserIdValid("abc");

            Assert.IsFalse(result);
        }
    }
}