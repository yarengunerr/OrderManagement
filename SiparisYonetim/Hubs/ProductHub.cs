using Microsoft.AspNetCore.SignalR;

namespace SiparisYonetim.Hubs
{
    public class ProductHub : Hub
    {


        public async Task SendUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveUpdate", message);
        }

        
        public async Task SendOrderUpdate()
        {
            
            await Clients.All.SendAsync("ReceiveUpdate", "Yeni bir sipariş onaylandı!");
        }

        public async Task SendNewOrders(string ordersJson)
        {
            await Clients.All.SendAsync("ReceiveNewOrders", ordersJson);
        }

        public override async Task OnConnectedAsync()
        {
            
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("Connected", "SignalR bağlantısı başarılı.");
        }
    }
}
