using Application.Dtos;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using FixedWidthParserWriter;
using Moq;

namespace ByCodersTec.Application.Services
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _mockRepository;
        private readonly TransactionService _service;

        public TransactionServiceTests()
        {
            _mockRepository = new Mock<ITransactionRepository>();
            _service = new TransactionService(_mockRepository.Object);
        }

        //[Fact]
        //public async Task ProcessCnabFileAsync_Stream_ValidLines_CallsRepositoryWithCorrectData()
        //{
        //    // Arrange
        //    var lines = new List<string>
        //    {
        //        "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO        "
        //    };
        //    var stream = CreateStreamFromLines(lines);
        //    var fileName = "CNAB.txt";
        //    var userId = "user123";

        //    var expectedDto = new TransactionDto
        //    {
        //        Type = 3,
        //        Date = "20190301",
        //        Value = 142,
        //        Cpf = "09620676017",
        //        Card = "4753****3153",
        //        Time = "153453",
        //        StoreOwner = "JOÃO MACEDO",
        //        StoreName = "BAR DO JOÃO"
        //    };

        //    var mockConfig = new Mock<FixedWidthConfig>();
        //    var mockParser = new Mock<IFixedWidthLinesProvider<TransactionDto>>();
        //    mockParser
        //        .Setup(p => p.Parse(It.IsAny<List<string>>(), It.IsAny<int>()))
        //        .Returns([expectedDto]);

        //    // Use reflection to inject mock parser (since FixedWidthLinesProvider is concrete)
        //    var serviceField = typeof(TransactionService).GetField("<FixedWidthLinesProvider>1__FixedWidthLinesProvider",
        //        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        //    serviceField?.SetValue(_service, mockParser.Object);

        //    var expectedImportFile = It.IsAny<ImportFile>();
        //    var expectedTransactions = It.IsAny<List<Transaction>>();

        //    // Act
        //    await _service.ProcessCnabFileAsync(stream, fileName, userId);

        //    // Assert
        //    _mockRepository.Verify(r => r.SaveTransactionsAsync(
        //        It.Is<ImportFile>(f =>
        //            f.FileName == fileName &&
        //            f.UserId == userId &&
        //            f.TotalRows == 1 &&
        //            f.ImportedRows == 1
        //        ),
        //        It.Is<List<Transaction>>(t =>
        //            t.Count == 1 &&
        //            t[0].Type == 3 &&
        //            t[0].Value == 1.42m &&
        //            t[0].Cpf == "09620676017" &&
        //            t[0].StoreName == "BAR DO JOÃO"
        //        )
        //    ), Times.Once);
        //}

        //[Fact]
        //public async Task ProcessCnabFileAsync_List_ValidAndInvalidLines_OnlySavesValid()
        //{
        //    // Arrange
        //    var validLine1 = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO        ";
        //    var validLine2 = "5201903010000013200556418150633123****7687145607MARIA JOSEFINALOJA DO Ó - MATRIZ ";
        //    var invalidLine = "invalid";
        //    var validLine1lenght = validLine1.Length;
        //    var validLine2lenght = validLine2.Length;
        //    var lines = new List<string> { validLine1, validLine2, invalidLine };

        //    var dtos = new List<TransactionDto>
        //    {
        //        new() { Type = 3, Value = 142, Cpf = "09620676017", StoreName = "BAR DO JOÃO", StoreOwner = "JOÃO MACEDO", Date = "20190301", Time = "153453", Card = "4753****3153" },
        //        new() { Type = 5, Value = 132, Cpf = "55641815063", StoreName = "LOJA DO Ó - MATRIZ ", StoreOwner = "MARIA JOSEFINA", Date = "20190301", Time = "145607", Card = "3123****7687" }
        //    };

        //    var mockParser = new Mock<IFixedWidthLinesProvider<TransactionDto>>();
        //    mockParser.Setup(p => p.Parse(It.IsAny<List<string>>(), It.IsAny<int> ())).Returns(dtos);

        //    InjectParser(mockParser.Object);

        //    // Act
        //    await _service.ProcessCnabFileAsync(lines, "test.txt", "user1");

        //    // Assert
        //    _mockRepository.Verify(r => r.SaveTransactionsAsync(
        //        It.Is<ImportFile>(f => f.TotalRows == 3 && f.ImportedRows == 2),
        //        It.Is<List<Transaction>>(t => t.Count == 2)
        //    ), Times.Once);
        //}

        [Fact]
        public async Task ProcessCnabFileAsync_List_ValidAndInvalidLines_ThrowsArgumentException()
        {
            // Arrange
            var validLine = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO";
            var invalidLine = "invalid";
            var validLinelenght = validLine.Length;
            var lines = new List<string> { validLine, invalidLine };

            var dtos = new List<TransactionDto>
            {
                new() { Type = 3, Value = 142, Cpf = "09620676017", StoreName = "BAR DO JOÃO", StoreOwner = "JOÃO MACEDO", Date = "20190301", Time = "153453", Card = "4753****3153" }
            };

            var mockParser = new Mock<IFixedWidthLinesProvider<TransactionDto>>();
            mockParser.Setup(p => p.Parse(It.IsAny<List<string>>(), It.IsAny<int>())).Returns(dtos);

            InjectParser(mockParser.Object);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.ProcessCnabFileAsync(lines, "test.txt", "user1"));
            Assert.Contains("No valid data provided", ex.Result.Message);
        }

        [Fact]
        public async Task GetTransactionsByStoreAsync_ValidStoreName_CallsRepository()
        {
            // Arrange
            var storeName = "BAR DO JOÃO";
            var transactions = new List<Transaction> { new(Guid.NewGuid(), Guid.NewGuid(), 1, DateTime.Today, 100m, "10299795020", "6228****9090", "owner", storeName) };
            _mockRepository.Setup(r => r.GetByStoreNameAsync(storeName)).ReturnsAsync(transactions);

            // Act
            var result = await _service.GetTransactionsByStoreAsync(storeName);

            // Assert
            Assert.Equal(transactions, result);
            _mockRepository.Verify(r => r.GetByStoreNameAsync(storeName), Times.Once);
        }

        [Fact]
        public void GetTransactionsByStoreAsync_NullStoreName_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.GetTransactionsByStoreAsync(string.Empty));
            Assert.Contains("Store name is required", ex.Result.Message);
        }

        [Fact]
        public void GetTransactionsByStoreAsync_EmptyStoreName_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.GetTransactionsByStoreAsync(""));
            Assert.Contains("Store name is required", ex.Result.Message);
        }

        [Fact]
        public async Task GetBalanceByStoreAsync_ValidStoreName_ReturnsBalance()
        {
            // Arrange
            var storeName = "LOJA DO Ó";
            var expectedBalance = 500.75m;
            _mockRepository.Setup(r => r.GetBalanceByStoreNameAsync(storeName)).ReturnsAsync(expectedBalance);

            // Act
            var result = await _service.GetBalanceByStoreAsync(storeName);

            // Assert
            Assert.Equal(expectedBalance, result);
            _mockRepository.Verify(r => r.GetBalanceByStoreNameAsync(storeName), Times.Once);
        }

        [Fact]
        public void GetBalanceByStoreAsync_NullStoreName_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.GetBalanceByStoreAsync(string.Empty));
            Assert.Contains("Store name is required", ex.Result.Message);
        }

        [Fact]
        public async Task GetAllStoreNamesAsync_CallsRepositoryAndReturnsNames()
        {
            // Arrange
            var storeNames = new List<string> { "Store A", "Store B" };
            _mockRepository.Setup(r => r.GetAllStoreNamesAsync()).ReturnsAsync(storeNames);

            // Act
            var result = await _service.GetAllStoreNamesAsync();

            // Assert
            Assert.Equal(storeNames, result);
            _mockRepository.Verify(r => r.GetAllStoreNamesAsync(), Times.Once);
        }

        [Fact]
        public async Task ProcessCnabFileAsync_Stream_NullStream_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.ProcessCnabFileAsync(fileStream: null!, "file.txt", "user1"));
        }

        [Fact]
        public async Task ProcessCnabFileAsync_List_NullList_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.ProcessCnabFileAsync([], "file.txt", "user1"));
        }

        // Helper: Inject mock parser via reflection
        private void InjectParser(IFixedWidthLinesProvider<TransactionDto> parser)
        {
            var field = typeof(TransactionService).GetField("_parser", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(_service, parser);
        }

        // Helper: Create stream from lines
        private Stream CreateStreamFromLines(IEnumerable<string> lines)
        {
            var content = string.Join(Environment.NewLine, lines);
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}

public interface IFixedWidthLinesProvider<T> where T : class, new()
{
    List<T> Parse(IEnumerable<string> dataLines, int configType);
}