using System.Net.Http.Json;
using TechStoreSA.Client.Interfaces;
using TechStoreSA.Shared;

namespace TechStoreSA.Client.Services
{
    public class OrderService: IOrderService
    {
        private readonly HttpClient _http;

        public OrderService(HttpClient http)
        {
            _http = http;
        }

        public async Task<PaginatedResponse<OrderDTO>> GetOrders(string searchTerm, int pageNumber, int pageSize)
        {
            var response = await _http.GetFromJsonAsync<ResponseAPI<PaginatedResponse<OrderDTO>>>($"api/Order/getOrder?searchTerm={searchTerm}&pageNumber={pageNumber}&pageSize={pageSize}");
            if (response!.IsSuccessful)
            {
                return response.Value!;
            }
            else
            {
                throw new Exception(response.Message);
            }
        }


        public async Task<OrderDTO> GetByIdOrder(Guid id)
        {
            var response = await _http.GetFromJsonAsync<ResponseAPI<OrderDTO>>($"api/Order/getByIdOrder/{id}");
            if (response!.IsSuccessful)
            {
                return response.Value!;
            }
            else
            {
                throw new Exception(response.Message);
            }
        }

        public async Task<Guid> StoreOrder(OrderDTO OrderDTO)
        {
            var request = await _http.PostAsJsonAsync("api/Order/storeOrder", OrderDTO);
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

        public async Task<Guid> UpdateOrder(OrderDTO OrderDTO)
        {
            var request = await _http.PutAsJsonAsync($"api/Order/updateOrder/{OrderDTO.OrderId}", OrderDTO);
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

        public async Task<ResponseAPI<Guid>> DeleteOrder(Guid id)
        {
            var request = await _http.DeleteAsync($"api/Order/deleteOrder/{id}");
            if (!request.IsSuccessStatusCode)
            {
                return new ResponseAPI<Guid> { IsSuccessful = false, Message = "Error al eliminar la orden en el servidor." };
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
