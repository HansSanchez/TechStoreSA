using System.Net.Http.Json;
using TechStoreSA.Client.Interfaces;
using TechStoreSA.Shared;

namespace TechStoreSA.Client.Services
{
    public class ProductService: IProductService
    {
        private readonly HttpClient _http;

        public ProductService(HttpClient http)
        {
            _http = http;
        }

        public async Task<PaginatedResponse<ProductDTO>> GetProducts(string searchTerm, int pageNumber, int pageSize)
        {
            var response = await _http.GetFromJsonAsync<ResponseAPI<PaginatedResponse<ProductDTO>>>($"api/Product/getProduct?searchTerm={searchTerm}&pageNumber={pageNumber}&pageSize={pageSize}");
            if (response!.IsSuccessful)
            {
                return response.Value!;
            }
            else
            {
                throw new Exception(response.Message);
            }
        }


        public async Task<ProductDTO> GetByIdProduct(Guid id)
        {
            var response = await _http.GetFromJsonAsync<ResponseAPI<ProductDTO>>($"api/Product/getByIdProduct/{id}");
            if (response!.IsSuccessful)
            {
                return response.Value!;
            }
            else
            {
                throw new Exception(response.Message);
            }
        }

        public async Task<Guid> StoreProduct(ProductDTO productDTO)
        {
            var request = await _http.PostAsJsonAsync("api/Product/storeProduct", productDTO);
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

        public async Task<Guid> UpdateProduct(ProductDTO productDTO)
        {
            var request = await _http.PutAsJsonAsync($"api/Product/updateProduct/{productDTO.ProductId}", productDTO);
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

        public async Task<ResponseAPI<Guid>> DeleteProduct(Guid id)
        {
            var request = await _http.DeleteAsync($"api/Product/deleteProduct/{id}");
            if (!request.IsSuccessStatusCode)
            {
                return new ResponseAPI<Guid> { IsSuccessful = false, Message = "Error al eliminar el producto en el servidor." };
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
