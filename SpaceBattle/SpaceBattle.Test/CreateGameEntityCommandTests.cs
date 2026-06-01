using Moq;
using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Tests;

public class CreateGameEntityCommandTests
{
    [Fact]
    public void Execute_ShouldGenerateId_WhenIdIsMissing()
    {
        var mockRepo = new Mock<IGameRepository>();
        var gameObject = new Dictionary<string, object>();
        var command = new CreateGameEntityCommand(mockRepo.Object, gameObject);

        command.Execute();

        Assert.True(gameObject.ContainsKey(InMemoryGameRepository.EntityIdKey));
        Assert.False(string.IsNullOrEmpty(gameObject[InMemoryGameRepository.EntityIdKey].ToString()));
        mockRepo.Verify(r => r.Save(gameObject), Times.Once);
    }

    [Fact]
    public void Execute_ShouldNotOverwriteId_WhenIdAlreadyExists()
    {
        var mockRepo = new Mock<IGameRepository>();
        var presetId = "123";
        var gameObject = new Dictionary<string, object> { { InMemoryGameRepository.EntityIdKey, presetId } };
        var command = new CreateGameEntityCommand(mockRepo.Object, gameObject);

        command.Execute();

        Assert.Equal(presetId, gameObject[InMemoryGameRepository.EntityIdKey]);
        mockRepo.Verify(r => r.Save(gameObject), Times.Once);
    }

    [Fact]
    public void Execute_ShouldPropagateException_WhenRepositoryThrows()
    {
        var mockRepo = new Mock<IGameRepository>();
        var gameObject = new Dictionary<string, object> { { InMemoryGameRepository.EntityIdKey, "dup" } };
        mockRepo.Setup(r => r.Save(It.IsAny<IDictionary<string, object>>())).Throws<Exception>();

        var command = new CreateGameEntityCommand(mockRepo.Object, gameObject);

        Assert.Throws<Exception>(() => command.Execute());
    }
}

