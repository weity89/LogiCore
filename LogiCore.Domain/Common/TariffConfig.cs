namespace LogiCore.Domain.Common;

public sealed class TariffConfig
{
    private static readonly Lazy<TariffConfig> _instance = new(() => new TariffConfig());
    public static TariffConfig Instance => _instance.Value;

    public decimal InsuranceRate { get; set; } = 0.05m;
    public decimal ExpressMultiplier { get; set; } = 1.5m;
    public decimal FragilePackagingCost { get; set; } = 500m;

    private TariffConfig() { }
}