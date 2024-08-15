using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechStoreSA.Server.Models;
using TechStoreSA.Shared;
using Microsoft.EntityFrameworkCore;

namespace TechStoreSA.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DbTechStoreSaContext _dbTechStoreSaContext;

        public ProductController(DbTechStoreSaContext dbTechStoreSaContext)
        {
            _dbTechStoreSaContext = dbTechStoreSaContext;
        }

        [HttpGet("getProduct")]
        public async Task<IActionResult> GetProducts(string searchTerm = "", int pageNumber = 1, int pageSize = 2)
        {
            var responseApi = new ResponseAPI<PaginatedResponse<ProductDTO>>();
            try
            {
                if (pageNumber <= 0) pageNumber = 1;
                if (pageSize <= 0) pageSize = 2;

                var query = _dbTechStoreSaContext.Products.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(e => e.Name.Contains(searchTerm) || e.Description.Contains(searchTerm));
                }

                // Ordenar por fecha de creación (CreatedAt)
                query = query.OrderByDescending(e => e.CreatedAt);

                var totalItems = await query.CountAsync();

                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(item => new ProductDTO
                    {
                        ProductId = item.ProductId,
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.Price,
                        StockQuantity = item.StockQuantity,
                        Category = item.Category,
                        CreatedAt = item.CreatedAt,
                        UpdatedAt = item.UpdatedAt,
                    }).ToListAsync();

                var paginatedResponse = new PaginatedResponse<ProductDTO>
                {
                    Items = items,
                    TotalItems = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                responseApi.IsSuccessful = true;
                responseApi.Value = paginatedResponse;
            }
            catch (Exception ex)
            {
                responseApi.IsSuccessful = false;
                responseApi.Message = ex.Message;
            }

            return Ok(responseApi);
        }

        [HttpGet("getByIdProduct/{id}")]
        public async Task<IActionResult> GetByIdProduct(Guid id)
        {
            var responseApi = new ResponseAPI<ProductDTO>();
            try
            {
                var dbProduct = await _dbTechStoreSaContext.Products
                    .FirstOrDefaultAsync(x => x.ProductId == id && x.DeletedAt == null);

                if (dbProduct == null)
                {
                    return NotFound(new { Message = "No se ha encontrado el producto que buscas por Id" });
                }

                var getProductDTO = new ProductDTO
                {
                    ProductId = dbProduct.ProductId,
                    Name = dbProduct.Name,
                    Description = dbProduct.Description,
                    Price = dbProduct.Price,
                    StockQuantity = dbProduct.StockQuantity,
                    Category = dbProduct.Category,
                    CreatedAt = dbProduct.CreatedAt,
                    UpdatedAt = dbProduct.UpdatedAt,
                };

                responseApi.IsSuccessful = true;
                responseApi.Value = getProductDTO;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }

            return Ok(responseApi);
        }

        [HttpPost("storeProduct")]
        public async Task<IActionResult> StoreProduct(ProductDTO productDTO)
        {
            var responseApi = new ResponseAPI<Guid>();
            try
            {
                var dbProduct = new Product
                {
                    Name = productDTO.Name,
                    Description = productDTO.Description,
                    Price = productDTO.Price,
                    StockQuantity = productDTO.StockQuantity,
                    Category = productDTO.Category,
                    CreatedAt = DateTime.UtcNow,
                };

                _dbTechStoreSaContext.Add(dbProduct);
                await _dbTechStoreSaContext.SaveChangesAsync();

                responseApi.IsSuccessful = true;
                responseApi.Value = dbProduct.ProductId;
            }
            catch (Exception ex)
            {
                responseApi.IsSuccessful = false;
                responseApi.Message = ex.Message;
            }

            return Ok(responseApi);
        }

        [HttpPut("updateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, ProductDTO productDTO)
        {
            var responseApi = new ResponseAPI<Guid>();
            try
            {
                var dbProduct = await _dbTechStoreSaContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);

                if (dbProduct != null)
                {
                    dbProduct.Name = productDTO.Name;
                    dbProduct.Description = productDTO.Description;
                    dbProduct.Price = productDTO.Price;
                    dbProduct.StockQuantity = productDTO.StockQuantity;
                    dbProduct.Category = productDTO.Category;
                    dbProduct.UpdatedAt = DateTime.UtcNow;

                    _dbTechStoreSaContext.Update(dbProduct);
                    await _dbTechStoreSaContext.SaveChangesAsync();

                    responseApi.IsSuccessful = true;
                    responseApi.Value = dbProduct.ProductId;
                }
                else
                {
                    responseApi.IsSuccessful = false;
                    responseApi.Message = "No ha sido posible encontrar el producto con ese Id";
                }
            }
            catch (Exception ex)
            {
                responseApi.IsSuccessful = false;
                responseApi.Message = ex.Message;
            }

            return Ok(responseApi);
        }

        [HttpDelete("deleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var responseApi = new ResponseAPI<Guid>();
            try
            {
                var dbProduct = await _dbTechStoreSaContext.Products
                    .Include(e => e.OrderDetails)
                    .FirstOrDefaultAsync(x => x.ProductId == id && x.DeletedAt == null);

                if (dbProduct != null)
                {
                    if (dbProduct.OrderDetails.Any())
                    {
                        responseApi.IsSuccessful = false;
                        responseApi.Message = "No se puede eliminar el producto porque tiene detalles de orden relacionados.";
                    }
                    else
                    {
                        dbProduct.DeletedAt = DateTime.UtcNow;
                        await _dbTechStoreSaContext.SaveChangesAsync();

                        responseApi.IsSuccessful = true;
                        responseApi.Value = dbProduct.ProductId;
                    }
                }
                else
                {
                    responseApi.IsSuccessful = false;
                    responseApi.Message = "No ha sido posible encontrar el producto con ese Id";
                }
            }
            catch (Exception ex)
            {
                responseApi.IsSuccessful = false;
                responseApi.Message = ex.InnerException?.Message ?? ex.Message;
            }

            return Ok(responseApi);
        }

    }
}
