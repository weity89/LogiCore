using LogiCore.Domain.Common;
using LogiCore.Domain.Factories;
using LogiCore.Domain.Order;
using LogiCore.Domain.Services;
using LogiCore.Domain.Vehicle;

namespace LogiCore.App.Demo;

public static class DemoData
{
    public static (DeliveryService service, List<Vehicle> fleet, Customer customer) Setup()
    {
        var fleet = new List<Vehicle>
        {
            VehicleFactory.Create("truck", "TRK001", 10000, 50, 80, 10),
            VehicleFactory.Create("refrigerator", "REF002", 5000, 30, 70, 15),
            VehicleFactory.Create("plane", "PLN003", 20000, 100, 800, 50),
            VehicleFactory.Create("ship", "SHP004", 50000, 500, 30, 5),
            VehicleFactory.Create("drone", "DRN005", 10, 1, 60, 2),
            VehicleFactory.Create("truck", "TRK006", 8000, 40, 80, 9)
        };

        var validator = new CargoCompatibilityValidator();

        var service = new DeliveryService(fleet, validator);

        var customer = new Customer("Иван Иванов", "+7-977-777-77-77");

        return (service, fleet, customer);
    }

    public static Route CreateMoscowSpbRoute()
    {
        var moscow = new RoutePoint(55.7558, 37.6173, "Москва");
        var spb = new RoutePoint(59.9343, 30.3351, "Санкт-Петербург");
        return new Route(new[] { moscow, spb });
    }
}