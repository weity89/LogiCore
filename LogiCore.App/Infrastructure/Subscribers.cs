using LogiCore.Domain.Events;
using LogiCore.Domain.Order;

namespace LogiCore.App.Infrastructure;

public class ConsoleNotifier
{
    public void Subscribe(DeliveryServiceWrapper service)
    {
        service.OrderCreated += OnOrderCreated;
        service.OrderStatusChanged += OnOrderStatusChanged;
        service.VehicleOverloadAttempt += OnVehicleOverloadAttempt;
        service.DeliveryCompleted += OnDeliveryCompleted;
    }

    public void Unsubscribe(DeliveryServiceWrapper service)
    {
        service.OrderCreated -= OnOrderCreated;
        service.OrderStatusChanged -= OnOrderStatusChanged;
        service.VehicleOverloadAttempt -= OnVehicleOverloadAttempt;
        service.DeliveryCompleted -= OnDeliveryCompleted;
    }

    private void OnOrderCreated(object? sender, OrderCreatedEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[EVENT] Создан заказ {e.Order.Id.ToString()[..8]} для клиента {e.Order.Customer.Name}");
        Console.ResetColor();
    }

    private void OnOrderStatusChanged(object? sender, OrderStatusChangedEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[EVENT] Заказ {e.Order.Id.ToString()[..8]}: статус {e.OldStatus} -> {e.NewStatus}");
        Console.ResetColor();
    }

    private void OnVehicleOverloadAttempt(object? sender, VehicleOverloadAttemptEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[EVENT] ПЕРЕГРУЗ! Попытка загрузить {e.CargoWeightKg} кг в {e.Vehicle.GetType().Name} (макс {e.Vehicle.MaxLoadKg} кг)");
        Console.ResetColor();
    }

    private void OnDeliveryCompleted(object? sender, DeliveryCompletedEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[EVENT] ДОСТАВКА ЗАВЕРШЕНА! Заказ {e.Order.Id.ToString()[..8]}. Итоговая стоимость: {e.Order.TotalCost:C}");
        Console.ResetColor();
    }
}
public class DeliveryServiceWrapper
{
    private readonly LogiCore.Domain.Services.DeliveryService _service;

    public DeliveryServiceWrapper(LogiCore.Domain.Services.DeliveryService service) => _service = service;

    public event EventHandler<OrderCreatedEventArgs>? OrderCreated
    {
        add => _service.OrderCreated += value;
        remove => _service.OrderCreated -= value;
    }

    public event EventHandler<OrderStatusChangedEventArgs>? OrderStatusChanged
    {
        add => _service.OrderStatusChanged += value;
        remove => _service.OrderStatusChanged -= value;
    }

    public event EventHandler<VehicleOverloadAttemptEventArgs>? VehicleOverloadAttempt
    {
        add => _service.VehicleOverloadAttempt += value;
        remove => _service.VehicleOverloadAttempt -= value;
    }

    public event EventHandler<DeliveryCompletedEventArgs>? DeliveryCompleted
    {
        add => _service.DeliveryCompleted += value;
        remove => _service.DeliveryCompleted -= value;
    }
}