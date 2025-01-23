using System.ComponentModel.DataAnnotations;

namespace Otanime
{
    public class Image
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
