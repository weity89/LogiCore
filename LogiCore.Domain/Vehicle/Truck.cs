using LogiCore.Domain.Cargo;
using LogiCore.Domain.Common;

namespace LogiCore.Domain.Vehicle;

public class Truck : Vehicle
{
    public decimal TollRoadsCoefficient { get; }

    public Truck(string registrationNumber, double maxLoadKg, double maxVolumeM3, double averageSpeedKmH, decimal baseRatePerKm, decimal tollRoadsCoefficient) 
        : base(registrationNumber, maxLoadKg, maxVolumeM3, averageSpeedKmH, baseRatePerKm)
    {
        TollRoadsCoefficient = tollRoadsCoefficient;
    }

    public override decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo.Cargo> cargo)
    {
        var baseCost = (decimal)route.DistanceKm * BaseRatePerKm;
        return baseCost * TollRoadsCoefficient;
    }
}