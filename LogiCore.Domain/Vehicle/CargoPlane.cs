using LogiCore.Domain.Cargo;
using LogiCore.Domain.Common;

namespace LogiCore.Domain.Vehicle;

public class CargoPlane : Vehicle
{
    public decimal CostPerKg { get; }

    public CargoPlane(string registrationNumber, double maxLoadKg, double maxVolumeM3, double averageSpeedKmH, decimal baseRatePerKm, decimal costPerKg) 
        : base(registrationNumber, maxLoadKg, maxVolumeM3, averageSpeedKmH, baseRatePerKm)
    {
        CostPerKg = costPerKg;
    }

    public override decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo.Cargo> cargo)
    {
        var baseCost = (decimal)route.DistanceKm * BaseRatePerKm;
        var weightCost = cargo.Sum(c => (decimal)c.WeightKg * CostPerKg);
        return baseCost + weightCost;
    }

    public override bool CanCarry(Cargo.Cargo cargo)
    {
        if (!base.CanCarry(cargo)) return false;
        if (cargo is DangerousCargo dangerous && dangerous.HazardClass > 3) return false;
        return true;
    }
}