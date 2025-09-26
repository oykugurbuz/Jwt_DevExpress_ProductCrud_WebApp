using Microsoft.AspNetCore.SignalR;

namespace WebAppDemo.Hubs
{
    public class UserCountHub : Hub
    {
        private static int _onlineUsersCount = 0;

        public override Task OnConnectedAsync()
        {
            _onlineUsersCount++;
            Clients.All.SendAsync("UpdateUserCount", _onlineUsersCount);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _onlineUsersCount--;
            Clients.All.SendAsync("UpdateUserCount", _onlineUsersCount);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
