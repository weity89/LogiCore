namespace LogiCore.Domain.Cargo;

public class StandardCargo : Cargo, IInsurable, IStackable
{
    public bool IsStackable => true;

    public StandardCargo(string description, double weightKg, double volumeM3, decimal declaredValue)
        : base(description, weightKg, volumeM3, declaredValue)
    {
    }
}