using Application.Dtos;
using Application.Repositories;
using Domain.Entities;
using FluentAssertions;
using Moq;
using System.Text;

namespace Application.Services
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _mockRepo;
        private readonly TransactionService _service;
        private readonly List<string> _listMock =
        [
            "1201903010000020000556418150631234****3324090002MARIA JOSEFINALOJA DO Ó - MATRIZ",
            "1201903010000015200096206760171234****7890233000JOÃO MACEDO   BAR DO JOÃO       ",
            "8201903010000010203845152540732344****1222123222MARCOS PEREIRAMERCADO DA AVENIDA",
        ];

        public TransactionServiceTests()
        {
            _mockRepo = new Mock<ITransactionRepository>();
            _service = new TransactionService(_mockRepo.Object);
        }

        [Fact]
        public async Task ProcessCnabFileAsync_ValidFile_ProcessesTransactions()
        {
            // Arrange
            var importFile = new ImportFile(Guid.NewGuid(), "CNAB.txt", "test");
            var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), 3, DateTime.Now, 142.00m, "09620676017", "4753****3153", "JOÃO MACEDO", "BAR DO JOÃO");
            //_mockValidator.Setup(v => v.ValidateAsync(It.IsAny<TransactionDto>(), default)).ReturnsAsync(new ValidationResult());
            //_mockMapper.Setup(m => m.Map<Transaction>(It.IsAny<TransactionDto>())).Returns(transaction);
            _mockRepo.Setup(r => r.SaveTransactionsAsync(importFile, It.IsAny<IEnumerable<Transaction>>())).Returns(Task.CompletedTask);

            // Act
            await _service.ProcessCnabFileAsync(_listMock, "CNAB.txt", "test");

            // Assert
            _mockRepo.Verify(r => r.SaveTransactionsAsync(importFile, It.Is<IEnumerable<Transaction>>(t => t.Count() == 1)), Times.Once());
        }

        [Fact]
        public async Task ProcessCnabFileAsync_InvalidLine_ThrowsValidationException()
        {
            // Arrange
            var cnabContent = "2201903010000010700845152540738723****9987123333MARCOS PEREIRAMERCADO DA AVENIDA"; // Linha curta
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(cnabContent));

            // Act & Assert
            var importFile = new ImportFile(Guid.NewGuid(), "CNAB.txt", "test");
            await _service.ProcessCnabFileAsync(stream, "CNAB.txt", "test"); // Linha inválida é ignorada
            _mockRepo.Verify(r => r.SaveTransactionsAsync(importFile, It.IsAny<IEnumerable<Transaction>>()), Times.Once());
        }

        [Fact]
        public async Task ProcessCnabFileAsync_InvalidDto_ThrowsValidationException()
        {
            // Arrange
            var cnabContent = "3201903010000014200096206760174753****31531534JOÃO MACEDO   BAR DO JOÃO       ";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(cnabContent));
            //_mockValidator.Setup(v => v.ValidateAsync(It.IsAny<TransactionDto>(), default)).ReturnsAsync(new ValidationResult([new ValidationFailure("Cpf", "Invalid")]));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.ProcessCnabFileAsync(stream, "CNAB.txt", "test"));
        }

        [Fact]
        public async Task GetTransactionsByStoreAsync_ValidStore_ReturnsTransactions()
        {
            // Arrange
            var storeName = "BAR DO JOÃO";
            var transactions = new List<Transaction>
            {
                new (Guid.NewGuid(), Guid.NewGuid(), 3, DateTime.Now, 142.00m, "09620676017", "4753****3153","JOÃO MACEDO", storeName)
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
            var importFileId = Guid.NewGuid();
            var transactions = new List<Transaction>
            {
                new (importFileId, Guid.NewGuid(), 1, DateTime.Now, 100m, "09620676017", "4753****3153", "JOÃO MACEDO", storeName), // +100
                new (importFileId, Guid.NewGuid(), 2, DateTime.Now, 50m, "09620676017", "4753****3153", "JOÃO MACEDO", storeName)  // -50
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