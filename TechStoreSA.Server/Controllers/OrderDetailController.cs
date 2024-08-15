using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechStoreSA.Server.Models;
using TechStoreSA.Shared;
using Microsoft.EntityFrameworkCore;

namespace TechStoreSA.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly DbTechStoreSaContext _dbTechStoreSaContext;

        public OrderDetailController(DbTechStoreSaContext dbTechStoreSaContext)
        {
            _dbTechStoreSaContext = dbTechStoreSaContext;
        }

        [HttpGet("getOrderDetails")]
        public async Task<IActionResult> GetOrderDetails(Guid orderId, int pageNumber = 1, int pageSize = 10)
        {
            var responseApi = new ResponseAPI<PaginatedResponse<OrderDetailDTO>>();
            try
            {
                var query = _dbTechStoreSaContext.OrderDetails
                    .Include(od => od.Product)
                    .Include(od => od.Order)
                    .Where(od => od.OrderId == orderId && od.DeletedAt == null)
                    .Select(od => new OrderDetailDTO
                    {
                        OrderDetailId = od.OrderDetailId,
                        OrderId = od.OrderId,
                        ProductId = od.ProductId,
                        Quantity = od.Quantity,
                        Price = od.Price,
                        CreatedAt = od.CreatedAt,
                        UpdatedAt = od.UpdatedAt,
                        ProductDTO = new ProductDTO
                        {
                            ProductId = od.Product.ProductId,
                            Name = od.Product.Name,
                            Description = od.Product.Description,
                            Price = od.Product.Price,
                            StockQuantity = od.Product.StockQuantity,
                            Category = od.Product.Category,
                            CreatedAt = od.Product.CreatedAt,
                            UpdatedAt = od.Product.UpdatedAt,
                            DeletedAt = od.Product.DeletedAt
                        },
                        OrderDTO = new OrderDTO
                        {
                            OrderId = od.Order.OrderId,
                            OrderDate = od.Order.OrderDate,
                            TotalAmount = od.Order.TotalAmount,
                            Status = od.Order.Status,
                            CreatedAt = od.Order.CreatedAt,
                            UpdatedAt = od.Order.UpdatedAt,
                            DeletedAt = od.Order.DeletedAt
                        }
                    });

                var totalItems = await query.CountAsync();

                var orderDetails = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (!orderDetails.Any())
                {
                    return NotFound(new { Message = "No se encontraron detalles de orden para este ID de orden." });
                }

                var paginatedResponse = new PaginatedResponse<OrderDetailDTO>
                {
                    Items = orderDetails,
                    TotalItems = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                responseApi.IsSuccessful = true;
                responseApi.Value = paginatedResponse;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }

            return Ok(responseApi);
        }

        [HttpGet("getByIdOrderDetail/{id}")]
        public async Task<IActionResult> GetByIdOrderDetail(Guid id)
        {
            var responseApi = new ResponseAPI<OrderDetailDTO>();
            try
            {
                var orderDetail = await _dbTechStoreSaContext.OrderDetails
                    .FirstOrDefaultAsync(od => od.OrderDetailId == id && od.DeletedAt == null);

                if (orderDetail == null)
                {
                    return NotFound(new { Message = "No se ha encontrado el detalle de orden con ese ID." });
                }

                var orderDetailDTO = new OrderDetailDTO
                {
                    OrderDetailId = orderDetail.OrderDetailId,
                    OrderId = orderDetail.OrderId,
                    ProductId = orderDetail.ProductId,
                    Quantity = orderDetail.Quantity,
                    Price = orderDetail.Price,
                    CreatedAt = orderDetail.CreatedAt,
                    UpdatedAt = orderDetail.UpdatedAt,
                };

                responseApi.IsSuccessful = true;
                responseApi.Value = orderDetailDTO;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }

            return Ok(responseApi);
        }

        [HttpPost("storeOrderDetail")]
        public async Task<IActionResult> StoreOrderDetail(OrderDetailDTO orderDetailDTO)
        {
            var responseApi = new ResponseAPI<Guid>();
            try
            {
                var orderExists = await _dbTechStoreSaContext.Orders.AnyAsync(o => o.OrderId == orderDetailDTO.OrderId && o.DeletedAt == null);
                var productExists = await _dbTechStoreSaContext.Products.AnyAsync(p => p.ProductId == orderDetailDTO.ProductId && p.DeletedAt == null);

                if (!orderExists)
                {
                    return BadRequest(new { Message = "No existe una orden con ese ID." });
                }

                if (!productExists)
                {
                    return BadRequest(new { Message = "No existe un producto con ese ID." });
                }

                if (orderDetailDTO.Quantity <= 0)
                {
                    return BadRequest(new { Message = "La cantidad debe ser mayor que cero." });
                }

                var orderDetail = new OrderDetail
                {
                    OrderId = orderDetailDTO.OrderId,
                    ProductId = orderDetailDTO.ProductId,
                    Quantity = orderDetailDTO.Quantity,
                    Price = orderDetailDTO.Price,
                    CreatedAt = DateTime.UtcNow,
                };

                _dbTechStoreSaContext.OrderDetails.Add(orderDetail);
                await _dbTechStoreSaContext.SaveChangesAsync();

                responseApi.IsSuccessful = true;
                responseApi.Value = orderDetail.OrderDetailId;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }

            return Ok(responseApi);
        }

        [HttpPut("updateOrderDetail/{id}")]
        public async Task<IActionResult> UpdateOrderDetail(Guid id, OrderDetailDTO orderDetailDTO)
        {
            var responseApi = new ResponseAPI<Guid>();
            try
            {
                var orderDetail = await _dbTechStoreSaContext.OrderDetails
                    .FirstOrDefaultAsync(od => od.OrderDetailId == id && od.DeletedAt == null);

                if (orderDetail == null)
                {
                    return NotFound(new { Message = "No se ha encontrado el detalle de orden con ese ID." });
                }

                if (orderDetailDTO.Quantity <= 0)
                {
                    return BadRequest(new { Message = "La cantidad debe ser mayor que cero." });
                }

                orderDetail.Quantity = orderDetailDTO.Quantity;
                orderDetail.Price = orderDetailDTO.Price;
                orderDetail.UpdatedAt = DateTime.UtcNow;

                _dbTechStoreSaContext.OrderDetails.Update(orderDetail);
                await _dbTechStoreSaContext.SaveChangesAsync();

                responseApi.IsSuccessful = true;
                responseApi.Value = orderDetail.OrderDetailId;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }

            return Ok(responseApi);
        }

        [HttpDelete("deleteOrderDetail/{id}")]
        public async Task<IActionResult> DeleteOrderDetail(Guid id)
        {
            var responseApi = new ResponseAPI<Guid>();
            try
            {
                var orderDetail = await _dbTechStoreSaContext.OrderDetails
                    .FirstOrDefaultAsync(od => od.OrderDetailId == id && od.DeletedAt == null);

                if (orderDetail == null)
                {
                    return NotFound(new { Message = "No se ha encontrado el detalle de orden con ese ID." });
                }

                orderDetail.DeletedAt = DateTime.UtcNow;
                await _dbTechStoreSaContext.SaveChangesAsync();

                responseApi.IsSuccessful = true;
                responseApi.Value = orderDetail.OrderDetailId;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }

            return Ok(responseApi);
        }
    }
}
