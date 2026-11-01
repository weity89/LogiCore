using LogiCore.App.Demo;
using LogiCore.App.Infrastructure;

Console.WriteLine("=== LogiCore System Initialization ===");

var (service, fleet, customer) = DemoData.Setup();
var wrapper = new DeliveryServiceWrapper(service);

var consoleNotifier = new ConsoleNotifier();
consoleNotifier.Subscribe(wrapper);

var fileLogger = new FileLogger();
fileLogger.Subscribe(wrapper);

Console.WriteLine($"Клиент: {customer.Name}, Парк: {fleet.Count} ТС");

Console.WriteLine("Готово к демо. Нажмите любую клавишу...");
Console.ReadKey();