using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json; 
using SiparisYonetim.Data;
using SiparisYonetim.Hubs;
using SiparisYonetim.Models;

namespace SiparisYonetim.Pages
{
    public class AdminPanelModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ProductHub> _hubContext;
        private static readonly SemaphoreSlim _semaphore = new(1, 1); 

        public AdminPanelModel(AppDbContext context, IHubContext<ProductHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public List<Product> Products { get; set; } = new();
        public List<Order> Orders { get; set; } = new(); 

        public List<Log> Logs { get; set; } = new();

        [BindProperty]
        public Product NewProduct { get; set; }

        public void OnGet()
        {
            
            Products = _context.Products.ToList();
            Orders = _context.Orders
                .Where(o => o.OrderStatus == "Bekliyor")  
                .Include(o => o.Product)  
                .ToList();

            Logs = _context.Logs.OrderByDescending(l => l.LogDate).Take(10).ToList();
        }

        public async Task<IActionResult> OnPostConfirmAllOrdersAsync()
        {
            await _semaphore.WaitAsync(); 
            try
            {
                
                var pendingOrders = _context.Orders.Where(o => o.OrderStatus == "Bekliyor").ToList();

                
                foreach (var order in pendingOrders)
                {
                    
                    order.OrderStatus = "Onayland�";

                    
                    var log = new Log
                    {
                        CustomerID = order.CustomerID,
                        OrderID = order.OrderID,
                        LogDate = DateTime.Now,
                        LogType = "Onayland�",  
                        LogDetails = $"Sipari� {order.OrderID} onayland�."
                    };
                    _context.Logs.Add(log);

                    
                    
                    var confirmedOrder = new Order
                    {
                        CustomerID = order.CustomerID,
                        ProductID = order.ProductID,
                        Quantity = order.Quantity,
                        TotalPrice = order.TotalPrice,
                        OrderDate = DateTime.Now, 
                        OrderStatus = order.OrderStatus
                    };

                    
                    _context.Orders.Add(confirmedOrder);
                }

                await _context.SaveChangesAsync();

               
                var updatedOrders = _context.Orders.ToList();
                var json = JsonConvert.SerializeObject(updatedOrders);
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate", json);
                await _hubContext.Clients.All.SendAsync("ReceiveNewOrders", json);

                TempData["SuccessMessage"] = "T�m sipari�ler ba�ar�yla onayland�!";
            }
            finally
            {
                _semaphore.Release(); 
            }

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostCancelAllOrdersAsync()
        {
            await _semaphore.WaitAsync(); 
            try
            {
                
                var allOrders = _context.Orders.ToList();

                foreach (var order in allOrders)
                {
                    
                    order.OrderStatus = "�ptal Edildi";

                    
                    var log = new Log
                    {
                        CustomerID = order.CustomerID,
                        OrderID = order.OrderID,
                        LogDate = DateTime.Now,
                        LogType = "�ptal Edildi",  
                        LogDetails = $"Sipari� {order.OrderID} iptal edildi."
                    };
                    _context.Logs.Add(log);
                }

                
                _context.Orders.RemoveRange(allOrders);
                await _context.SaveChangesAsync();

                
                var updatedOrders = _context.Orders.ToList(); 
                var json = JsonConvert.SerializeObject(updatedOrders);
                await _hubContext.Clients.All.SendAsync("ReceiveNewOrders", json);

                TempData["SuccessMessage"] = "T�m sipari�ler ba�ar�yla iptal edildi!";
            }
            finally
            {
                _semaphore.Release(); 
            }

            return RedirectToPage();
        }




        public async Task<IActionResult> OnPostAddProductAsync()
        {
            if (!ModelState.IsValid) return Page();

            await _semaphore.WaitAsync(); 
            try
            {
                
                _context.Products.Add(NewProduct);
                await _context.SaveChangesAsync();

                
                var updatedProducts = _context.Products.ToList();
                var json = JsonConvert.SerializeObject(updatedProducts);
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate", json);

                TempData["SuccessMessage"] = "Yeni �r�n ba�ar�yla eklendi!";
            }
            finally
            {
                _semaphore.Release(); 
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateStockAsync(int id, int amount)
        {
            await _semaphore.WaitAsync(); 
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product != null)
                {
                    
                    product.Stock = amount;
                    await _context.SaveChangesAsync();

                    
                    var updatedProducts = _context.Products.ToList();
                    var json = JsonConvert.SerializeObject(updatedProducts);
                    await _hubContext.Clients.All.SendAsync("ReceiveUpdate", json);
                }
            }
            finally
            {
                _semaphore.Release(); 
            }

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostDeleteProductAsync(int id)
        {
            await _semaphore.WaitAsync(); 
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product != null)
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();

                    
                    var updatedProducts = _context.Products.ToList();
                    var json = JsonConvert.SerializeObject(updatedProducts);
                    await _hubContext.Clients.All.SendAsync("ReceiveUpdate", json);
                }
            }
            finally
            {
                _semaphore.Release(); 
            }

            return RedirectToPage();
        }
    }

}
