// LogiCore.Domain/Common/Route.cs
namespace LogiCore.Domain.Common;

public class Route
{
    public IReadOnlyList<RoutePoint> Points { get; }
    public double DistanceKm => /* Вычисление будет позже */ 0; 

    public Route(IEnumerable<RoutePoint> points)
    {
        Points = points.ToList().AsReadOnly();
    }
}