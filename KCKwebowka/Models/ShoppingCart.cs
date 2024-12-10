namespace KCKwebowka.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public ICollection<Product> productsInCart { get; set; } = new List<Product>();
        public float fullprice { get; set; }
        public float fullpriceNetto { get; set; }



    }
}
