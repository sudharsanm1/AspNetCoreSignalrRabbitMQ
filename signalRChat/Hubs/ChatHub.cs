using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {

        private static List<User> Users = new List<User>();

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override Task OnConnectedAsync()
        {
            User user = new User() { Name = Context.User.Identity.Name, ConnectionID = Context.ConnectionId };
            Users.Add(user);
            //Clients.Others.userConnected(user.Name);
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception ex)
        {
            if (Users.Any(x => x.ConnectionID == Context.ConnectionId))
            {
                User user = Users.First(x => x.ConnectionID == Context.ConnectionId);
                //Clients.Others.userLeft(user.Name);   
                Users.Remove(user);
            }
            return base.OnDisconnectedAsync(ex);
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}