using Orders.Domain.Entities;
using Orders.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Domain.DataAccess.Repositories
{
    public interface IOrderRepository
    {
        Task<int> AddAsync(Order order);
        Task<List<int>> AddAllAsync(IEnumerable<Order> orders);
        Task UpdateOrder(Order order, string newExternalId = null);
        Task<IEnumerable<Order>> GetOrdersByFilters(IDictionary<string, string> filters);
        Task<IEnumerable<Order>> GetAllOrders();
        Task<IEnumerable<Order>> GetOrdersByEmail(string email);
        Task<IEnumerable<Order>> GetOrdersByStatus(Status status);
        Task<IEnumerable<Order>> GetOrdersByUserId(int userId);
    }
}
