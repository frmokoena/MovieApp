using Moq;
using Movies.Data.UnitTests;
using System.Data;

namespace Movies.Data;

public class UnitOfWorkTest
{
    [Fact]
    public async Task QueryMapsToCorretResult()
    {
        // Arrange
        var connectionMock = new Mock<IDbConnection>();
        var queryMock = new Mock<IQuery<FakeEntity>>();

        var response = new FakeEntity
        {
            Id = "120",
            Name = "It Is a Test"
        };

        queryMock.Setup(
            m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(response);

        var uom = new UnitOfWork(connectionMock.Object);

        // Act
        var result = await uom.QueryAsync(queryMock.Object);

        // Asset
        connectionMock.Verify();
        Assert.Equal(response.Id, result.Id);
    }

    [Fact]
    public void CommandExecuteSuccessfully()
    {
        // Arrange
        var connectionMock = new Mock<IDbConnection>();
        var commandMock = new Mock<ICommand>();

        var response = Task.CompletedTask;

        commandMock.Setup(
            m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<CancellationToken>())
            )
            .Returns(new Task<Task>(() => response));

        var uom = new UnitOfWork(connectionMock.Object);

        // Act
        var result = uom.ExecuteAsync(commandMock.Object);

        // Asset
        connectionMock.Verify();
    }

    [Fact]
    public async Task CommandFailsWithExceptionIfTransactionalAndNoTransaction()
    {
        // Arrange
        var connectionMock = new Mock<IDbConnection>();
        var commandMock = new Mock<ICommand>();

        commandMock.Setup(m => m.RequiresTransaction)
            .Returns(true);

        var response = Task.CompletedTask;

        commandMock.Setup(
            m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<CancellationToken>())
            )
            .Returns(new Task<Task>(() => response));

        var uom = new UnitOfWork(connectionMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => uom.ExecuteAsync(commandMock.Object));
    }

    [Fact]
    public async Task CommandMapsToCorretResult()
    {
        // Arrange
        var connectionMock = new Mock<IDbConnection>();
        var commandMock = new Mock<ICommand<FakeEntity>>();

        var response = new FakeEntity
        {
            Id = "120",
            Name = "It Is a Test"
        };

        commandMock.Setup(
            m => m.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(response);

        var uom = new UnitOfWork(connectionMock.Object);

        // Act
        var result = await uom.ExecuteAsync(commandMock.Object);

        // Asset
        connectionMock.Verify();
        Assert.Equal(response.Id, result.Id);
    }

    [Fact]
    public async Task CommandFailsToMapAndThrowsExceptionIfTransactionalAndNoTransaction()
    {
        // Arrange
        var connectionMock = new Mock<IDbConnection>();
        var commandMock = new Mock<ICommand<FakeEntity>>();

        commandMock.Setup(m => m.RequiresTransaction)
            .Returns(true);

        var uom = new UnitOfWork(connectionMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => uom.ExecuteAsync(commandMock.Object));
    }

    [Fact]
    public void CandExecuteTransaction()
    {
        // Arrange
        var connectionMock = new Mock<IDbConnection>();
        var commandMock = new Mock<ICommand<FakeEntity>>();
        
        var uom = new UnitOfWork(connectionMock.Object,transactional: true);

        // Act & Assert
        connectionMock.Verify(m => m.BeginTransaction(It.IsAny<IsolationLevel>()), Times.Once);
    }
}
