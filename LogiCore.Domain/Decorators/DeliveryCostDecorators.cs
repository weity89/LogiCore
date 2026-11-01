using LogiCore.Domain.Cargo;

namespace LogiCore.Domain.Decorators;

public interface IDeliveryCost
{
    decimal Total { get; }
    string Describe();
}

public class BaseDeliveryCost : IDeliveryCost
{
    private readonly decimal _baseCost;
    public BaseDeliveryCost(decimal baseCost) => _baseCost = baseCost;
    public decimal Total => _baseCost;
    public string Describe() => $"Базовая стоимость: {_baseCost:C}";
}

public abstract class DeliveryCostDecorator : IDeliveryCost
{
    protected readonly IDeliveryCost _wrapped;
    protected DeliveryCostDecorator(IDeliveryCost wrapped) => _wrapped = wrapped;
    
    public abstract decimal Total { get; }
    public abstract string Describe();
}

public class InsuranceDecorator : DeliveryCostDecorator
{
    private readonly decimal _insuranceCost;
    
    public InsuranceDecorator(IDeliveryCost wrapped, IReadOnlyCollection<Cargo.Cargo> cargos, decimal rate = 0.05m) 
        : base(wrapped)
    {
        var totalValue = cargos.Sum(c => c.DeclaredValue);
        _insuranceCost = totalValue * rate;
    }

    public override decimal Total => _wrapped.Total + _insuranceCost;
    public override string Describe() => $"{_wrapped.Describe()} + Страхование ({_insuranceCost:C})";
}

public class ExpressDecorator : DeliveryCostDecorator
{
    private readonly decimal _multiplier;
    
    public ExpressDecorator(IDeliveryCost wrapped, decimal multiplier = 1.5m) : base(wrapped)
    {
        _multiplier = multiplier;
    }

    public override decimal Total => _wrapped.Total * _multiplier;
    public override string Describe() => $"{_wrapped.Describe()} + Срочность (x{_multiplier})";
}

public class FragilePackagingDecorator : DeliveryCostDecorator
{
    private readonly decimal _fixedCost;
    
    public FragilePackagingDecorator(IDeliveryCost wrapped, decimal fixedCost = 500m) : base(wrapped)
    {
        _fixedCost = fixedCost;
    }

    public override decimal Total => _wrapped.Total + _fixedCost;
    public override string Describe() => $"{_wrapped.Describe()} + Упаковка хрупкого ({_fixedCost:C})";
}