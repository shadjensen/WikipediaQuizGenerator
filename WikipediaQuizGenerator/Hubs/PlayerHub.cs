﻿
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

        /// <summary>
        /// This method is called when a host is selected. It saves one connection as the host through which most controlling calls will be sent.
        /// If this method is called when a host is already delcared, it will override the first declaration allowing two clients to act as host but only one
        /// will recieve back the host calls
        /// </summary>
        public void DeclareHost()
        {
            PlayerDataServices? playerDataServices = serviceProvider.GetService<PlayerDataServices>();
            if (playerDataServices != null)
                playerDataServices.ServerHost = Clients.Caller;
        }

        public override async Task OnDisconnectedAsync(Exception exception) 
        {
            var userId = Context.UserIdentifier;

            //check if the disconnection was the host. If so, declare the host null in playerDataServices
            PlayerDataServices? playerDataServices = serviceProvider.GetService<PlayerDataServices>();
            if (playerDataServices != null) 
            { 
                
                //-TODO:check for host disconnection

            }

        }

        /// <summary>
        /// Registers a player in the lobby.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task JoinLobby(string username)
        {
            PlayerDataServices? playerDataServices = serviceProvider.GetService<PlayerDataServices>();

            if (playerDataServices != null)
            {
                if (playerDataServices.ServerHost != null)
                {

                    //we add the player to our three libraries, and set their initial score to zero
                    playerDataServices.Players.Add(username);
                    playerDataServices.PlayerScores.Add(username, 0);

                    await Clients.Caller.SendAsync("LobbyJoined", username);
                    await playerDataServices.ServerHost.SendAsync("LobbyJoined", username);
                }
                else 
                {
                    await Clients.Caller.SendAsync("NoLobbyHost", username);
                }
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
                playerDataServices.generateQuestion(pageTitle, out string formattedQuestion, out string formattedAnswer, out string[] questionOptions, out string answer, out int sentenceIndex);
                playerDataServices.recievingScores = true;

                await Clients.All.SendAsync("RecieveQuestionClient", questionOptions, answer);
                await playerDataServices.ServerHost.SendAsync("RecieveQuestionHost", formattedQuestion, formattedAnswer, answer, sentenceIndex);
            }

        }

        public async Task SendQuestionContext(string pageTitle, int sentenceIndex) 
        {
            PlayerDataServices? playerDataServices = serviceProvider.GetService<PlayerDataServices>();
            if (playerDataServices != null)
            {
                WikiPage page = playerDataServices.allWikiPages[pageTitle];

                //the sentence before our answer
                string preceedingSentence ="";
                //the sentence after our answer
                string proceedingSentence ="";
                if (sentenceIndex > 0)
                    preceedingSentence = page.sentencePairs[sentenceIndex - 1].Item1;
                if (sentenceIndex < page.sentencePairs.Count - 2)
                    proceedingSentence = page.sentencePairs[sentenceIndex + 1].Item1;

                await playerDataServices.ServerHost.SendAsync("RecieveQuestionContext", preceedingSentence, proceedingSentence);


            }
        }

        public async Task SendScores() 
        {
            PlayerDataServices? playerDataServices = serviceProvider.GetService<PlayerDataServices>();
            if (playerDataServices != null) 
            {
                List<KeyValuePair<string, int>> scoreList = playerDataServices.PlayerScores.ToList();
                await playerDataServices.ServerHost.SendAsync("RecieveScores", scoreList);
                await Clients.All.SendAsync("ShowClientScores", null);
            
            
            }


        }

        public async Task SendFinalScores() 
        {
            PlayerDataServices? playerDataServices = serviceProvider.GetService<PlayerDataServices>();
            if (playerDataServices != null)
            {
                List<KeyValuePair<string, int>> scoreList = playerDataServices.PlayerScores.ToList();
                await playerDataServices.ServerHost.SendAsync("RecieveScores", scoreList);
                await Clients.All.SendAsync("ShowClientFinalScore", scoreList);


            }

        }

        public async void SendAnswer(string username, string answer) 
        {
            PlayerDataServices? playerDataServices = serviceProvider.GetService<PlayerDataServices>();
            if (playerDataServices != null) 
            {
                //send true, and an updated score if the answer was correct. Otherwise send false and the current score
                if (playerDataServices.currentQuestionAnswer.Equals(answer))
                {
                    playerDataServices.playerAnswerOrder.Add(username);
                    playerDataServices.PlayerScores[username] += calculateScore((playerDataServices.playerAnswerOrder.IndexOf(username) + 1));

                    await Clients.Caller.SendAsync("RecieveClientScore", true, playerDataServices.PlayerScores[username]);
                }
                else 
                {                    
                    await Clients.Caller.SendAsync("RecieveClientScore", false, playerDataServices.PlayerScores[username]);
                }

                await playerDataServices.ServerHost.SendAsync("UpdateClientAnswerCount");


            }

        }

        public async void GenerateQRCode(string httpAddress) 
        {
            QRCodeService? qrcodeservice = serviceProvider.GetService<QRCodeService>();
            PlayerDataServices? playerDataServices = serviceProvider.GetService<PlayerDataServices>();
            if (qrcodeservice != null) 
            {
                string qrID = qrcodeservice.GenerateQrCode(httpAddress);
                
                if (playerDataServices != null) 
                {
                    await playerDataServices.ServerHost.SendAsync("RecieveQRCode", qrID);
                }
                
            }

        }

        /// <summary>
        /// Calculates the score gain of a player given the order in which they answered a question. This order is only counted among players that answered correctly.
        /// The system for scoring is as follows:
        /// 
        /// The first player to answer correctly recieves 100 points, the next 90, the next 80 and so on. Any scores less than 50 will just be capped at 50 instead so answer number
        /// 6 and on will all recieve the same number of points regardless of what order they answered.
        /// </summary>
        /// <param name="answerOrder"></param>
        /// <returns></returns>
        private int calculateScore(int answerOrder) 
        {
            int score = 100;

            score -= 10 * (answerOrder - 1);

            if (score > 50)
                return score;
            return 50;
        }







    }
}
