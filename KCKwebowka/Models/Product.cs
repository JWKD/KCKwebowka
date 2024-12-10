namespace KCKwebowka.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } // Relacja do kategorii

    }
}
