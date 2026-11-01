using LogiCore.Domain.Order;
using LogiCore.Domain.Vehicle;

namespace LogiCore.Domain.Events;

public delegate void OrderStatusChangedHandler(object sender, OrderStatusChangedEventArgs e);

public class OrderCreatedEventArgs : EventArgs
{
    public Order.Order Order { get; }
    public OrderCreatedEventArgs(Order.Order order) => Order = order;
}

public class OrderStatusChangedEventArgs : EventArgs
{
    public Order.Order Order { get; }
    public OrderStatus OldStatus { get; }
    public OrderStatus NewStatus { get; }
    public OrderStatusChangedEventArgs(Order.Order order, OrderStatus oldStatus, OrderStatus newStatus)
    {
        Order = order;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}

public class VehicleOverloadAttemptEventArgs : EventArgs
{
    public Order.Order Order { get; }
    public Vehicle.Vehicle Vehicle { get; }
    public double CargoWeightKg { get; }
    public VehicleOverloadAttemptEventArgs(Order.Order order, Vehicle.Vehicle vehicle, double cargoWeightKg)
    {
        Order = order;
        Vehicle = vehicle;
        CargoWeightKg = cargoWeightKg;
    }
}

public class DeliveryCompletedEventArgs : EventArgs
{
    public Order.Order Order { get; }
    public DeliveryCompletedEventArgs(Order.Order order) => Order = order;
}