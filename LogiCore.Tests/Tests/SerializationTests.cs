using LogiCore.App.Infrastructure;
using LogiCore.Domain.Order;
using LogiCore.Domain.Vehicle;
using Xunit;

namespace LogiCore.Tests;

public class SerializationTests
{
    [Fact]
    public void StateManager_SaveAndLoad_ReturnsEquivalentData()
    {
        var manager = new StateManager();
        var fleet = new List<Vehicle> { new Truck("TRK001", 10000, 50, 80, 10, 1.2m) };
        var orders = new List<Order>();
        
        manager.SaveState(fleet, orders);
        var loaded = manager.LoadState();
        
        Assert.NotNull(loaded);
        Assert.Equal(fleet.Count, loaded.Value.fleet.Count);
    }
}