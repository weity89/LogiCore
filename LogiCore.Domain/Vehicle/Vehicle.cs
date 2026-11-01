using System.Text.RegularExpressions;
using LogiCore.Domain.Abstractions;
using LogiCore.Domain.Common;
using LogiCore.Domain.Cargo;

namespace LogiCore.Domain.Vehicle;

public enum VehicleState
{
    Free,
    InTransit,
    UnderMaintenance
}

public abstract class Vehicle : IEntity
{
    public Guid Id { get; }
    
    private string _registrationNumber = string.Empty;
    public string RegistrationNumber 
    { 
        get => _registrationNumber; 
        init 
        {
            if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, @"^[A-Z0-9]{5,10}$"))
                throw new ArgumentException("Неверный формат регистрационного номера.");
            _registrationNumber = value;
        } 
    }

    public double MaxLoadKg { get; }
    public double MaxVolumeM3 { get; }
    public double AverageSpeedKmH { get; }
    public decimal BaseRatePerKm { get; }
    public VehicleState State { get; set; } = VehicleState.Free;

    protected Vehicle(string registrationNumber, double maxLoadKg, double maxVolumeM3, double averageSpeedKmH, decimal baseRatePerKm)
    {
        Id = Guid.NewGuid();
        RegistrationNumber = registrationNumber;
        MaxLoadKg = maxLoadKg;
        MaxVolumeM3 = maxVolumeM3;
        AverageSpeedKmH = averageSpeedKmH;
        BaseRatePerKm = baseRatePerKm;
    }
    
    public abstract decimal CalculateDeliveryCost(Route route, IReadOnlyCollection<Cargo.Cargo> cargo);
    
    public virtual bool CanCarry(Cargo.Cargo cargo)
    {
        if (State != VehicleState.Free) return false;
        if (cargo.WeightKg > MaxLoadKg) return false;
        if (cargo.VolumeM3 > MaxVolumeM3) return false;
        return true;
    }

    public override string ToString() => $"{GetType().Name} [{RegistrationNumber}]";
    
    public override bool Equals(object? obj) => obj is Vehicle other && Id == other.Id;
    public override int GetHashCode() => Id.GetHashCode();
}