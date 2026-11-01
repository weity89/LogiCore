using LogiCore.Domain.Cargo;
using LogiCore.Domain.Order;
using LogiCore.Domain.Vehicle;

namespace LogiCore.App.Demo;

public class ReportGenerator
{
    private readonly List<Order> _orders;
    private readonly List<Vehicle> _fleet;

    public ReportGenerator(List<Order> orders, List<Vehicle> fleet)
    {
        _orders = orders;
        _fleet = fleet;
    }

    public void GenerateAll()
    {
        Console.WriteLine("\n--- ОТЧЕТ 1: Топ-3 ТС по выручке ---");
        ReportTop3Vehicles();

        Console.WriteLine("\n--- ОТЧЕТ 2: Заказы по статусам (кол-во и сумма) ---");
        ReportOrdersByStatus();

        Console.WriteLine("\n--- ОТЧЕТ 3: Средняя загрузка ТС по типам (%) ---");
        ReportAverageLoadByVehicleType();

        Console.WriteLine("\n--- ОТЧЕТ 4: Клиенты с суммой заказов выше 5000 ---");
        ReportRichCustomers(threshold: 5000m);

        Console.WriteLine("\n--- ОТЧЕТ 5: Join «груз -> заказ -> клиент» (синтаксис запросов) ---");
        ReportCargoOrderCustomerJoin();

        Console.WriteLine("\n--- ОТЧЕТ 6: Словарь «класс опасности -> количество» ---");
        ReportHazardClassDictionary();
    }

    //1
    private void ReportTop3Vehicles()
    {
        var top3 = _orders
            .Where(o => o.AssignedVehicle != null && o.Status == OrderStatus.Delivered)
            .GroupBy(o => o.AssignedVehicle!)
            .Select(g => new 
            { 
                Vehicle = g.Key, 
                Revenue = g.Sum(o => o.TotalCost) 
            })
            .OrderByDescending(x => x.Revenue)
            .Take(3)
            .ToList();

        if (!top3.Any())
        {
            Console.WriteLine("Нет доставленных заказов.");
            return;
        }

        foreach (var item in top3)
        {
            Console.WriteLine($"{item.Vehicle.GetType().Name} [{item.Vehicle.RegistrationNumber}]: {item.Revenue:C}");
        }
    }
    //2
    private void ReportOrdersByStatus()
    {
        var byStatus = _orders
            .GroupBy(o => o.Status)
            .Select(g => new
            {
                Status = g.Key,
                Count = g.Count(),
                TotalCost = g.Sum(o => o.TotalCost)
            })
            .OrderBy(x => x.Status);

        foreach (var item in byStatus)
        {
            Console.WriteLine($"Статус: {item.Status,-12} | Кол-во: {item.Count} | Сумма: {item.TotalCost:C}");
        }
    }
    //3
    private void ReportAverageLoadByVehicleType()
    {
        var byType = _orders
            .Where(o => o.AssignedVehicle != null && o.Status != OrderStatus.Cancelled)
            .GroupBy(o => o.AssignedVehicle!.GetType().Name)
            .Select(g => new
            {
                VehicleType = g.Key,
                AverageLoadPercent = g.Average(o => 
                {
                    var totalWeight = o.Cargos.Sum(c => c.WeightKg);
                    return (totalWeight / o.AssignedVehicle!.MaxLoadKg) * 100;
                })
            });

        foreach (var item in byType)
        {
            Console.WriteLine($"{item.VehicleType,-20}: средняя загрузка {item.AverageLoadPercent:F1}%");
        }
    }
    //4
    private void ReportRichCustomers(decimal threshold)
    {
        var richCustomers = _orders
            .Where(o => o.Status != OrderStatus.Cancelled)
            .GroupBy(o => o.Customer)
            .Select(g => new
            {
                Customer = g.Key,
                TotalSpent = g.Sum(o => o.TotalCost)
            })
            .Where(x => x.TotalSpent > threshold)
            .OrderByDescending(x => x.TotalSpent);

        foreach (var item in richCustomers)
        {
            Console.WriteLine($"{item.Customer.Name}: {item.TotalSpent:C}");
        }
    }
    //5
    private void ReportCargoOrderCustomerJoin()
    {
        var query = from order in _orders
                    from cargo in order.Cargos
                    where order.Status != OrderStatus.Cancelled
                    select new
                    {
                        CustomerName = order.Customer.Name,
                        CargoDescription = cargo.Description,
                        CargoWeight = cargo.WeightKg
                    };

        var result = query.ToList();

        foreach (var item in result)
        {
            Console.WriteLine($"{item.CustomerName} везет '{item.CargoDescription}' ({item.CargoWeight} кг)");
        }
    }
    //6
    private void ReportHazardClassDictionary()
    {
        var hazardCargos = _orders
            .SelectMany(o => o.Cargos)
            .OfType<DangerousCargo>();

        var dictionary = hazardCargos
            .GroupBy(c => c.HazardClass)
            .ToDictionary(
                g => g.Key, 
                g => g.Count()
            );

        if (!dictionary.Any())
        {
            Console.WriteLine("Опасных грузов нет.");
            return;
        }

        foreach (var kvp in dictionary.OrderBy(k => k.Key))
        {
            Console.WriteLine($"Класс опасности {kvp.Key}: {kvp.Value} шт.");
        }
    }
}