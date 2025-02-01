using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using SiparisYonetim.Data;
using SiparisYonetim.Models;
using Newtonsoft.Json;

namespace SiparisYonetim.Pages
{
    public class StoreModel : PageModel
    {
        private readonly AppDbContext _context;

        public StoreModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Product> Products { get; set; }

        [BindProperty]
        public int ProductID { get; set; }

        [BindProperty]
        public int Quantity { get; set; }

        public void OnGet()
        {
            Products = _context.Products.ToList();
        }

        public IActionResult OnPostAddToCart()
        {
            
            var product = _context.Products.FirstOrDefault(p => p.ProductID == ProductID);
            if (product == null)
            {
                TempData["Error"] = "Ürün bulunamadý.";
                return RedirectToPage();
            }

            
            var cart = GetCartFromSession();

            
            var cartItem = cart.FirstOrDefault(ci => ci.ProductID == ProductID);
            if (cartItem != null)
            {
                
                cartItem.Quantity += Quantity;
            }
            else
            {
                
                cart.Add(new CartItem
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Quantity = Quantity
                });
            }

            
            SaveCartToSession(cart);

            TempData["SuccessMessage"] = "Ürün sepete baþarýyla eklendi!";
            return RedirectToPage();
        }

        private List<CartItem> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            return string.IsNullOrEmpty(cartJson) ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
        }

        private void SaveCartToSession(List<CartItem> cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("Cart", cartJson);
        }

        public IActionResult OnPostClearCart()
        {
            HttpContext.Session.Remove("Cart"); 
            return RedirectToPage();
        }

    }
}
