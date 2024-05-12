
using Microsoft.AspNetCore.SignalR;
using WikipediaQuizGenerator.Services;

namespace WikipediaQuizGenerator.Hubs
{
    
    public class PlayerHub : Hub
    {
        private readonly IServiceProvider serviceProvider;
        private ISingleClientProxy host;
        public PlayerHub(IServiceProvider _serviceProvider) 
        {
            serviceProvider = _serviceProvider;
        }

        public void DeclareHost() 
        {
            host = Clients.Caller;
        }

        public async Task SendMessage(string user, string message) 
        {
            await Clients.All.SendAsync("RecieveMessage", user, message);
        
        }

        public async Task JoinLobby(string username, string userCountry) 
        {
            PlayerDataServices? playerDataServices = serviceProvider.GetService<PlayerDataServices>();
        
            if (playerDataServices != null) 
            {
                //we add the player to our three libraries, and set their initial score to zero
                playerDataServices.Players.Add(username);
                playerDataServices.PlayerCountries.Add(username, userCountry);
                playerDataServices.PlayerScores.Add(username, 0);

                await Clients.Caller.SendAsync("LobbyJoined", username);
                await host.SendAsync("LobbyJoined", username);
            }
            else
                await Clients.Caller.SendAsync("LobbyJoined", "Could not resolve playerDataServices");
        }

        public async Task SendNextQuestion(string[] keywords) 
        {

            await Clients.All.SendAsync("RecieveAnswerOptions", keywords);
        
        }

        

        


    }
}
