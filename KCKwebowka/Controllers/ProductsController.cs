using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KCKwebowka.Models;

namespace KCKwebowka.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ShopDbContext _context;


        public ProductsController(ShopDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public IActionResult Index()
        {

            // Odczytujemy wartość z sesji, czy pokazujemy ceny brutto
            var showBrutto = HttpContext.Session.GetString("ShowBrutto");

            // Przekształcamy wartość z sesji (jeśli jest) na bool
            ViewBag.ShowBrutto = !string.IsNullOrEmpty(showBrutto) && bool.Parse(showBrutto);


            var products = _context.Products.Include(p => p.Category).ToList();
            return View(products);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId)
        {

            // Odczytujemy wartość z sesji, czy pokazujemy ceny brutto
            var showBrutto = HttpContext.Session.GetString("ShowBrutto");

            // Przekształcamy wartość z sesji (jeśli jest) na bool
            ViewBag.ShowBrutto = !string.IsNullOrEmpty(showBrutto) && bool.Parse(showBrutto);


            // Pobierz produkt z bazy danych
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                return NotFound(); // Jeśli produkt nie istnieje, zwróć błąd
            }

            // Pobierz wspólny koszyk z bazy danych (tylko jeden koszyk dla wszystkich)
            var cart = _context.ShoppingCarts.Include(c => c.productsInCart)
                                      .FirstOrDefault();

            if (cart == null)
            {
                // Jeśli koszyk nie istnieje, stwórz nowy
                cart = new ShoppingCart();
                _context.ShoppingCarts.Add(cart);
                _context.SaveChanges(); // Zapisz koszyk w bazie, aby uzyskać Cart.Id
            }

            // Dodaj produkt do koszyka (nawet jeśli produkt już jest, dodaj go ponownie)
            cart.productsInCart.Add(product
            );

            // Zapisz zmiany w bazie danych
            _context.SaveChanges();

            // Przekierowanie z powrotem do strony głównej (lub do strony koszyka)
            return RedirectToAction("Index");
        }

        public IActionResult Cart()
        {

            // Odczytujemy wartość z sesji, czy pokazujemy ceny brutto
            var showBrutto = HttpContext.Session.GetString("ShowBrutto");

            // Przekształcamy wartość z sesji (jeśli jest) na bool
            ViewBag.ShowBrutto = !string.IsNullOrEmpty(showBrutto) && bool.Parse(showBrutto);


            // Pobierz koszyk i załaduj powiązane produkty (produkty w koszyku oraz powiązane dane produktu)
            var cart = _context.ShoppingCarts
                               .Include(c => c.productsInCart)        // Załącz produkty w koszyku
                               .FirstOrDefault();
            
            // Jeśli koszyk jest pusty, zwróć stronę pustego koszyka
            if (cart == null || !cart.productsInCart.Any())
            {
                return View("EmptyCart"); // Pusty koszyk
            }

            var products = _context.Products.Include(p => p.Category).ToList();

            // Zwróć widok z koszykiem
            return View(new { Cart = cart, Products = products });
        }

        public IActionResult RemoveFromCart(int productId)
        {
            // Odczytujemy wartość z sesji, czy pokazujemy ceny brutto
            var showBrutto = HttpContext.Session.GetString("ShowBrutto");

            // Przekształcamy wartość z sesji (jeśli jest) na bool
            ViewBag.ShowBrutto = !string.IsNullOrEmpty(showBrutto) && bool.Parse(showBrutto);
            // Pobierz koszyk
            var cart = _context.ShoppingCarts
                               .Include(c => c.productsInCart)
                               .FirstOrDefault();

            if (cart != null)
            {
                // Znajdź produkt w koszyku
                var productInCart = cart.productsInCart.FirstOrDefault(pi => pi.Id == productId);

                if (productInCart != null)
                {
                    
                    // Usuń produkt z koszyka
                    cart.productsInCart.Remove(productInCart);

                    // Zapisz zmiany w koszyku
                    _context.SaveChanges();
                }
            }

            // Po usunięciu przekieruj użytkownika do widoku koszyka
            return RedirectToAction("Cart");
        }


        // Metoda do wyświetlania formularza wyszukiwania
        public IActionResult Search()
        {

            // Odczytujemy wartość z sesji, czy pokazujemy ceny brutto
            var showBrutto = HttpContext.Session.GetString("ShowBrutto");

            // Przekształcamy wartość z sesji (jeśli jest) na bool
            ViewBag.ShowBrutto = !string.IsNullOrEmpty(showBrutto) && bool.Parse(showBrutto);


            ViewData["Categories"] = _context.Categories.ToList();  // Pobierz wszystkie kategorie
            ViewData["SearchName"] = "";  // Pusta wartość na początek
            ViewData["CategoryId"] = (int?)null;  // Pusta wartość na początek
            ViewData["Products"] = new List<Product>();  // Pusta lista produktów na początek

            return View();
        }

        // Metoda do wyszukiwania po nazwie
        public IActionResult SearchByName(string searchName)
        {

            // Odczytujemy wartość z sesji, czy pokazujemy ceny brutto
            var showBrutto = HttpContext.Session.GetString("ShowBrutto");

            // Przekształcamy wartość z sesji (jeśli jest) na bool
            ViewBag.ShowBrutto = !string.IsNullOrEmpty(showBrutto) && bool.Parse(showBrutto);


            var products = _context.Products
                                   .Where(p => p.name.Contains(searchName))
                                   .Include(p => p.Category)
                                   .ToList();

            ViewData["Categories"] = _context.Categories.ToList();  // Pobierz wszystkie kategorie
            ViewData["SearchName"] = searchName;  // Przekazujemy nazwę do widoku
            ViewData["CategoryId"] = (int?)null;  // Pusta wartość dla kategorii
            ViewData["Products"] = products;  // Przekazujemy produkty do widoku

            return View("Search");
        }

        // Metoda do wyszukiwania po kategorii
        public IActionResult SearchByCategory(int? categoryId)
        {

            // Odczytujemy wartość z sesji, czy pokazujemy ceny brutto
            var showBrutto = HttpContext.Session.GetString("ShowBrutto");

            // Przekształcamy wartość z sesji (jeśli jest) na bool
            ViewBag.ShowBrutto = !string.IsNullOrEmpty(showBrutto) && bool.Parse(showBrutto);


            var products = _context.Products
                                   .Where(p => p.CategoryId == categoryId)
                                   .Include(p => p.Category)
                                   .ToList();

            ViewData["Categories"] = _context.Categories.ToList();  // Pobierz wszystkie kategorie
            ViewData["SearchName"] = "";  // Pusta wartość dla wyszukiwania po nazwie
            ViewData["CategoryId"] = categoryId;  // Przekazujemy ID kategorii do widoku
            ViewData["Products"] = products;  // Przekazujemy produkty do widoku

            return View("Search");
        }


        [HttpPost]
        public IActionResult TogglePrice(bool priceToggle)
        {
            // Zapisujemy stan checkboxa w sesji
            HttpContext.Session.SetString("ShowBrutto", priceToggle.ToString());

            // Po zapisaniu przekierowujemy do strony z produktami
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Checkout()
        {
            // Pobierz koszyk użytkownika (w tym przypadku globalny)
            var cart = _context.ShoppingCarts
                               .Include(c => c.productsInCart)
                               .FirstOrDefault();

            if (cart != null)
            {
                // Usuń wszystkie produkty z koszyka
                cart.productsInCart.Clear();
                _context.SaveChanges(); // Zapisz zmiany w bazie danych
            }

            // Przekieruj na stronę potwierdzenia zamówienia lub główną
            return View("AfterOrder");
        }


    }
}
