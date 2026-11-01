using LogiCore.Domain.Cargo;
using LogiCore.Domain.Common;

namespace LogiCore.Domain.Vehicle;

public class CargoShip : Vehicle
{
    public CargoShip(string registrationNumber, double maxLoadKg, double maxVolumeM3, double averageSpeedKmH, decimal baseRatePerKm) 
        : base(registrationNumber, maxLoadKg, maxVolumeM3, averageSpeedKmH, baseRatePerKm)
    {
    }

    public override decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo.Cargo> cargo)
    {
        var baseCost = (decimal)route.DistanceKm * BaseRatePerKm;
        decimal oversizedSurcharge = 0;
        if (cargo.Any(c => c is OversizedCargo))
        {
            oversizedSurcharge = baseCost * 0.2m;
        }
        return baseCost + oversizedSurcharge;
    }
}