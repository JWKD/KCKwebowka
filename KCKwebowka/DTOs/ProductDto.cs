using KCKwebowka.Models;

namespace KCKwebowka.DTOs
{
    public class ProductDTO
    {
        public string Name { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; }
    }
}
