using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SiparisYonetim.Data;
using SiparisYonetim.Models;

namespace SiparisYonetim.Pages
{
    public class CustomerPanelModel : PageModel
    {
        private readonly AppDbContext _context;

        public CustomerPanelModel(AppDbContext context)
        {
            _context = context;
        }

        public Customer Customer { get; private set; }

        public IActionResult OnGet(string customerName)
        {
            if (string.IsNullOrEmpty(customerName))
            {
                TempData["Error"] = "Müþteri adý belirtilmedi!";
                return RedirectToPage("Login");
            }

            Customer = _context.Customers.FirstOrDefault(c => c.CustomerName == customerName);

            if (Customer == null)
            {
                TempData["Error"] = "Müþteri bulunamadý!";
                return RedirectToPage("Login");
            }

            return Page();
        }
    }
}