
using Microsoft.AspNetCore.SignalR;
using WikipediaQuizGenerator.Services;
using Wikipedia;

namespace WikipediaQuizGenerator.Hubs
{

    public class PlayerHub : Hub
    {
        private readonly IServiceProvider serviceProvider;
        private ISingleClientProxy host;

        /// <summary>
        /// constructs a PlayerHub. Requires an IServiceProvider to function
        /// </summary>
        /// <param name="_serviceProvider"></param>
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

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Sends the Wikipedia page title to the saved host player
        /// </summary>
        /// <returns></returns>
        public async Task SendPageTitle() 
        {
            PlayerDataServices? playerDataServices = serviceProvider.GetService<PlayerDataServices>();
            if (playerDataServices != null) 
            {
                string pageName = playerDataServices.generatePageName();
                await Clients.Caller.SendAsync("RecievePageTitle", pageName);
            
            }
        
        }

        /// <summary>
        /// Given a page title, requests a question from that wikipedia page. A question consists of one string containing a sentence with a word or phrase
        /// replaced with "_____", and 8 strings representing potential answers which could correctly replace the "_____" to complete the sentence. A question also
        /// consists of one string representing the correct phrase which is passed to both the client and the host, and a sentence index number which indicates where in the 
        /// wikipedia page this sentence is taken from. This number is later used so the host can display the context (preceeding and proceeding sentences) to the question.
        /// </summary>
        /// <param name="pageTitle">The title of the wikipedia page from which the question is taken</param>
        /// <returns></returns>
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

        /// <summary>
        /// Given a wikipedia page and an integer representing a desired sentence's index within that page, sends to the host the sentence before and after
        /// the desired sentence in the wikipedia page. If the desired sentence is the first or last sentence in the page, only one sentence will be sent.
        /// 
        /// If the sentenceIndex does not exist, an error will be thrown.
        /// </summary>
        /// <param name="pageTitle">The title of the Wikipedia page being queried</param>
        /// <param name="sentenceIndex">The int index of a sentence within the wikipedia page.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Sends the scores of every player to every player. This is sente as a list of Key Value pairs in which the key
        /// is the player's name and the value is their score.
        /// </summary>
        /// <returns></returns>
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


        ///SHOULD BE REMOVED AS THE ACTUAL CODE IS THE SAME AS SENDSCORES(). WILL REQUIRE MINOR REFACTORING
        /// <summary>
        /// Sends the scores of every player to every player. This is sente as a list of Key Value pairs in which the key
        /// is the player's name and the value is their score.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Called after a player submits an answer, the server checks if that answer is correct and updates scores accordingly as well as 
        /// communicates whether or not it was correct to each player. This call is not used by the host.
        /// </summary>
        /// <param name="username">The name of the player</param>
        /// <param name="answer">the player's selected answer</param>
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

        /// <summary>
        /// given a url, generates a QR code. This QR code is saved in the Client's wwwroot/images folder as qrcode.png. This code calls GenerateQRCode 
        /// from a QRCodeService object, meaning if an error occurs it is either with retrieving the QRCodeService or the call within that object.
        /// </summary>
        /// <param name="httpAddress"></param>
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
