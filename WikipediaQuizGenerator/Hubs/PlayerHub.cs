
using Microsoft.AspNetCore.SignalR;
using WikipediaQuizGenerator.Services;
using Wikipedia;

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
            PlayerDataServices? playerDataServices = serviceProvider.GetService<PlayerDataServices>();
            if (playerDataServices != null)
                playerDataServices.ServerHost = Clients.Caller;
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
                await playerDataServices.ServerHost.SendAsync("LobbyJoined", username);
            }
            else
                await Clients.Caller.SendAsync("LobbyJoined", "Could not resolve playerDataServices");
        }

        public async Task SendPageTitle() 
        {
            PlayerDataServices? playerDataServices = serviceProvider.GetService<PlayerDataServices>();
            if (playerDataServices != null) 
            {
                string pageName = playerDataServices.generatePageName();
                await Clients.Caller.SendAsync("RecievePageTitle", pageName);
            
            }
        
        }

        public async Task SendQuestion(string pageTitle)
        {
            PlayerDataServices? playerDataServices = serviceProvider.GetService<PlayerDataServices>();
            if (playerDataServices != null)
            {
                playerDataServices.generateQuestion(pageTitle, out string formattedQuestion, out string formattedAnswer, out string[] questionOptions, out string answer);

                await Clients.All.SendAsync("RecieveQuestionClient", questionOptions);
                await playerDataServices.ServerHost.SendAsync("RecieveQuestionHost", formattedQuestion, formattedAnswer, answer);
            }

        }

        public async void RecieveAnswer(string username, string answer) 
        {
            PlayerDataServices? playerDataServices = serviceProvider.GetService<PlayerDataServices>();
            if (playerDataServices != null) 
            {
                //send true, and an updated score if the answer was correct. Otherwise send false and the current score
                if (playerDataServices.currentQuestionAnswer.Equals(answer))
                {
                    playerDataServices.PlayerScores[username]++;
                    await Clients.Caller.SendAsync("SendClientScore", true, playerDataServices.PlayerScores[username]);
                }
                else 
                {                    
                    await Clients.Caller.SendAsync("SendClientScore", false, playerDataServices.PlayerScores[username]);
                }


            }

        }







    }
}
