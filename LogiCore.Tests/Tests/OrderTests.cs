using LogiCore.Domain.Common;
using LogiCore.Domain.Exceptions;
using LogiCore.Domain.Order;
using LogiCore.Domain.Vehicle;
using LogiCore.Domain.Cargo;
using Xunit;

namespace LogiCore.Tests;

public class OrderTests
{
    private readonly Customer _customer = new("Test", "123");
    private readonly Route _route;
    private readonly List<LogiCore.Domain.Cargo.Cargo> _cargos;

    public OrderTests()
    {
        _route = new Route(new[] { new RoutePoint(55, 37, "A"), new RoutePoint(56, 38, "B") });
        _cargos = new List<LogiCore.Domain.Cargo.Cargo> { new StandardCargo("Test", 100, 1, 1000) };
    }

    [Fact]
    public void Order_StartDelivery_ThrowsIfNotAssigned()
    {
        var order = new Order(_customer, _route, _cargos);
        Assert.Throws<InvalidOrderStateException>(() => order.StartDelivery());
    }

    [Fact]
    public void Order_Complete_ThrowsIfNotInTransit()
    {
        var order = new Order(_customer, _route, _cargos);
        order.Assign(new Truck("TRK001", 100, 10, 80, 10, 1), 1000);
        Assert.Throws<InvalidOrderStateException>(() => order.Complete());
    }

    [Fact]
    public void Order_Cancel_AllowedFromCreatedAndAssigned()
    {
        var order = new Order(_customer, _route, _cargos);
        order.Cancel();
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }
}