using LogiCore.Domain.Cargo;
using LogiCore.Domain.Common;
using LogiCore.Domain.Exceptions;
using LogiCore.Domain.Vehicle;

namespace LogiCore.Domain.Order;

public enum OrderStatus
{
    Created,
    Assigned,
    InTransit,
    Delivered,
    Cancelled
}

public class Order : IEntity
{
    public Guid Id { get; }
    public Customer Customer { get; }
    
    private readonly List<Cargo.Cargo> _cargos;
    public IReadOnlyCollection<Cargo.Cargo> Cargos => _cargos.AsReadOnly();
    
    public Route Route { get; }
    
    public Vehicle.Vehicle? AssignedVehicle { get; private set; }
    public decimal TotalCost { get; private set; }

    public OrderStatus Status { get; private set; }

    public Order(Customer customer, Route route, IEnumerable<Cargo.Cargo> cargos)
    {
        Id = Guid.NewGuid();
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        Route = route ?? throw new ArgumentNullException(nameof(route));
        _cargos = cargos.ToList();
        Status = OrderStatus.Created;
    }
    
    public void Assign(Vehicle.Vehicle vehicle, decimal cost)
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOrderStateException($"Невозможно назначить ТС. Текущий статус: {Status}");
        
        AssignedVehicle = vehicle;
        TotalCost = cost;
        Status = OrderStatus.Assigned;
    }
    
    public void StartDelivery()
    {
        if (Status != OrderStatus.Assigned)
            throw new InvalidOrderStateException($"Невозможно начать доставку. Текущий статус: {Status}");

        Status = OrderStatus.InTransit;
    }
    
    public void Complete()
    {
        if (Status != OrderStatus.InTransit)
            throw new InvalidOrderStateException($"Невозможно завершить доставку. Текущий статус: {Status}");

        Status = OrderStatus.Delivered;
    }
    
    public void Cancel()
    {
        if (Status != OrderStatus.Created && Status != OrderStatus.Assigned)
            throw new InvalidOrderStateException($"Невозможно отменить заказ. Текущий статус: {Status}");

        Status = OrderStatus.Cancelled;
    }
}