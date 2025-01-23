using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Otanime
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [MaxLength(100)]
        public string? PaymentMethod { get; set; }

        [MaxLength(100)]
        public string? PaymentStatus { get; set; }

        public List<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();

        [MaxLength(100)]
        public string? Status { get; set; }

        [MaxLength(100)]
        public string? DeliveryType { get; set; }

        [MaxLength(100)]
        public string? DeliveryPrice { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [Phone]
        public string? Phone { get; set; }
    }
}
