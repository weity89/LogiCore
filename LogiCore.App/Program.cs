using LogiCore.App;
using LogiCore.App.Demo;
using LogiCore.App.Infrastructure;
using LogiCore.Domain.Order;
using LogiCore.Domain.Vehicle;

Console.WriteLine("=== LogiCore System ===");

var (service, fleet, customer) = DemoData.Setup();
var wrapper = new DeliveryServiceWrapper(service);
var consoleNotifier = new ConsoleNotifier();
consoleNotifier.Subscribe(wrapper);

while (true)
{
    Console.WriteLine("\n=== ГЛАВНОЕ МЕНЮ ===");
    Console.WriteLine("1. Запустить демо-сценарий");
    Console.WriteLine("2. Открыть интерактивное меню");
    Console.WriteLine("0. Выход");
    Console.Write("Выбор: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            var runner = new DemoRunner(service, fleet);
            runner.Run();
            break;
        case "2":
            var menu = new Menu(service, fleet);
            menu.Run();
            break;
        case "0":
            Console.WriteLine("Выход из системы...");
            return;
        default:
            Console.WriteLine("Неверный ввод. Попробуйте снова.");
            break;
    }

    Console.WriteLine("\nНажмите любую клавишу для возврата в главное меню...");
    Console.ReadKey();
}