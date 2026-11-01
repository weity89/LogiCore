using System.Text.Json;
using LogiCore.Domain.Common;
using LogiCore.Domain.Factories;
using LogiCore.Domain.Order;
using LogiCore.Domain.Vehicle;

namespace LogiCore.App.Infrastructure;

public class StateManager
{
    private const string FilePath = "logistics_state.json";

    public void SaveState(List<Vehicle> fleet, List<Order> orders)
    {
        var fleetDto = new List<VehicleDto>();
        foreach (var v in fleet)
        {
            fleetDto.Add(new VehicleDto
            {
                Type = v.GetType().Name,
                RegistrationNumber = v.RegistrationNumber,
                MaxLoadKg = v.MaxLoadKg,
                MaxVolumeM3 = v.MaxVolumeM3,
                AverageSpeedKmH = v.AverageSpeedKmH,
                BaseRatePerKm = v.BaseRatePerKm,
                State = v.State.ToString()
            });
        }

        var ordersDto = new List<OrderDto>();
        foreach (var o in orders)
        {
            var cargosDto = new List<CargoDto>();
            foreach (var c in o.Cargos)
            {
                cargosDto.Add(new CargoDto
                {
                    Type = c.GetType().Name,
                    Description = c.Description,
                    WeightKg = c.WeightKg,
                    VolumeM3 = c.VolumeM3,
                    DeclaredValue = c.DeclaredValue
                });
            }

            ordersDto.Add(new OrderDto
            {
                Id = o.Id,
                CustomerName = o.Customer.Name,
                Status = o.Status.ToString(),
                TotalCost = o.TotalCost,
                Cargos = cargosDto
            });
        }

        var state = new { Fleet = fleetDto, Orders = ordersDto };
        
        try
        {
            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
            Console.WriteLine($"[STATE] Состояние сохранено в {FilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Ошибка сохранения: {ex.Message}");
        }
    }

    public (List<Vehicle> fleet, List<Order> orders)? LoadState()
    {
        if (!File.Exists(FilePath))
        {
            Console.WriteLine("[STATE] Файл состояния не найден.");
            return null;
        }

        try
        {
            var json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json)) throw new JsonException("Файл пуст.");

            using var doc = JsonDocument.Parse(json);
            var fleetDto = JsonSerializer.Deserialize<List<VehicleDto>>(doc.RootElement.GetProperty("Fleet").GetRawText());
            var ordersDto = JsonSerializer.Deserialize<List<OrderDto>>(doc.RootElement.GetProperty("Orders").GetRawText());

            if (fleetDto == null || ordersDto == null) throw new JsonException("Не удалось прочитать данные.");

            var fleet = new List<Vehicle>();
            foreach (var dto in fleetDto)
            {
                string factoryType = dto.Type switch
                {
                    "Truck" => "truck",
                    "RefrigeratorTruck" => "refrigerator",
                    "CargoPlane" => "plane",
                    "CargoShip" => "ship",
                    "DroneCourier" => "drone",
                    _ => throw new Exception($"Неизвестный тип ТС: {dto.Type}")
                };

                var vehicle = VehicleFactory.Create(factoryType, dto.RegistrationNumber, dto.MaxLoadKg, dto.MaxVolumeM3, dto.AverageSpeedKmH, dto.BaseRatePerKm);
                vehicle.State = Enum.Parse<VehicleState>(dto.State);
                fleet.Add(vehicle);
            }

            var orders = new List<Order>();
            var customer = new Customer("Иван Иванов", "123");
            var route = new Route(new[] { new RoutePoint(55.75, 37.61, "M"), new RoutePoint(59.93, 30.33, "S") });

            foreach (var dto in ordersDto)
            {
                var cargos = new List<Domain.Cargo.Cargo>();
                foreach (var c in dto.Cargos)
                {
                    string factoryType = c.Type switch
                    {
                        "StandardCargo" => "standard",
                        "PerishableCargo" => "perishable",
                        "FragileCargo" => "fragile",
                        "DangerousCargo" => "dangerous",
                        "OversizedCargo" => "oversized",
                        _ => throw new Exception($"Неизвестный тип груза: {c.Type}")
                    };
                    cargos.Add(CargoFactory.Create(factoryType, c.Description, c.WeightKg, c.VolumeM3, c.DeclaredValue));
                }

                var order = new Order(customer, route, cargos);
                orders.Add(order);
            }

            Console.WriteLine("[STATE] Состояние успешно загружено.");
            return (fleet, orders);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Ошибка загрузки: {ex.Message}");
            return null;
        }
    }
}