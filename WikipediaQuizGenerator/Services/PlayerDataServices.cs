using Microsoft.AspNetCore.SignalR;
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
        public Dictionary<string, WikiPage> allWikiPages { get; set; } = new Dictionary<string, WikiPage>();
        public ISingleClientProxy ServerHost { get; set; }
        public string currentPageTitle { get; set; }
        public string currentQuestionAnswer { get; set; }
        private int numberOfQuestionOptions { get; set; } = 8;
        public bool recievingScores { get; set; } = false;


        public PlayerDataServices()
        {

            LoadWikiPages("WikipediaUrls.txt");
        }

        public bool LoadWikiPages(string filename) 
        {
            try
            {
                string binPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                string filepath = binPath.Substring(0, binPath.Length - 39) + filename;

                string[] urls = File.ReadAllLines(filepath);
                foreach(string url in urls) 
                {
                    WikiPage page = new WikiPage(url);
                    allWikiPages.Add(page.pageTitle, page);                
                }
                return true;
            }
            catch 
            {
                return false;
            }

        
        }

        public string generatePageName() 
        {
            Random random = new Random();
            List<KeyValuePair<string, WikiPage>> pagesAsList = allWikiPages.ToList();
            currentPageTitle = pagesAsList[random.Next(pagesAsList.Count)].Key;
            return currentPageTitle;
        }

        public void generateQuestion(string pageName, out string formattedQuestion, out string formattedAnswer, out string[] questionOptions, out string answer) 
        {
            WikiPage wikiPage = allWikiPages[pageName];
            Random random = new Random();

            //grab sentence to use as question
            bool validSentence = false;
            int pageIndex = random.Next(wikiPage.sentencePairs.Count);
            //this is assigned to help the compiler, but will be overwritten in the loop so we can pick a random sentence
            Tuple<string, List<string>> sentencePair = wikiPage.sentencePairs[0];
            while (!validSentence) 
            {
                sentencePair = wikiPage.sentencePairs[pageIndex];
                //only keep the sentence if there are at least two keywords in the sentence. This
                //prevents the algorithm from picking "sentences" that are just keywords or getting
                //sentences too short or incomplete to really use
                if (sentencePair.Item2.Count >= 2)
                {
                    validSentence = true;
                }
                else
                    pageIndex = random.Next(wikiPage.sentencePairs.Count);
            }

            
            answer = sentencePair.Item2[random.Next(sentencePair.Item2.Count)];
            currentQuestionAnswer = answer;
            formattedQuestion = sentencePair.Item1.Replace(answer, "________", StringComparison.OrdinalIgnoreCase);
            formattedAnswer = sentencePair.Item1;


            //select enough options for players to choose, minus one so we can ensure one is the answer
            string[] options = new string[numberOfQuestionOptions];
            int answerIndex = wikiPage.allKeywords.IndexOf(answer);
            for (int i = 0; i < numberOfQuestionOptions-1; i++) 
            {
                bool foundWord = false;
                while (!foundWord) {

                    /**
                     * we want to pull options from surrounding sentences, so we grab keywords with an an adjustable pull range. This range is X keywords before and after 
                     * our answer's index. It's important to remember that many sentences including our answer sentence will include multiple keywords so having too short of
                     * a range will only pull from a few sentences. A sentence might have 8 or 9 keywords in it, so even a pull range of 20 might actually only span 4 sentences.
                     * 
                     * It's not horrible if words that appear in the sentence appear as keywords, but it's not ideal as context often allows you to rule those out. To remedy this we
                     * adjust the pull range based on how many key words are in the answe sentence. It's ideal if the range is as small as possible while still maximing the chance
                     * the words don't appear in the base sentence. This can be adjusted and fine tuned.
                    **/
                    int pullRange = sentencePair.Item2.Count * 5;
                    int optionIndex = random.Next(pullRange * 2) - pullRange + answerIndex;
                    while ((optionIndex < 0) || (optionIndex >= wikiPage.allKeywords.Count) || (optionIndex == answerIndex))
                            optionIndex = random.Next(20) - 10 + answerIndex;

                    string option = wikiPage.allKeywords[optionIndex];

                    if (!options.Contains(option) && (!option.Equals(answer))) 
                    {
                        options[i] = option;
                        foundWord = true;
                    }


                    /**

                    //string option = wikiPage.allKeywords[random.Next(wikiPage.allKeywords.Count)];
                    int chosenSentenceIndex = 0;
                    while ((chosenSentenceIndex == 0) || (chosenSentenceIndex < -1) || (chosenSentenceIndex > wikiPage.sentencePairs.Count))
                        chosenSentenceIndex = pageIndex + random.Next(12) - 6;

                    Tuple<string, List<string>> chosenSentence = wikiPage.sentencePairs[chosenSentenceIndex];

                    string option = chosenSentence.Item2[random.Next(chosenSentence.Item2.Count)];


                    //ensures there are no duplicates within the options array
                    if (!options.Contains(option) && !option.Equals(answer) || chosenSentence.Item1.Contains(option)) 
                    {
                        options[i] = option;
                        foundWord = true;
                    }

                    **/
                }
            }
            //fill the last slot with the correct answer
            options[numberOfQuestionOptions - 1] = answer;

            questionOptions = options;
        
        }


    }
}
