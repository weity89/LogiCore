namespace LogiCore.Domain.Cargo;

public class OversizedCargo : Cargo
{
    public OversizedCargo(string description, double weightKg, double volumeM3, decimal declaredValue)
        : base(description, weightKg, volumeM3, declaredValue)
    {
    }
}