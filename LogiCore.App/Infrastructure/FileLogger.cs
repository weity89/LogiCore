using LogiCore.Domain.Events;

namespace LogiCore.App.Infrastructure;

public class FileLogger
{
    private readonly string _filePath = "delivery_log.txt";

    public void Subscribe(DeliveryServiceWrapper service)
    {
        service.OrderStatusChanged += LogStatusChange;
    }

    private void LogStatusChange(object? sender, OrderStatusChangedEventArgs e)
    {
        File.AppendAllText(_filePath, $"[{DateTime.Now}] Заказ {e.Order.Id} статус: {e.NewStatus}\n");
    }
}