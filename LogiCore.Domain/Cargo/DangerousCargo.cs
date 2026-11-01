namespace LogiCore.Domain.Cargo;

public class DangerousCargo : Cargo
{
    public int HazardClass { get; }

    public DangerousCargo(string description, double weightKg, double volumeM3, decimal declaredValue, int hazardClass)
        : base(description, weightKg, volumeM3, declaredValue)
    {
        if (hazardClass < 1 || hazardClass > 9) 
            throw new ArgumentOutOfRangeException(nameof(hazardClass), "Класс опасности должен быть от 1 до 9.");
        HazardClass = hazardClass;
    }
}