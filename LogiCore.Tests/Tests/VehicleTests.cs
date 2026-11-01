using LogiCore.Domain.Cargo;
using LogiCore.Domain.Common;
using LogiCore.Domain.Vehicle;
using Xunit;

namespace LogiCore.Tests;

public class VehicleTests
{
    private readonly Route _route;
    private readonly List<LogiCore.Domain.Cargo.Cargo> _cargos;

    public VehicleTests()
    {
        _route = new Route(new[]
        {
            new RoutePoint(55.0, 37.0, "A"),
            new RoutePoint(56.0, 38.0, "B")
        });
        
        _cargos = new List<LogiCore.Domain.Cargo.Cargo> 
        { 
            new StandardCargo("Test", 100, 1, 1000) 
        };
    }

    [Fact]
    public void Truck_CalculateCost_ReturnsWithTollCoefficient()
    {
        var truck = new Truck("TRK001", 10000, 50, 80, 10, 1.2m);
        var cost = truck.CalculateDeliveryCost(_route, _cargos);
        
        Assert.True(cost > 0);
    }

    [Fact]
    public void CargoPlane_CalculateCost_ReturnsWithWeightSurcharge()
    {
        var plane = new CargoPlane("PLN001", 20000, 100, 800, 50, 2.5m);
        var cost = plane.CalculateDeliveryCost(_route, _cargos);
        
        Assert.True(cost > 0);
    }

    [Fact]
    public void DroneCourier_CalculateCost_ThrowsIfRouteTooLong()
    {
        var drone = new DroneCourier("DRN001", 10, 1, 60, 2, 50);
        Assert.Throws<InvalidOperationException>(() => drone.CalculateDeliveryCost(_route, _cargos));
    }

    [Fact]
    public void Vehicle_CanCarry_ReturnsFalseForOverweight()
    {
        var truck = new Truck("TRK001", 100, 50, 80, 10, 1.2m);
        var heavyCargo = new StandardCargo("Heavy", 200, 1, 1000);
        
        Assert.False(truck.CanCarry(heavyCargo));
    }

    [Fact]
    public void RefrigeratorTruck_CanCarry_ReturnsFalseForWrongTemperature()
    {
        var fridge = new RefrigeratorTruck("REF001", 5000, 30, 70, 15, -10, 5);
        var frozenCargo = new PerishableCargo("Frozen", 100, 1, 1000, DateTime.Now.AddDays(1), -20);
        
        Assert.False(fridge.CanCarry(frozenCargo));
    }
}