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
                TempData["Error"] = "M��teri ad� belirtilmedi!";
                return RedirectToPage("Login");
            }

            Customer = _context.Customers.FirstOrDefault(c => c.CustomerName == customerName);

            if (Customer == null)
            {
                TempData["Error"] = "M��teri bulunamad�!";
                return RedirectToPage("Login");
            }

            return Page();
        }
    }
}