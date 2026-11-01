using LogiCore.App.Infrastructure;
using LogiCore.Domain.Abstractions;
using LogiCore.Domain.Cargo;
using LogiCore.Domain.Common;
using LogiCore.Domain.Decorators;
using LogiCore.Domain.Factories;
using LogiCore.Domain.Order;
using LogiCore.Domain.Services;
using LogiCore.Domain.Vehicle;

using CargoEntity = LogiCore.Domain.Cargo.Cargo;

namespace LogiCore.App.Demo;

public class DemoRunner
{
    private readonly DeliveryService _service;
    private readonly List<Vehicle> _fleet;
    private readonly List<Order> _ordersList = new();

    public DemoRunner(DeliveryService service, List<Vehicle> fleet)
    {
        _service = service;
        _fleet = fleet;
    }

    public void Run()
    {
        Console.WriteLine("\n=== 1. Демонстрация фабрик: парк и грузы ===");
        ShowFleetAndCargo();

        Console.WriteLine("\n=== 2. Оформление 5 заказов (один невалидный) ===");
        var orders = CreateDemoOrders();

        Console.WriteLine("\n=== 3. Попытка перегруза (Событие VehicleOverloadAttempt) ===");
        if (orders.TryGetValue("Oversized", out var oversizedOrder))
        {
            TryOverload(oversizedOrder);
        }

        Console.WriteLine("\n=== 4. Подбор ТС и расчет стоимости (Декораторы) ===");
        if (orders.TryGetValue("Fragile", out var fragileOrder))
        {
            CalculateAndAssign(fragileOrder);
        }

        Console.WriteLine("\n=== 5. Жизненный цикл заказа (События) ===");
        if (orders.TryGetValue("Fragile", out var lifecycleOrder))
        {
            RunLifecycle(lifecycleOrder);
        }
        
        Console.WriteLine("\n=== Подготовка данных для отчетов ===");
        if (orders.TryGetValue("Standard", out var standardOrder))
        {
            var truck = _fleet.OfType<Truck>().First();
            standardOrder.Assign(truck, 1000m); // Просто для отчета
            _service.StartDelivery(standardOrder);
            _service.CompleteDelivery(standardOrder);
        }
        
        if (orders.TryGetValue("Perishable", out var perishableOrder))
        {
            var fridge = _fleet.OfType<RefrigeratorTruck>().First();
            perishableOrder.Assign(fridge, 2000m);
            _service.StartDelivery(perishableOrder);
            _service.CompleteDelivery(perishableOrder);
        }
        
        if (orders.TryGetValue("Oversized", out var oversizedOrderToPrepare)) 
        {
            var ship = _fleet.OfType<CargoShip>().First();
            oversizedOrderToPrepare.Assign(ship, 5000m);
            _service.StartDelivery(oversizedOrderToPrepare);
            _service.CompleteDelivery(oversizedOrderToPrepare);
        }
        
        Console.WriteLine("\n=== 6. LINQ-отчеты (T8) ===");
        RunReports();

        Console.WriteLine("\n=== 7. Сериализация и восстановление (T9) ===");
        RunSerialization(orders.Values.ToList());
    }

    private void ShowFleetAndCargo()
    {
        Console.WriteLine($"В парке {_fleet.Count} ТС:");
        foreach (var v in _fleet) Console.WriteLine($"  - {v}");
        
        Console.WriteLine("Создаем тестовые грузы...");
        var cargos = new List<CargoEntity>
        {
            CargoFactory.Create("standard", "Коробки", 100, 2, 1000),
            CargoFactory.Create("fragile", "Стекло", 50, 1, 5000),
            CargoFactory.Create("perishable", "Молоко", 200, 5, 2000),
            CargoFactory.Create("dangerous", "Кислота", 150, 3, 10000),
            CargoFactory.Create("oversized", "Труба", 5000, 30, 50000)
        };
        foreach (var c in cargos) Console.WriteLine($"  - {c}");
    }

    private Dictionary<string, Order> CreateDemoOrders()
    {
        var route = DemoData.CreateMoscowSpbRoute();
        var customer = new Customer("Иван Иванов", "123");
        var orders = new Dictionary<string, Order>();
        
        
        var standardCargo = new List<CargoEntity> { CargoFactory.Create("standard", "Одежда", 500, 10, 5000) };
        orders["Standard"] = _service.CreateOrder(customer, route, standardCargo);
        _ordersList.Add(orders["Standard"]);

        var fragileCargo = new List<CargoEntity> { CargoFactory.Create("fragile", "Вазы", 100, 2, 20000) };
        orders["Fragile"] = _service.CreateOrder(customer, route, fragileCargo);
        _ordersList.Add(orders["Fragile"]);

        var oversizedCargo = new List<CargoEntity> { CargoFactory.Create("oversized", "Станок", 15000, 100, 100000) };
        orders["Oversized"] = _service.CreateOrder(customer, route, oversizedCargo);
        _ordersList.Add(orders["Oversized"]);

        var perishableCargo = new List<CargoEntity> { CargoFactory.Create("perishable", "Рыба", 200, 5, 10000) };
        orders["Perishable"] = _service.CreateOrder(customer, route, perishableCargo);
        _ordersList.Add(orders["Perishable"]);
        
        
        var invalidCargo = new List<CargoEntity>
        {
            CargoFactory.Create("dangerous", "Бензин", 100, 2, 5000),
            CargoFactory.Create("perishable", "Мясо", 100, 2, 5000)
        };
        try
        {
            _service.CreateOrder(customer, route, invalidCargo);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ПЕРЕХВАЧЕНО ИСКЛЮЧЕНИЕ]: {ex.Message}");
            Console.ResetColor();
        }

        return orders;
    }

    private void TryOverload(Order order)
    {
        var drone = _fleet.OfType<DroneCourier>().FirstOrDefault();
        if (drone != null)
        {
            Console.WriteLine($"Пытаемся назначить дрон для негабарита...");
            try
            {
                _service.ForceAssignVehicle(order, drone);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ожидаемая ошибка: {ex.Message}");
            }
        }
    }

    private void CalculateAndAssign(Order order)
    {
        var truck = _fleet.OfType<Truck>().First();
        
        decimal baseCost = truck.CalculateDeliveryCost(order.Route, order.Cargos);
        Console.WriteLine($"Базовая стоимость: {baseCost:C}");

        IDeliveryCost cost = new BaseDeliveryCost(baseCost);
        cost = new InsuranceDecorator(cost, order.Cargos, 0.05m);
        Console.WriteLine($"После страховки: {cost.Total:C} ({cost.Describe()})");

        cost = new ExpressDecorator(cost, 1.5m);
        Console.WriteLine($"После срочности: {cost.Total:C} ({cost.Describe()})");

        if (order.Cargos.Any(c => c is FragileCargo))
        {
            cost = new FragilePackagingDecorator(cost, 500m);
            Console.WriteLine($"После упаковки: {cost.Total:C} ({cost.Describe()})");
        }

        order.Assign(truck, cost.Total);
    }

    private void RunLifecycle(Order order)
    {
        _service.StartDelivery(order);
        Thread.Sleep(500);
        _service.CompleteDelivery(order);
    }

    private void RunReports()
    {
        var reportGenerator = new ReportGenerator(_ordersList, _fleet);
        reportGenerator.GenerateAll();
    }

    private void RunSerialization(List<Order> orders)
    {
        Console.WriteLine("Сериализация");
    }
}