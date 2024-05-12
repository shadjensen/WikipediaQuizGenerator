using Wikipedia;

namespace WikipediaQuizGenerator.Services
{
    public class PlayerDataServices
    {
        /**
         * Because this game isn't intended to be played with hundreds or thousands of players, we'll sort data 
         * by playerName and just keep it in a list. It would slow the code with a high player count but it's unlikely
         * this code will ever be played with the kind of numbers that justify refactoring
         **/
        public List<string> Players { get; set; } = new List<string>();
        public Dictionary<string, string> PlayerCountries { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, int> PlayerScores { get; set; } = new Dictionary<string, int>();
        public List<WikiPage> allWikiPages { get; set; } = new List<WikiPage>();

        public PlayerDataServices()
        {

        }

        public bool LoadWikiPages(string filename) 
        {
            try
            {
                string binPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                string filepath = binPath.Substring(0, binPath.Length - 26) + filename;

                string[] urls = File.ReadAllLines(filepath);
                foreach(string url in urls) 
                {
                    allWikiPages.Add(new WikiPage(url));                
                }
                return true;
            }
            catch 
            {
                return false;
            }

        
        }


    }
}
