using SpaceBattle.Lib;
using Xunit;

namespace SpaceBattle.Tests;

public class InMemoryGameRepositoryTests
{
    [Fact]
    public void Save_ShouldStoreEntity_WhenEntityIsValid()
    {
        var repository = new InMemoryGameRepository();
        var entity = new Dictionary<string, object>
        {
            { InMemoryGameRepository.EntityIdKey, "item-1" },
            { "type", "Ship" }
        };

        repository.Save(entity);

        Assert.Equal(entity, repository.Retrieve("item-1"));
    }

    [Fact]
    public void Save_ShouldThrowArgumentException_WhenNoId()
    {
        var repository = new InMemoryGameRepository();
        var entity = new Dictionary<string, object> { { "type", "Ship" } };

        Assert.Throws<ArgumentException>(() => repository.Save(entity));
    }

    [Fact]
    public void Retrieve_ShouldThrowKeyNotFoundException_WhenIdNotFound()
    {
        var repository = new InMemoryGameRepository();
        Assert.Throws<KeyNotFoundException>(() => repository.Retrieve("missing"));
    }

    [Fact]
    public void Remove_ShouldRemoveEntity_WhenIdExists()
    {
        var repository = new InMemoryGameRepository();
        var entity = new Dictionary<string, object> { { InMemoryGameRepository.EntityIdKey, "item-1" } };
        repository.Save(entity);

        repository.Remove("item-1");

        Assert.Throws<KeyNotFoundException>(() => repository.Retrieve("item-1"));
    }
}

