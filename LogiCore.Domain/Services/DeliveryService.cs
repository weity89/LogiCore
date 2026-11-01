using LogiCore.Domain.Abstractions;
using LogiCore.Domain.Decorators;
using LogiCore.Domain.Events;
using LogiCore.Domain.Exceptions;
using LogiCore.Domain.Order;
using LogiCore.Domain.Vehicle;

namespace LogiCore.Domain.Services;

public class DeliveryService
{
    private readonly List<Vehicle.Vehicle> _fleet;
    private readonly IValidator<IEnumerable<Cargo.Cargo>> _compatibilityValidator;

    public event EventHandler<OrderCreatedEventArgs>? OrderCreated;
    public event EventHandler<OrderStatusChangedEventArgs>? OrderStatusChanged;
    public event EventHandler<VehicleOverloadAttemptEventArgs>? VehicleOverloadAttempt;
    public event EventHandler<DeliveryCompletedEventArgs>? DeliveryCompleted;

    public DeliveryService(IEnumerable<Vehicle.Vehicle> fleet, IValidator<IEnumerable<Cargo.Cargo>> validator)
    {
        _fleet = fleet.ToList();
        _compatibilityValidator = validator;
    }

    public Order.Order CreateOrder(Customer customer, Common.Route route, IEnumerable<Cargo.Cargo> cargos)
    {
        var validation = _compatibilityValidator.Validate(cargos);
        if (!validation.IsValid)
        {
            throw new IncompatibleCargoException(string.Join("; ", validation.Errors));
        }

        var order = new Order.Order(customer, route, cargos);
        customer.AddOrderToHistory(order);

        OrderCreated?.Invoke(this, new OrderCreatedEventArgs(order));
        return order;
    }

    public void AssignVehicleToOrder(Order.Order order, ITariffStrategy tariffStrategy)
    {
        var suitableVehicle = _fleet
            .Where(v => v.State == VehicleState.Free)
            .Where(v => v.CanCarry(order.Cargos.FirstOrDefault() ?? throw new LogisticsException("Нет грузов в заказе")))
            .OrderBy(v => v.CalculateDeliveryCost(order.Route, order.Cargos))
            .FirstOrDefault();

        if (suitableVehicle == null)
        {
            var overloadVehicle = _fleet.FirstOrDefault(v => v.State == VehicleState.Free);
            if (overloadVehicle != null)
            {
                double totalWeight = order.Cargos.Sum(c => c.WeightKg);
                if (totalWeight > overloadVehicle.MaxLoadKg)
                {
                    VehicleOverloadAttempt?.Invoke(this, new VehicleOverloadAttemptEventArgs(order, overloadVehicle, totalWeight));
                }
            }
        
            throw new LogisticsException("Нет подходящего свободного транспортного средства для данного заказа.");
        }

        decimal baseCost = suitableVehicle.CalculateDeliveryCost(order.Route, order.Cargos);
        
        IDeliveryCost cost = new BaseDeliveryCost(baseCost);
    
        suitableVehicle.State = VehicleState.InTransit;
    
        var oldStatus = order.Status;
        order.Assign(suitableVehicle, cost.Total);
    
        OrderStatusChanged?.Invoke(this, new OrderStatusChangedEventArgs(order, oldStatus, order.Status));
    }

    public void StartDelivery(Order.Order order)
    {
        var oldStatus = order.Status;
        order.StartDelivery();
        OrderStatusChanged?.Invoke(this, new OrderStatusChangedEventArgs(order, oldStatus, order.Status));
    }

    public void CompleteDelivery(Order.Order order)
    {
        var oldStatus = order.Status;
        order.Complete();
        if (order.AssignedVehicle != null)
        {
            order.AssignedVehicle.State = VehicleState.Free;
        }
        
        OrderStatusChanged?.Invoke(this, new OrderStatusChangedEventArgs(order, oldStatus, order.Status));
        DeliveryCompleted?.Invoke(this, new DeliveryCompletedEventArgs(order));
    }

    public void CancelOrder(Order.Order order)
    {
        var oldStatus = order.Status;
        order.Cancel();
        if (order.AssignedVehicle != null)
        {
            order.AssignedVehicle.State = VehicleState.Free;
        }
        
        OrderStatusChanged?.Invoke(this, new OrderStatusChangedEventArgs(order, oldStatus, order.Status));
    }
}