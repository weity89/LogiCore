using System.Globalization;

namespace LogiCore.Domain.Common;


public readonly struct RoutePoint : IEquatable<RoutePoint>
{
    public double Latitude { get; }
    public double Longitude { get; }
    public string Name { get; }

    public RoutePoint(double latitude, double longitude, string name)
    {
        if (latitude < -90 || latitude > 90) throw new ArgumentOutOfRangeException(nameof(latitude));
        if (longitude < -180 || longitude > 180) throw new ArgumentOutOfRangeException(nameof(longitude));
        
        Latitude = latitude;
        Longitude = longitude;
        Name = name ?? string.Empty;
    }
    
    public static double operator -(RoutePoint p1, RoutePoint p2)
    {
        const double earthRadiusKm = 6371.0;
        var dLat = ToRadians(p2.Latitude - p1.Latitude);
        var dLon = ToRadians(p2.Longitude - p1.Longitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(p1.Latitude)) * Math.Cos(ToRadians(p2.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }
    
    public static explicit operator string(RoutePoint p) => p.ToString();

    public override string ToString() => $"{Name} [{Latitude.ToString("F4", CultureInfo.InvariantCulture)}; {Longitude.ToString("F4", CultureInfo.InvariantCulture)}]";

    public bool Equals(RoutePoint other) => Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);
    public override bool Equals(object? obj) => obj is RoutePoint other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);
    
    public static bool operator ==(RoutePoint left, RoutePoint right) => left.Equals(right);
    public static bool operator !=(RoutePoint left, RoutePoint right) => !(left == right);

    private static double ToRadians(double angle) => Math.PI * angle / 180.0;
}