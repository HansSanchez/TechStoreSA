using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechStoreSA.Shared
{
    public class OrderDetailDTO
    {
        public Guid OrderDetailId { get; set; }

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public ProductDTO ProductDTO { get; set; } = new ProductDTO();

        public OrderDTO? OrderDTO { get; set; } = new OrderDTO();
    }
}
