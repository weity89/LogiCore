namespace LogiCore.App.Infrastructure;

public class VehicleDto
{
    public string Type { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public double MaxLoadKg { get; set; }
    public double MaxVolumeM3 { get; set; }
    public double AverageSpeedKmH { get; set; }
    public decimal BaseRatePerKm { get; set; }
    public string State { get; set; } = string.Empty;
}

public class CargoDto
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double WeightKg { get; set; }
    public double VolumeM3 { get; set; }
    public decimal DeclaredValue { get; set; }
}

public class OrderDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalCost { get; set; }
    public List<CargoDto> Cargos { get; set; } = new();
}