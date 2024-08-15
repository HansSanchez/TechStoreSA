using TechStoreSA.Shared;

namespace TechStoreSA.Client.Interfaces
{
    public interface IOrderService
    {
        Task<PaginatedResponse<OrderDTO>> GetOrders(string searchTerm, int pageNumber, int pageSize);
        Task<OrderDTO> GetByIdOrder(Guid id);
        Task<Guid> StoreOrder(OrderDTO orderDTO);
        Task<Guid> UpdateOrder(OrderDTO orderDTO);
        Task<ResponseAPI<Guid>> DeleteOrder(Guid id);
    }
}
