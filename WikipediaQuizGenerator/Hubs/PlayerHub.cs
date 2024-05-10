
using Microsoft.AspNetCore.SignalR;

namespace WikipediaQuizGenerator.Hubs
{
    public class PlayerHub : Hub
    {
        private readonly IServiceProvider serviceprovider;

        public PlayerHub(IServiceProvider _serviceProvider) 
        {
            serviceprovider = _serviceProvider;
        }

        public async Task SendMessage(string user, string message) 
        {
            await Clients.All.SendAsync("RecieveMessage", user, message);
        
        }

        public async Task JoinLobby(string username, string userCountry) 
        {
            PlayerDataService playerDataService = (PlayerDataService)serviceprovider.GetService(typeof(PlayerDataService));
            
            if (playerDataService != null) 
            {
                playerDataService.Players.Add(username, userCountry);
            
            
            
            }

            await Clients.Caller.SendAsync("LobbyJoined", username, userCountry);
        }


    }
}
