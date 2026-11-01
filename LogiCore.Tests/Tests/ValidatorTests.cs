using LogiCore.Domain.Cargo;
using LogiCore.Domain.Services;
using Xunit;

namespace LogiCore.Tests;

public class ValidatorTests
{
    private readonly CargoCompatibilityValidator _validator = new();

    [Fact]
    public void Validate_DangerousAndPerishable_ReturnsFailure()
    {
        var cargos = new List<LogiCore.Domain.Cargo.Cargo>
        {
            new DangerousCargo("Benzin", 100, 2, 5000, 3),
            new PerishableCargo("Milk", 100, 2, 2000, DateTime.Now.AddDays(1), 0)
        };
        
        var result = _validator.Validate(cargos);
        
        Assert.False(result.IsValid);
        Assert.Contains("Опасные грузы нельзя", result.Errors[0]);
    }

    [Fact]
    public void Validate_ExpiredCargo_ReturnsFailure()
    {
        var cargos = new List<LogiCore.Domain.Cargo.Cargo>
        {
            new PerishableCargo("Milk", 100, 2, 2000, DateTime.Now.AddDays(-1), 0)
        };
        
        var result = _validator.Validate(cargos);
        
        Assert.False(result.IsValid);
        Assert.Contains("просрочен", result.Errors[0]);
    }

    [Fact]
    public void Validate_ValidCargos_ReturnsSuccess()
    {
        var cargos = new List<LogiCore.Domain.Cargo.Cargo>
        {
            new StandardCargo("Box", 100, 2, 1000),
            new FragileCargo("Glass", 50, 1, 5000, 1.5)
        };
        
        var result = _validator.Validate(cargos);
        
        Assert.True(result.IsValid);
    }
}