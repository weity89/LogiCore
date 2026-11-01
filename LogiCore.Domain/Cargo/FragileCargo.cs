namespace LogiCore.Domain.Cargo;

public class FragileCargo : Cargo, IInsurable
{
    public double RiskCoefficient { get; }

    public FragileCargo(string description, double weightKg, double volumeM3, decimal declaredValue, double riskCoefficient)
        : base(description, weightKg, volumeM3, declaredValue)
    {
        if (riskCoefficient <= 0) throw new ArgumentException("Коэффициент риска должен быть больше нуля.");
        RiskCoefficient = riskCoefficient;
    }
}