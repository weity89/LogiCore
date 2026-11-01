using LogiCore.Domain.Cargo;
using LogiCore.Domain.Decorators;
using Xunit;

namespace LogiCore.Tests;

public class DecoratorTests
{
    [Fact]
    public void Decorator_Composition_AppliesInOrder()
    {
        decimal baseCost = 1000m;
        var cargos = new List<LogiCore.Domain.Cargo.Cargo> { new StandardCargo("Test", 100, 1, 10000) };
        
        IDeliveryCost cost = new BaseDeliveryCost(baseCost);

        cost = new InsuranceDecorator(cost, cargos, 0.05m);

        cost = new ExpressDecorator(cost, 1.5m);

        Assert.Equal(2250m, cost.Total);
    }

    [Fact]
    public void Decorator_OrderMatters_ForMultiplicativeDecorators()
    {
        decimal baseCost = 1000m;

        IDeliveryCost cost1 = new BaseDeliveryCost(baseCost);
        cost1 = new InsuranceDecorator(cost1, new List<LogiCore.Domain.Cargo.Cargo>{ new StandardCargo("T", 1, 1, 10000) }, 0.05m);
        cost1 = new ExpressDecorator(cost1, 1.5m);
        
        Assert.Equal(2250m, cost1.Total);
    }
}