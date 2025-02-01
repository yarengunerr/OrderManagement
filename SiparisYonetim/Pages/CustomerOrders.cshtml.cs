using Microsoft.AspNetCore.Mvc.RazorPages;
using SiparisYonetim.Models;
using SiparisYonetim.Data;
using Microsoft.EntityFrameworkCore;

namespace SiparisYonetim.Pages
{
    public class CustomerOrdersModel : PageModel
    {
        private readonly AppDbContext _context;

        public CustomerOrdersModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Order> Orders { get; set; } = new();

        public void OnGet()
        {
            var customerId = GetCustomerId();

            if (customerId == -1)
            {
                TempData["Error"] = "Lütfen giriþ yapýn.";
                RedirectToPage("Login");
            }

            
            Orders = _context.Orders
                .Where(o => o.CustomerID == customerId)
                .Include(o => o.Product)
                .ToList();
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
