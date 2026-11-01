using LogiCore.Domain.Cargo;
using LogiCore.Domain.Common;

namespace LogiCore.Domain.Abstractions;

public interface ITariffStrategy
{
    string Name { get; }
    decimal Calculate(decimal baseCost, Route route, IReadOnlyCollection<Cargo.Cargo> cargo);
}