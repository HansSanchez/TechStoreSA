using TechStoreSA.Shared;

namespace TechStoreSA.Client.Interfaces
{
    public interface IOrderDetailService
    {
        Task<PaginatedResponse<OrderDetailDTO>> GetOrderDetails(Guid orderID, int pageNumber, int pageSize);
        Task<OrderDetailDTO> GetByIdOrderDetail(Guid id);
        Task<Guid> StoreOrderDetail(OrderDetailDTO orderDetailDTO);
        Task<Guid> UpdateOrderDetail(OrderDetailDTO orderDetailDTO);
        Task<ResponseAPI<Guid>> DeleteOrderDetail(Guid id);
    }
}
