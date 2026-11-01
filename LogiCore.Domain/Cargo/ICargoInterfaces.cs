namespace LogiCore.Domain.Cargo;

public interface ITemperatureSensitive
{
    double RequiredTemperatureC { get; }
}

public interface IInsurable
{
    decimal DeclaredValue { get; }
}

public interface IStackable
{
    bool IsStackable { get; }
}