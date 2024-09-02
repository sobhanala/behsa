using System.Globalization;
using Application.DTOs.Transaction;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Mappers;
using Application.Services.DomainService;
using Domain.Entities;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace test.Application.UnitTests.Services.DomainService;

public class TransactionServiceTests
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IFileReaderService _fileReaderService;
    private readonly TransactionService _transactionService;

    public TransactionServiceTests()
    {
        _transactionRepository = Substitute.For<ITransactionRepository>();
        _fileReaderService = Substitute.For<IFileReaderService>();
        _transactionService = new TransactionService(_transactionRepository, _fileReaderService);
    }

    [Fact]
    public async Task AddTransactionsFromCsvAsync_ShouldReturnOk_WhenTransactionsAreAddedSuccessfully()
    {
        // Arrange
        var filePath = "test.csv";
        var transactionCsvModels = new List<TransactionCsvModel>
        {
            new() { TransactionID = 1, SourceAcount = 101, DestiantionAccount = 102, Amount = 100, Date = new DateTime(1399, 04, 15, new PersianCalendar()), Type = "کارت به کارت" },
            new() { TransactionID = 2, SourceAcount = 101, DestiantionAccount = 103, Amount = 200, Date = new DateTime(1399, 04, 30, new PersianCalendar()), Type = "ساتنا" }
        };
        // Expect these dates to be converted to Gregorian:
        var expectedFirstDate = new DateTime(2020, 7, 5); // 1399/04/15 in Gregorian Calendar
        var expectedSecondDate = new DateTime(2020, 7, 20); // 1399/04/30 in Gregorian Calendar

        var transactions = transactionCsvModels.Select(csvModel => csvModel.ToTransaction()).ToList();
        var existingTransactionIds = new List<long> { 3 };

        _fileReaderService.ReadFromFile<TransactionCsvModel>(filePath).Returns(transactionCsvModels);
        _transactionRepository.GetAllIdsAsync().Returns(existingTransactionIds);
        _transactionRepository.CreateBulkAsync(Arg.Any<List<Transaction>>()).Returns(Task.CompletedTask);

        // Act
        var result = await _transactionService.AddTransactionsFromCsvAsync(filePath);

        // Assert
        Assert.True(result.Succeed);
        
        // Verifying if the dates were converted correctly
        await _transactionRepository.Received(1).CreateBulkAsync(Arg.Is<List<Transaction>>(x =>
                x.Count == transactionCsvModels.Count &&
                x[0].Date == expectedFirstDate && // Ensure the first date was converted correctly
                x[1].Date == expectedSecondDate // Ensure the second date was converted correctly
        ));
    }
    
    [Fact]
    public async Task AddTransactionsFromCsvAsync_ShouldOnlyAddNewTransactions_WhenSomeTransactionsAlreadyExist()
    {
        // Arrange
        var filePath = "test.csv";
        var transactionCsvModels = new List<TransactionCsvModel>
        {
            new() { TransactionID = 1, SourceAcount = 101, DestiantionAccount = 102, Amount = 100, Date = new DateTime(1399, 04, 29, new PersianCalendar()), Type = "کارت به کارت" },
            new() { TransactionID = 2, SourceAcount = 101, DestiantionAccount = 103, Amount = 200, Date = new DateTime(1399, 04, 15, new PersianCalendar()), Type = "ساتنا" },
            new() { TransactionID = 3, SourceAcount = 104, DestiantionAccount = 105, Amount = 300, Date = new DateTime(1399, 04, 17, new PersianCalendar()), Type = "ساتنا"}
        };

        var transactions = transactionCsvModels.Select(csvModel => csvModel.ToTransaction()).ToList();
    
        // Let's say TransactionID = 3 already exists in the database.
        var existingTransactionIds = new List<long> { 3 };

        _fileReaderService.ReadFromFile<TransactionCsvModel>(filePath).Returns(transactionCsvModels);
        _transactionRepository.GetAllIdsAsync().Returns(existingTransactionIds);
        _transactionRepository.CreateBulkAsync(Arg.Any<List<Transaction>>()).Returns(Task.CompletedTask);

        // Act
        var result = await _transactionService.AddTransactionsFromCsvAsync(filePath);

        // Assert
        Assert.True(result.Succeed);

        // Only the new transactions (TransactionID = 1 and TransactionID = 2) should be added.
        await _transactionRepository.Received(1).CreateBulkAsync(Arg.Is<List<Transaction>>(x =>
            x.Count == 2 &&
            x.Any(t => t.TransactionId == 1) &&
            x.Any(t => t.TransactionId == 2) &&
            x.All(t => t.TransactionId != 3)
        ));
    }


    [Fact]
    public async Task AddTransactionsFromCsvAsync_ShouldReturnFail_WhenExceptionIsThrown()
    {
        // Arrange
        var filePath = "test.csv";
        var exceptionMessage = "An error occurred while reading the file.";
            
        _fileReaderService
            .ReadFromFile<TransactionCsvModel>(filePath)
            .Throws(new Exception(exceptionMessage));
        
        // Act
        var result = await _transactionService.AddTransactionsFromCsvAsync(filePath);

        // Assert
        Assert.False(result.Succeed);
        Assert.Equal($"An error occurred: {exceptionMessage}", result.Message);
    }
    
    [Fact]
    public async Task GetAllTransactionsAsync_ShouldReturnAllTransactions()
    {
        // Arrange
        var transactions = new List<Transaction>
        {
            new() { TransactionId = 1, SourceAccountId = 101, DestinationAccountId = 102, Amount = 100, Date = DateTime.UtcNow, Type = "کارت به کارت" },
            new() { TransactionId = 2, SourceAccountId = 101, DestinationAccountId = 103, Amount = 200, Date = DateTime.UtcNow, Type = "ساتنا" }
        };

        _transactionRepository.GetAllTransactions().Returns(transactions);

        // Act
        var result = await _transactionService.GetAllTransactionsAsync();

        // Assert
        Assert.True(result.Succeed);
        Assert.Equal(transactions, result.Value);
    }
    
    [Fact]
    public async Task GetAllTransactionsAsync_ShouldReturnFailResult_WhenExceptionIsThrown()
    {
        // Arrange
        var exceptionMessage = "Database connection failed.";
        _transactionRepository.GetAllTransactions().Throws(new Exception(exceptionMessage));

        // Act
        var result = await _transactionService.GetAllTransactionsAsync();

        // Assert
        Assert.False(result.Succeed);
        Assert.Null(result.Value);
        Assert.Equal($"An error occurred: {exceptionMessage}", result.Message);
    }

    [Fact]
    public async Task GetAllTransactionsAsync_ShouldReturnEmptyList_WhenNoTransactionsAreFound()
    {
        // Arrange
        var transactions = new List<Transaction>();
        _transactionRepository.GetAllTransactions().Returns(Task.FromResult(transactions));

        // Act
        var result = await _transactionService.GetAllTransactionsAsync();

        // Assert
        Assert.True(result.Succeed);
        Assert.Empty(result.Value!);
    }
    
    [Fact]
    public async Task GetTransactionsByAccountIdAsync_ShouldReturnOkResult_WhenTransactionsAreRetrievedAndGroupedSuccessfully()
    {
        // Arrange
        long accountId = 101;
        var transactionsSourceAccount = new List<Transaction>
        {
            new Transaction { TransactionId = 1, SourceAccountId = 101, DestinationAccountId = 102, Amount = 100, Date = new DateTime(2023, 7, 5), Type = "کارت به کارت" },
            new Transaction { TransactionId = 2, SourceAccountId = 101, DestinationAccountId = 103, Amount = 200, Date = new DateTime(2023, 7, 6), Type = "ساتنا" }
        };
        
        var transactionsDestinationAccount = new List<Transaction>
        {
            new Transaction { TransactionId = 3, SourceAccountId = 104, DestinationAccountId = 101, Amount = 300, Date = new DateTime(2023, 7, 7), Type = "ساتنا" },
            new Transaction { TransactionId = 4, SourceAccountId = 105, DestinationAccountId = 101, Amount = 400, Date = new DateTime(2023, 7, 8), Type = "کارت به کارت" }
        };

        _transactionRepository.GetBySourceAccountId(accountId).Returns(transactionsSourceAccount);
        _transactionRepository.GetByDestinationAccountId(accountId).Returns(transactionsDestinationAccount);

        // Act
        var result = await _transactionService.GetTransactionsByAccountIdAsync(accountId);

        // Assert
        Assert.True(result.Succeed); 
        Assert.Equal(4, result.Value!.Count);
    }
    
    [Fact]
    public async Task GetTransactionsByAccountIdAsync_ShouldReturnOkResult_WhenThereAreMultipleTransactionsBetweenTwoAccounts()
    {
        // Arrange
        long accountId = 101;
        var transactionsSourceAccount = new List<Transaction>
        {
            new Transaction { TransactionId = 1, SourceAccountId = 101, DestinationAccountId = 102, Amount = 100, Date = new DateTime(2023, 7, 5), Type = "کارت به کارت" },
            new Transaction { TransactionId = 2, SourceAccountId = 101, DestinationAccountId = 102, Amount = 200, Date = new DateTime(2023, 7, 6), Type = "ساتنا" }
        };
        
        var transactionsDestinationAccount = new List<Transaction>
        {
            new Transaction { TransactionId = 3, SourceAccountId = 104, DestinationAccountId = 101, Amount = 300, Date = new DateTime(2023, 7, 7), Type = "ساتنا" },
            new Transaction { TransactionId = 4, SourceAccountId = 102, DestinationAccountId = 101, Amount = 400, Date = new DateTime(2023, 7, 8), Type = "کارت به کارت" }
        };

        _transactionRepository.GetBySourceAccountId(accountId).Returns(transactionsSourceAccount);
        _transactionRepository.GetByDestinationAccountId(accountId).Returns(transactionsDestinationAccount);

        // Act
        var result = await _transactionService.GetTransactionsByAccountIdAsync(accountId);

        // Assert
        Assert.True(result.Succeed);
        Assert.Equal(4, result.Value!.SelectMany(x => x.TransactionWithSources).Count());
        Assert.Equal(2, result.Value!.Count);
    }

    [Fact]
    public async Task GetTransactionsByAccountIdAsync_ShouldReturnEmptyList_WhenNoTransactionsAreFound()
    {
        // Arrange
        long accountId = 101;
        _transactionRepository.GetBySourceAccountId(accountId).Returns(new List<Transaction>());
        _transactionRepository.GetByDestinationAccountId(accountId).Returns(new List<Transaction>());

        // Act
        var result = await _transactionService.GetTransactionsByAccountIdAsync(accountId);

        // Assert
        Assert.True(result.Succeed);
        Assert.Empty(result.Value!);
    }

    [Fact]
    public async Task GetTransactionsByAccountIdAsync_ShouldReturnFailResult_WhenExceptionIsThrown()
    {
        // Arrange
        long accountId = 101;
        var exceptionMessage = "Database error";
        _transactionRepository.GetBySourceAccountId(accountId).Throws(new Exception(exceptionMessage));

        // Act
        var result = await _transactionService.GetTransactionsByAccountIdAsync(accountId);

        // Assert
        Assert.False(result.Succeed);
        Assert.Null(result.Value);
        Assert.Equal($"An error occurred: {exceptionMessage}", result.Message);
    }
    
    
}