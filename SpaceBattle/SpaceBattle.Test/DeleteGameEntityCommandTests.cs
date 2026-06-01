using SpaceBattle.Lib;
using Moq;
using Xunit;

namespace SpaceBattle.Tests;

public class DeleteGameEntityCommandTests
{
    [Fact]
    public void Execute_ShouldCallRemoveOnRepository()
    {
        var mockRepo = new Mock<IGameRepository>();
        var id = "item-1";
        var command = new DeleteGameEntityCommand(mockRepo.Object, id);

        command.Execute();

        mockRepo.Verify(r => r.Remove(id), Times.Once);
    }
}

