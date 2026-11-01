using System.Collections;
using LogiCore.Domain.Abstractions;
using LogiCore.Domain.Common;
using LogiCore.Domain.Exceptions;

namespace LogiCore.Domain.Repositories;

public class Repository<T> : IReadOnlyRepository<T>, IEnumerable<T> where T : class, IEntity
{
    private readonly Dictionary<Guid, T> _storage = new();

    public void Add(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        if (_storage.ContainsKey(entity.Id))
            throw new LogisticsException($"Сущность с Id {entity.Id} уже существует в репозитории.");
        
        _storage.Add(entity.Id, entity);
    }

    public void Remove(Guid id)
    {
        if (!_storage.Remove(id))
            throw new LogisticsException($"Сущность с Id {id} не найдена.");
    }
    
    public IEnumerable<T> FindAll(Predicate<T> predicate)
    {
        foreach (var entity in _storage.Values)
        {
            if (predicate(entity)) yield return entity;
        }
    }

    public T? GetById(Guid id)
    {
        return _storage.TryGetValue(id, out var entity) ? entity : null;
    }

    public IEnumerable<T> GetAll()
    {
        return _storage.Values;
    }
    
    public T this[Guid id]
    {
        get
        {
            var entity = GetById(id);
            if (entity == null) throw new KeyNotFoundException($"Сущность с Id {id} не найдена.");
            return entity;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var entity in _storage.Values)
        {
            yield return entity;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}