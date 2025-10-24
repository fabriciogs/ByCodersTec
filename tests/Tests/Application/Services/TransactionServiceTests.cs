using Application.Dtos;
using Application.Repositories;
using Domain.Entities;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System.Text;

namespace Application.Services
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _mockRepo;
        private readonly Mock<IValidator<TransactionDto>> _mockValidator;
        private readonly TransactionService _service;

        public TransactionServiceTests()
        {
            _mockRepo = new Mock<ITransactionRepository>();
            _mockValidator = new Mock<IValidator<TransactionDto>>();
            _service = new TransactionService(_mockRepo.Object, _mockMapper.Object, _mockValidator.Object);
        }

        [Fact]
        public async Task ProcessCnabFileAsync_ValidFile_ProcessesTransactions()
        {
            // Arrange
            var cnabContent = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(cnabContent));
            var dto = new TransactionDto
            {
                Type = 3,
                DateTime = new DateTime(2019, 03, 01, 15, 34, 53),
                Amount = 142.00m,
                Cpf = "09620676017",
                Card = "4753****3153",
                StoreOwner = "JOÃO MACEDO",
                StoreName = "BAR DO JOÃO"
            };
            var fileImportId = Guid.NewGuid();
            var transaction = new Transaction(Guid.NewGuid(), 3, dto.DateTime.ToString("yyyyMMdd"), 142.00m, "09620676017", "4753****3153", dto.DateTime.ToString("HHmmss"), "JOÃO MACEDO", "BAR DO JOÃO");
            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<TransactionDto>(), default)).ReturnsAsync(new ValidationResult());
            _mockMapper.Setup(m => m.Map<Transaction>(It.IsAny<TransactionDto>())).Returns(transaction);
            _mockRepo.Setup(r => r.AddRangeAsync(fileImportId, It.IsAny<IEnumerable<Transaction>>())).Returns(Task.CompletedTask);

            // Act
            await _service.ProcessCnabFileAsync(stream);

            // Assert
            _mockRepo.Verify(r => r.AddRangeAsync(fileImportId, It.Is<IEnumerable<Transaction>>(t => t.Count() == 1)), Times.Once());
        }

        [Fact]
        public async Task ProcessCnabFileAsync_InvalidLine_ThrowsValidationException()
        {
            // Arrange
            var cnabContent = "3201903010000014200096206760174753****31531534JOÃO MACEDO   BAR DO JOÃO"; // Linha curta
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(cnabContent));

            // Act & Assert
            var fileImportId = Guid.NewGuid();
            await _service.ProcessCnabFileAsync(stream); // Linha inválida é ignorada
            _mockRepo.Verify(r => r.AddRangeAsync(fileImportId, It.IsAny<IEnumerable<Transaction>>()), Times.Once());
        }

        [Fact]
        public async Task ProcessCnabFileAsync_InvalidDto_ThrowsValidationException()
        {
            // Arrange
            var cnabContent = "3201903010000014200096206760174753****31531534JOÃO MACEDO   BAR DO JOÃO       ";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(cnabContent));
            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<TransactionDto>(), default))
                .ReturnsAsync(new ValidationResult([new ValidationFailure("Cpf", "Invalid")]));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.ProcessCnabFileAsync(stream));
        }

        [Fact]
        public async Task GetTransactionsByStoreAsync_ValidStore_ReturnsTransactions()
        {
            // Arrange
            var storeName = "BAR DO JOÃO";
            var transactions = new List<Transaction>
            {
                new (Guid.NewGuid(), 3, DateTime.Now.ToString("yyyyMMdd"), 142.00m, "09620676017", "4753****3153", DateTime.Now.ToString("HHmmss"),"JOÃO MACEDO", storeName)
            };
            _mockRepo.Setup(r => r.GetByStoreNameAsync(storeName)).ReturnsAsync(transactions);

            // Act
            var result = await _service.GetTransactionsByStoreAsync(storeName);

            // Assert
            result.Should().HaveCount(1);
            result.First().StoreName.Should().Be(storeName);
        }

        [Fact]
        public async Task GetBalanceByStoreAsync_ValidStore_ReturnsBalance()
        {
            // Arrange
            var storeName = "BAR DO JOÃO";
            var transactions = new List<Transaction>
            {
                new (Guid.NewGuid(), 1, DateTime.Now.ToString("yyyyMMdd"), 100m, "09620676017", "4753****3153", DateTime.Now.ToString("HHmmss"), "JOÃO MACEDO", storeName), // +100
                new (Guid.NewGuid(), 2, DateTime.Now.ToString("yyyyMMdd"), 50m, "09620676017", "4753****3153", DateTime.Now.ToString("HHmmss"), "JOÃO MACEDO", storeName)  // -50
            };
            _mockRepo.Setup(r => r.GetByStoreNameAsync(storeName)).ReturnsAsync(transactions);

            // Act
            var balance = await _service.GetBalanceByStoreAsync(storeName);

            // Assert
            balance.Should().Be(50m); // 100 - 50
        }

        [Fact]
        public async Task GetAllStoreNamesAsync_ReturnsDistinctStores()
        {
            // Arrange
            var stores = new List<string> { "BAR DO JOÃO", "MERCADO DA AVENIDA" };
            _mockRepo.Setup(r => r.GetAllStoreNamesAsync()).ReturnsAsync(stores);

            // Act
            var result = await _service.GetAllStoreNamesAsync();

            // Assert
            result.Should().BeEquivalentTo(stores);
        }

        [Fact]
        public async Task GetTransactionsByStoreAsync_EmptyStoreName_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetTransactionsByStoreAsync(""));
        }
    }
}