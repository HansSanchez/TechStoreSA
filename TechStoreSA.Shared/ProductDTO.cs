using System.ComponentModel.DataAnnotations;

namespace TechStoreSA.Shared
{
    public class ProductDTO
    {
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "El campo nombre es de carácter obligatorio")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El campo descripción es de carácter obligatorio")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El campo precio es de carácter obligatorio")]
        [Range(1.00, double.MaxValue, ErrorMessage = "El precio debe ser un número positivo.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El campo cantidad de stock es de carácter obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser un número positivo.")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "El campo categoría es de carácter obligatorio")]
        public string Category { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
