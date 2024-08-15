using TechStoreSA.Shared;

namespace TechStoreSA.Client.Interfaces
{
    public interface IProductService
    {
        Task<PaginatedResponse<ProductDTO>> GetProducts(string searchTerm, int pageNumber, int pageSize);
        Task<ProductDTO> GetByIdProduct(Guid id);
        Task<Guid> StoreProduct(ProductDTO productDTO);
        Task<Guid> UpdateProduct(ProductDTO productDTO);
        Task<ResponseAPI<Guid>> DeleteProduct(Guid id);
    }
}
