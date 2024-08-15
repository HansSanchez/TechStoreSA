namespace TechStoreSA.Shared
{
    public class OrderDTO
    {
        public Guid OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        // Propiedad para los detalles de la orden
        public List<OrderDetailDTO> OrderDetails { get; set; } = new List<OrderDetailDTO>();
    }
}
