using LogiCore.App.Demo;
using LogiCore.App.Infrastructure;

Console.WriteLine("=== LogiCore System ===");

var (service, fleet, customer) = DemoData.Setup();
var wrapper = new DeliveryServiceWrapper(service);

var consoleNotifier = new ConsoleNotifier();
consoleNotifier.Subscribe(wrapper);

var fileLogger = new FileLogger();
fileLogger.Subscribe(wrapper);

var runner = new DemoRunner(service, fleet);
runner.Run();

Console.WriteLine("\nДемо завершено. Нажмите любую клавишу...");
Console.ReadKey();