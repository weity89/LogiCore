using LogiCore.Domain.Abstractions;
using LogiCore.Domain.Cargo;
using LogiCore.Domain.Common;

namespace LogiCore.Domain.Services;

public class CargoCompatibilityValidator : IValidator<IEnumerable<Cargo.Cargo>>
{
    public ValidationResult Validate(IEnumerable<Cargo.Cargo> cargos)
    {
        var result = ValidationResult.Success();
        var cargoList = cargos.ToList();
        
        if (cargoList.Any(c => c is DangerousCargo) && cargoList.Any(c => c is PerishableCargo))
        {
            result = ValidationResult.Failure("Опасные грузы нельзя перевозить вместе со скоропортящимися.");
            return result;
        }
        
        foreach (var cargo in cargoList.OfType<PerishableCargo>())
        {
            if (cargo.ExpirationDate < DateTime.Now)
            {
                result = ValidationResult.Failure($"Груз '{cargo.Description}' просрочен (срок годности: {cargo.ExpirationDate}).");
                return result;
            }
        }
        
        return result;
    }
}