using LogiCore.Domain.Common;

namespace LogiCore.Domain.Order;

public class Customer : IEntity
{
    public Guid Id { get; }
    public string Name { get; }
    public string ContactInfo { get; }

    private readonly List<Order> _orderHistory = new();
    
    public IReadOnlyCollection<Order> OrderHistory => _orderHistory.AsReadOnly();

    public Customer(string name, string contactInfo)
    {
        Id = Guid.NewGuid();
        Name = name;
        ContactInfo = contactInfo;
    }

    internal void AddOrderToHistory(Order order)
    {
        _orderHistory.Add(order);
    }
}