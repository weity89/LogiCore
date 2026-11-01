using LogiCore.Domain.Common;
using LogiCore.Domain.Exceptions;

namespace LogiCore.Domain.Cargo;

public abstract class Cargo : IEntity
{
    public Guid Id { get; }
    public string Description { get; }
    public double WeightKg { get; }
    public double VolumeM3 { get; }
    public decimal DeclaredValue { get; }

    protected Cargo(string description, double weightKg, double volumeM3, decimal declaredValue)
    {
        if (weightKg <= 0) throw new CargoValidationException("Вес груза должен быть строго больше нуля.");
        if (volumeM3 <= 0) throw new CargoValidationException("Объем груза должен быть строго больше нуля.");

        Id = Guid.NewGuid();
        Description = description;
        WeightKg = weightKg;
        VolumeM3 = volumeM3;
        DeclaredValue = declaredValue;
    }

    public override string ToString() => $"{GetType().Name} '{Description}' ({WeightKg} кг, {VolumeM3} м3)";
}