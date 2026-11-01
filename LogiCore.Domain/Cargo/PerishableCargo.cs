namespace LogiCore.Domain.Cargo;

public class PerishableCargo : Cargo, ITemperatureSensitive, IInsurable
{
    public DateTime ExpirationDate { get; }

    private readonly double _requiredTemperatureC;

    double ITemperatureSensitive.RequiredTemperatureC => _requiredTemperatureC;

    public PerishableCargo(string description, double weightKg, double volumeM3, decimal declaredValue, 
        DateTime expirationDate, double requiredTemperatureC)
        : base(description, weightKg, volumeM3, declaredValue)
    {
        ExpirationDate = expirationDate;
        _requiredTemperatureC = requiredTemperatureC;
    }
}