using System.Net.Http.Json;
using TechStoreSA.Client.Interfaces;
using TechStoreSA.Shared;

namespace TechStoreSA.Client.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly HttpClient _http;

        public OrderDetailService(HttpClient http)
        {
            _http = http;
        }

        public async Task<PaginatedResponse<OrderDetailDTO>> GetOrderDetails(Guid orderId, int pageNumber, int pageSize)
        {
            var response = await _http.GetFromJsonAsync<ResponseAPI<PaginatedResponse<OrderDetailDTO>>>($"api/OrderDetail/getOrderDetails?orderId={orderId}&pageNumber={pageNumber}&pageSize={pageSize}");
            if (response!.IsSuccessful)
            {
                return response.Value!;
            }
            else
            {
                throw new Exception(response.Message);
            }
        }

        public async Task<OrderDetailDTO> GetByIdOrderDetail(Guid id)
        {
            var response = await _http.GetFromJsonAsync<ResponseAPI<OrderDetailDTO>>($"api/OrderDetail/getByIdOrderDetail/{id}");
            if (response!.IsSuccessful)
            {
                return response.Value!;
            }
            else
            {
                throw new Exception(response.Message);
            }
        }

        public async Task<Guid> StoreOrderDetail(OrderDetailDTO orderDetailDTO)
        {
            var request = await _http.PostAsJsonAsync("api/OrderDetail/storeOrderDetail", orderDetailDTO);
            var response = await request.Content.ReadFromJsonAsync<ResponseAPI<Guid>>();

            if (response!.IsSuccessful)
            {
                return response.Value!;
            }
            else
            {
                throw new Exception(response.Message);
            }
        }

        public async Task<Guid> UpdateOrderDetail(OrderDetailDTO orderDetailDTO)
        {
            var request = await _http.PutAsJsonAsync($"api/OrderDetail/updateOrderDetail/{orderDetailDTO.OrderDetailId}", orderDetailDTO);
            var response = await request.Content.ReadFromJsonAsync<ResponseAPI<Guid>>();

            if (response!.IsSuccessful)
            {
                return response.Value!;
            }
            else
            {
                throw new Exception(response.Message);
            }
        }

        public async Task<ResponseAPI<Guid>> DeleteOrderDetail(Guid id)
        {
            var request = await _http.DeleteAsync($"api/OrderDetail/deleteOrderDetail/{id}");
            if (!request.IsSuccessStatusCode)
            {
                return new ResponseAPI<Guid> { IsSuccessful = false, Message = "Error al eliminar el orderDetailo en el servidor." };
            }

            var response = await request.Content.ReadFromJsonAsync<ResponseAPI<Guid>>();

            if (response == null)
            {
                return new ResponseAPI<Guid> { IsSuccessful = false, Message = "Respuesta del servidor vacía o inválida." };
            }

            return response;
        }
    }
}
