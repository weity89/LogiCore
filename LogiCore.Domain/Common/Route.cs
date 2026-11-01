using LogiCore.Domain.Vehicle;

namespace LogiCore.Domain.Common;

public class Route
{
    private readonly List<RoutePoint> _points;
    
    public IReadOnlyList<RoutePoint> Points => _points.AsReadOnly();
    
    public double DistanceKm
    {
        get
        {
            if (_points.Count < 2) return 0;
            
            double totalDistance = 0;
            for (int i = 0; i < _points.Count - 1; i++)
            {
                totalDistance += _points[i + 1] - _points[i];
            }
            return totalDistance;
        }
    }

    public Route(IEnumerable<RoutePoint> points)
    {
        _points = points?.ToList() ?? throw new ArgumentNullException(nameof(points));
        if (_points.Count == 0)
            throw new Exceptions.RouteNotFoundException("Маршрут должен содержать хотя бы одну точку.");
    }
    
    public TimeSpan EstimateTime(Vehicle.Vehicle v)
    {
        if (v.AverageSpeedKmH <= 0) return TimeSpan.Zero;
        
        double hours = DistanceKm / v.AverageSpeedKmH;
        return TimeSpan.FromHours(hours);
    }
}