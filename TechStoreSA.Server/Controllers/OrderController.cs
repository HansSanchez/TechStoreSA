using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TechStoreSA.Server.Models;
using TechStoreSA.Shared;
using Microsoft.EntityFrameworkCore;

namespace TechStoreSA.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly DbTechStoreSaContext _dbTechStoreSaContext;

        public OrderController(DbTechStoreSaContext dbTechStoreSaContext)
        {
            _dbTechStoreSaContext = dbTechStoreSaContext;
        }

        [HttpGet("getOrder")]
        public async Task<IActionResult> GetOrders(string searchTerm = "", int pageNumber = 1, int pageSize = 2)
        {
            var responseApi = new ResponseAPI<PaginatedResponse<OrderDTO>>();
            try
            {
                if (pageNumber <= 0) pageNumber = 1;
                if (pageSize <= 0) pageSize = 2;

                var query = _dbTechStoreSaContext.Orders.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(e => e.Status.Contains(searchTerm));
                }

                var totalItems = await query.CountAsync();

                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(item => new OrderDTO
                    {
                        OrderId = item.OrderId,
                        OrderDate = item.OrderDate,
                        TotalAmount = item.TotalAmount,
                        Status = item.Status,
                        CreatedAt = item.CreatedAt,
                        UpdatedAt = item.UpdatedAt,
                    }).ToListAsync();

                var paginatedResponse = new PaginatedResponse<OrderDTO>
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

        [HttpGet("getByIdOrder/{id}")]
        public async Task<IActionResult> GetByIdOrder(Guid id)
        {
            var responseApi = new ResponseAPI<OrderDTO>();
            try
            {
                var dbOrder = await _dbTechStoreSaContext.Orders
                    .Include(o => o.OrderDetails) // Incluye los detalles de la orden para calcular el total
                    .FirstOrDefaultAsync(x => x.OrderId == id && x.DeletedAt == null);

                if (dbOrder == null)
                {
                    return NotFound(new { Message = "No se ha encontrado la orden que buscas por Id" });
                }

                // Recalcula el total basado en los detalles
                dbOrder.TotalAmount = dbOrder.OrderDetails.Sum(od => od.Quantity * od.Price);

                var getOrderDTO = new OrderDTO
                {
                    OrderId = dbOrder.OrderId,
                    OrderDate = dbOrder.OrderDate,
                    TotalAmount = dbOrder.TotalAmount,
                    Status = dbOrder.Status,
                    CreatedAt = dbOrder.CreatedAt,
                    UpdatedAt = dbOrder.UpdatedAt,
                };

                responseApi.IsSuccessful = true;
                responseApi.Value = getOrderDTO;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }

            return Ok(responseApi);
        }

        [HttpPost("storeOrder")]
        public async Task<IActionResult> StoreOrder(OrderDTO orderDTO)
        {
            var responseApi = new ResponseAPI<Guid>();
            try
            {
                var dbOrder = new Order
                {
                    OrderDate = DateTime.UtcNow,
                    Status = orderDTO.Status,
                    CreatedAt = DateTime.UtcNow
                };

                _dbTechStoreSaContext.Add(dbOrder);
                await _dbTechStoreSaContext.SaveChangesAsync();

                foreach (var detailDTO in orderDTO.OrderDetails)
                {
                    var detail = new OrderDetail
                    {
                        OrderId = dbOrder.OrderId,
                        ProductId = detailDTO.ProductId,
                        Quantity = detailDTO.Quantity,
                        Price = detailDTO.Price,
                        CreatedAt = DateTime.UtcNow
                    };
                    _dbTechStoreSaContext.OrderDetails.Add(detail);
                }

                dbOrder.TotalAmount = dbOrder.OrderDetails.Sum(od => od.Quantity * od.Price);
                await _dbTechStoreSaContext.SaveChangesAsync();

                responseApi.IsSuccessful = true;
                responseApi.Value = dbOrder.OrderId;
            }
            catch (Exception ex)
            {
                responseApi.IsSuccessful = false;
                responseApi.Message = ex.Message;
            }

            return Ok(responseApi);
        }



        [HttpPut("updateOrder/{id}")]
        public async Task<IActionResult> UpdateOrder(Guid id, OrderDTO orderDTO)
        {
            var responseApi = new ResponseAPI<Guid>();
            try
            {
                var dbOrder = await _dbTechStoreSaContext.Orders
                    .Include(o => o.OrderDetails) // Incluye los detalles para recalcular el total
                    .FirstOrDefaultAsync(x => x.OrderId == id);

                if (dbOrder != null)
                {
                    dbOrder.Status = orderDTO.Status;
                    dbOrder.UpdatedAt = DateTime.UtcNow;

                    // Recalcula el total basado en los detalles actualizados
                    dbOrder.TotalAmount = dbOrder.OrderDetails.Sum(od => od.Quantity * od.Price);

                    _dbTechStoreSaContext.Update(dbOrder);
                    await _dbTechStoreSaContext.SaveChangesAsync();

                    responseApi.IsSuccessful = true;
                    responseApi.Value = dbOrder.OrderId;
                }
                else
                {
                    responseApi.IsSuccessful = false;
                    responseApi.Message = "No ha sido posible encontrar la orden con ese Id";
                }
            }
            catch (Exception ex)
            {
                responseApi.IsSuccessful = false;
                responseApi.Message = ex.Message;
            }

            return Ok(responseApi);
        }

        [HttpDelete("deleteOrder/{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var responseApi = new ResponseAPI<Guid>();
            try
            {
                var dbOrder = await _dbTechStoreSaContext.Orders
                    .Include(e => e.OrderDetails)
                    .FirstOrDefaultAsync(x => x.OrderId == id && x.DeletedAt == null);

                if (dbOrder != null)
                {
                    if (dbOrder.OrderDetails.Any())
                    {
                        responseApi.IsSuccessful = false;
                        responseApi.Message = "No se puede eliminar la orden porque tiene detalles de orden relacionados.";
                    }
                    else
                    {
                        dbOrder.DeletedAt = DateTime.UtcNow;
                        await _dbTechStoreSaContext.SaveChangesAsync();

                        responseApi.IsSuccessful = true;
                        responseApi.Value = dbOrder.OrderId;
                    }
                }
                else
                {
                    responseApi.IsSuccessful = false;
                    responseApi.Message = "No ha sido posible encontrar la orden con ese Id";
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
