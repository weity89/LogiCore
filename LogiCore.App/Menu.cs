using LogiCore.App.Demo;
using LogiCore.App.Infrastructure;
using LogiCore.Domain.Services;

namespace LogiCore.App;

public class Menu
{
    private readonly DeliveryService _service;
    private readonly List<Domain.Vehicle.Vehicle> _fleet;

    public Menu(DeliveryService service, List<Domain.Vehicle.Vehicle> fleet)
    {
        _service = service;
        _fleet = fleet;
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine("\n=== ГЛАВНОЕ МЕНЮ ===");
            Console.WriteLine("1. Создать заказ");
            Console.WriteLine("2. Показать отчеты");
            Console.WriteLine("3. Сохранить состояние");
            Console.WriteLine("4. Загрузить состояние");
            Console.WriteLine("0. Выход");
            Console.Write("Выбор: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.WriteLine("зашлуш.");
                    break;
                case "2":
                    var reportGen = new ReportGenerator(new List<Domain.Order.Order>(), _fleet);
                    Console.WriteLine("Нет данных для отчета (используйте демо).");
                    break;
                case "3":
                    var stateManager = new StateManager();
                    stateManager.SaveState(_fleet, new List<Domain.Order.Order>());
                    break;
                case "4":
                    var loaded = new StateManager().LoadState();
                    if (loaded.HasValue) Console.WriteLine("Загружено.");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Неверный ввод.");
                    break;
            }
        }
    }
}