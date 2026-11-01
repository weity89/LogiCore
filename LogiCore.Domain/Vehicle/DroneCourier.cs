using LogiCore.Domain.Cargo;
using LogiCore.Domain.Common;

namespace LogiCore.Domain.Vehicle;

public sealed class DroneCourier : Vehicle
{
    public double MaxRangeKm { get; }

    public DroneCourier(string registrationNumber, double maxLoadKg, double maxVolumeM3, double averageSpeedKmH, decimal baseRatePerKm, double maxRangeKm) 
        : base(registrationNumber, maxLoadKg, maxVolumeM3, averageSpeedKmH, baseRatePerKm)
    {
        Conditions = TransportConditions.LongRange; 
        
        MaxRangeKm = maxRangeKm;
    }

    public override decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo.Cargo> cargo)
    {
        if (route.DistanceKm > MaxRangeKm)
            throw new InvalidOperationException("Маршрут превышает максимальную дальность дрона.");
            
        return (decimal)route.DistanceKm * BaseRatePerKm;
    }

    public override bool CanCarry(Cargo.Cargo cargo)
    {
        if (!base.CanCarry(cargo)) return false;
        if (cargo.WeightKg > 5) return false;
        return true;
    }
}