using Microsoft.AspNetCore.Mvc.RazorPages;
using SiparisYonetim.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using SiparisYonetim.Data;
using Microsoft.AspNetCore.SignalR;
using SiparisYonetim.Hubs;
using Microsoft.EntityFrameworkCore;

namespace SiparisYonetim.Pages
{
    public class CartModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ProductHub> _hubContext;

        public CartModel(AppDbContext context, IHubContext<ProductHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        public void OnGet()
        {
            var customerId = GetCustomerId();

            if (customerId == -1)
            {
                TempData["Error"] = "Lütfen giriþ yapýn.";
                RedirectToPage("Login");
            }

            var cartJson = HttpContext.Session.GetString("Cart");
            CartItems = string.IsNullOrEmpty(cartJson) ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
        }

        public async Task<IActionResult> OnPostConfirmOrderAsync()
        {
            var customerId = GetCustomerId();

            if (customerId == -1)
            {
                TempData["Error"] = "Lütfen giriþ yapýn.";
                return RedirectToPage("Login");
            }

            var cart = GetCartFromSession();
            var ordersToNotify = new List<Order>();

            foreach (var item in cart)
            {
                var product = _context.Products.Find(item.ProductID);
                if (product != null && product.Stock >= item.Quantity)
                {
                    var order = new Order
                    {
                        CustomerID = customerId,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        TotalPrice = item.Price * item.Quantity,
                        OrderStatus = "Bekliyor",
                        OrderDate = DateTime.Now
                    };

                    _context.Orders.Add(order);
                    ordersToNotify.Add(order);
                }
                else
                {
                    TempData["Error"] = "Stok yetersiz!";
                    return RedirectToPage();
                }
            }

            await _context.SaveChangesAsync();

            
            var newOrdersJson = JsonConvert.SerializeObject(ordersToNotify);
            await _hubContext.Clients.All.SendAsync("ReceiveNewOrders", newOrdersJson);

            HttpContext.Session.Remove("Cart");
            TempData["SuccessMessage"] = "Sipariþiniz baþarýyla onaylandý ve admin paneline iletildi.";

            return RedirectToPage("/Cart");
        }



        private List<CartItem> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            return string.IsNullOrEmpty(cartJson) ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
        }

        private int GetCustomerId()
        {
            if (User.Identity.IsAuthenticated)
            {
                var customerIdClaim = User.FindFirst("CustomerID");
                if (customerIdClaim != null)
                {
                    return int.Parse(customerIdClaim.Value);
                }
            }
            return -1;
        }
    }
}