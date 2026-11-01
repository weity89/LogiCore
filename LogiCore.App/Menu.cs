using LogiCore.App.Demo;
using LogiCore.App.Infrastructure;
using LogiCore.Domain.Common;
using LogiCore.Domain.Factories;
using LogiCore.Domain.Order;
using LogiCore.Domain.Services;
using LogiCore.Domain.Vehicle;

namespace LogiCore.App;

public class Menu
{
    private readonly DeliveryService _service;
    private readonly List<Vehicle> _fleet;
    private readonly List<Order> _orders;
    private readonly Customer _currentCustomer;

    public Menu(DeliveryService service, List<Vehicle> fleet, List<Order> orders, Customer customer)
    {
        _service = service;
        _fleet = fleet;
        _orders = orders;
        _currentCustomer = customer;
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine("\n=== ИНТЕРАКТИВНОЕ МЕНЮ ===");
            Console.WriteLine("1. Создать новый заказ");
            Console.WriteLine("2. Показать отчеты");
            Console.WriteLine("3. Сохранить состояние");
            Console.WriteLine("4. Загрузить состояние");
            Console.WriteLine("0. Назад в главное меню");
            Console.Write("Выбор: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    CreateOrderInteractive();
                    break;
                case "2":
                    ShowReports();
                    break;
                case "3":
                    new StateManager().SaveState(_fleet, _orders);
                    break;
                case "4":
                    LoadStateInteractive();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Неверный ввод.");
                    break;
            }
        }
    }

    private void CreateOrderInteractive()
    {
        Console.WriteLine("\n--- Создание заказа ---");
        
        var route = DemoData.CreateMoscowSpbRoute();
        
        var cargos = new List<LogiCore.Domain.Cargo.Cargo>();
        while (true)
        {
            Console.Write("Тип груза (standard/fragile/perishable/dangerous/oversized) или 'done' для завершения: ");
            var type = Console.ReadLine();
            if (type?.ToLower() == "done") break;

            try
            {
                Console.Write("Описание: ");
                var desc = Console.ReadLine() ?? "Груз";
                Console.Write("Вес (кг): ");
                var weight = double.Parse(Console.ReadLine() ?? "1");
                Console.Write("Объем (м3): ");
                var volume = double.Parse(Console.ReadLine() ?? "1");
                Console.Write("Объявленная стоимость: ");
                var value = decimal.Parse(Console.ReadLine() ?? "1000");

                var cargo = CargoFactory.Create(type ?? "standard", desc, weight, volume, value);
                cargos.Add(cargo);
                Console.WriteLine($"Груз '{desc}' добавлен.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        if (cargos.Count == 0)
        {
            Console.WriteLine("Заказ отменен (нет грузов).");
            return;
        }

        try
        {
            var order = _service.CreateOrder(_currentCustomer, route, cargos);
            _orders.Add(order);
            Console.WriteLine($"[SUCCESS] Заказ создан! ID: {order.Id.ToString()[..8]}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Не удалось создать заказ: {ex.Message}");
        }
    }

    private void ShowReports()
    {
        if (_orders.Count == 0)
        {
            Console.WriteLine("Нет заказов для отчета. Сначала создайте заказ или запустите демо.");
            return;
        }

        var reportGenerator = new ReportGenerator(_orders, _fleet);
        reportGenerator.GenerateAll();
    }

    private void LoadStateInteractive()
    {
        var loaded = new StateManager().LoadState();
        if (loaded.HasValue)
        {
            var (newFleet, newOrders) = loaded.Value;
            
            _fleet.Clear();
            _fleet.AddRange(newFleet);
            _orders.Clear();
            _orders.AddRange(newOrders);
            
            Console.WriteLine($"[STATE] Загружено {newFleet.Count} ТС и {newOrders.Count} заказов.");
        }
    }
}