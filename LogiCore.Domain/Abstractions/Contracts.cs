using LogiCore.Domain.Common;
using LogiCore.Domain.Exceptions;

namespace LogiCore.Domain.Abstractions;

public class ValidationResult
{
    public bool IsValid => !Errors.Any();
    public List<string> Errors { get; } = new();
    
    public static ValidationResult Success() => new();
    public static ValidationResult Failure(string error) => new() { Errors = { error } };
}

public interface IValidator<in T>
{
    ValidationResult Validate(T item);
}

public interface IReadOnlyRepository<out T> where T : class, IEntity
{
    T? GetById(Guid id);
    IEnumerable<T> GetAll();
}