using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Server.Hubs
{
    public class ServersHub : Hub
    {
        public Task SendMessage(string user, string message)
        {
            return Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(Context.ConnectionId + " has joined us!");
            await base.OnConnectedAsync();
        }
    }
}
