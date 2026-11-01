using LogiCore.Domain.Common;
using LogiCore.Domain.Repositories;
using Xunit;

namespace LogiCore.Tests;

public class RepositoryTests
{
    private class TestEntity : IEntity
    {
        public Guid Id { get; } = Guid.NewGuid();
    }

    [Fact]
    public void Repository_Add_And_GetById_Works()
    {
        var repo = new Repository<TestEntity>();
        var entity = new TestEntity();
        
        repo.Add(entity);
        
        Assert.Equal(entity, repo.GetById(entity.Id));
    }

    [Fact]
    public void Repository_Remove_Works()
    {
        var repo = new Repository<TestEntity>();
        var entity = new TestEntity();
        repo.Add(entity);
        
        repo.Remove(entity.Id);
        
        Assert.Null(repo.GetById(entity.Id));
    }

    [Fact]
    public void Repository_Indexer_ReturnsEntity()
    {
        var repo = new Repository<TestEntity>();
        var entity = new TestEntity();
        repo.Add(entity);
        
        Assert.Equal(entity, repo[entity.Id]);
    }

    [Fact]
    public void Repository_GetEnumerator_YieldsAll()
    {
        var repo = new Repository<TestEntity>();
        var e1 = new TestEntity();
        var e2 = new TestEntity();
        repo.Add(e1);
        repo.Add(e2);
        
        var count = 0;
        foreach (var item in repo) count++;
        
        Assert.Equal(2, count);
    }
}