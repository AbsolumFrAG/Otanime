using System;
using System.ComponentModel.DataAnnotations;

namespace Otanime 
{ 
    public class ProductOrder
    { 
        [Key] 
        public int ProductOrderId { get; set; } 

        [Required] 
        public int ProductId { get; set; }
        public required Product Product { get; set; }

        [Required] 
        public int OrderId { get; set; } 
        public required Order Order { get; set; }

        [Required] 
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public DateTime? DeliveryDate { get; set; }
    } 
}
