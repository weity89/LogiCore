using LogiCore.Domain.Cargo;
using LogiCore.Domain.Common;

namespace LogiCore.Domain.Vehicle;

public class RefrigeratorTruck : Vehicle
{
    public double MinTemperatureC { get; }
    public double MaxTemperatureC { get; }

    public RefrigeratorTruck(string registrationNumber, double maxLoadKg, double maxVolumeM3, double averageSpeedKmH, decimal baseRatePerKm, double minTemp, double maxTemp) 
        : base(registrationNumber, maxLoadKg, maxVolumeM3, averageSpeedKmH, baseRatePerKm)
    {
        MinTemperatureC = minTemp;
        MaxTemperatureC = maxTemp;
    }

    public override decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo.Cargo> cargo)
    {
        var baseCost = (decimal)route.DistanceKm * BaseRatePerKm;
        return baseCost * 1.1m;
    }

    public override bool CanCarry(Cargo.Cargo cargo)
    {
        if (!base.CanCarry(cargo)) return false;

        if (cargo is ITemperatureSensitive tempCargo)
        {
            var temp = tempCargo.RequiredTemperatureC;
            return temp >= MinTemperatureC && temp <= MaxTemperatureC;
        }
        return true;
    }
}