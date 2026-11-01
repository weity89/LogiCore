namespace LogiCore.Domain.Cargo;

public class PerishableCargo : Cargo, ITemperatureSensitive, IInsurable
{
    public DateTime ExpirationDate { get; }

    double ITemperatureSensitive.RequiredTemperatureC { get; }

    public PerishableCargo(string description, double weightKg, double volumeM3, decimal declaredValue, 
        DateTime expirationDate, double requiredTemperatureC)
        : base(description, weightKg, volumeM3, declaredValue)
    {
        ExpirationDate = expirationDate;
        ((ITemperatureSensitive)this).RequiredTemperatureC = requiredTemperatureC;
    }
}