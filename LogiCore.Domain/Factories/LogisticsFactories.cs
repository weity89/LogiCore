using LogiCore.Domain.Cargo;
using LogiCore.Domain.Exceptions;
using LogiCore.Domain.Vehicle;

namespace LogiCore.Domain.Factories;

public static class VehicleFactory
{
    public static Vehicle.Vehicle Create(string type, string regNumber, double maxLoad, double maxVolume, double speed, decimal rate)
    {
        return type.ToLower() switch
        {
            "truck" => new Truck(regNumber, maxLoad, maxVolume, speed, rate, tollRoadsCoefficient: 1.2m),
            "refrigerator" => new RefrigeratorTruck(regNumber, maxLoad, maxVolume, speed, rate, minTemp: -20, maxTemp: 10),
            "plane" => new CargoPlane(regNumber, maxLoad, maxVolume, speed, rate, costPerKg: 2.5m),
            "ship" => new CargoShip(regNumber, maxLoad, maxVolume, speed, rate),
            "drone" => new DroneCourier(regNumber, maxLoad, maxVolume, speed, rate, maxRangeKm: 50),
            _ => throw new LogisticsException($"Неизвестный тип транспорта: {type}")
        };
    }
}

public static class CargoFactory
{
    public static Cargo.Cargo Create(string type, string desc, double weight, double volume, decimal value)
    {
        return type.ToLower() switch
        {
            "standard" => new StandardCargo(desc, weight, volume, value),
            "perishable" => new PerishableCargo(desc, weight, volume, value, DateTime.Now.AddDays(5), -5),
            "fragile" => new FragileCargo(desc, weight, volume, value, riskCoefficient: 1.5),
            "dangerous" => new DangerousCargo(desc, weight, volume, value, hazardClass: 3),
            "oversized" => new OversizedCargo(desc, weight, volume, value),
            _ => throw new LogisticsException($"Неизвестный тип груза: {type}")
        };
    }
}