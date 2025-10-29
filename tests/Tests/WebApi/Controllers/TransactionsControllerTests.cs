using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace WebApi.Controllers
{
    public class TransactionsControllerTests
    {
        private readonly Mock<ILogger<TransactionsController>> _mockLogger;
        private readonly Mock<ITransactionService> _mockService;
        private readonly TransactionsController _controller;
        private readonly string _testFileName = "test-file-name.txt";
        private readonly string _testUserId = "test-user-id";
        private readonly List<string> _listMock =
        [
            "1201903010000020000556418150631234****3324090002MARIA JOSEFINALOJA DO Ó - MATRIZ",
            "1201903010000015200096206760171234****7890233000JOÃO MACEDO   BAR DO JOÃO       ",
            "8201903010000010203845152540732344****1222123222MARCOS PEREIRAMERCADO DA AVENIDA",
        ];

        public TransactionsControllerTests()
        {
            _mockLogger = new Mock<ILogger<TransactionsController>>();
            _mockService = new Mock<ITransactionService>();
            _controller = new TransactionsController(_mockLogger.Object, _mockService.Object);
        }

        [Fact]
        public async Task ImportLinesAsync_ValidFile_ReturnsOk()
        {
            // Act
            var result = await _controller.ImportLinesAsync(_listMock, _testFileName, _testUserId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ImportLinesAsync_InvalidFile_ReturnsBadRequest_EmptyList()
        {
            // Act
            var result = await _controller.ImportLinesAsync([], _testFileName, _testUserId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task ImportLinesAsync_InvalidFile_ReturnsBadRequest_EmptyFileName()
        {
            // Act
            var result = await _controller.ImportLinesAsync(_listMock, string.Empty, _testUserId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}