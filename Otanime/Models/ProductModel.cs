using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Otanime.Models
{
    public class ProductModel
    {
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Stock { get; set; }
        
        [Required(AllowEmptyStrings = true)]
        public List<ImageModel> Images { get; set; } = new List<ImageModel>();
    }

    public class ImageModel
    {
        public string ImageUrl { get; set; }
    }
}
