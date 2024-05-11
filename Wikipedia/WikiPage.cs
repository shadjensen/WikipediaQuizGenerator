using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Wikipedia
{
    public class WikiPage
    {
        public List<Tuple<string, List<int>>> sentences;
        public List<string> allKeywords;
        public string html;
        string pageTitle;
        public WikiPage() 
        {
            //WebClient client = new WebClient();

            //html = client.DownloadString(url);
            
            //parseHtml(html);



        }

        public void parseHtml(string html) 
        {
            pageTitle = "Japan";

            string[] wikiParagraphs = html.Split("\n");
            int index = 0;
            //this indicates the first index of actual text on the page, rather than the formatting and headers that often begin Wiki pages
            int startIndex = 0;
            foreach (string paragraph in wikiParagraphs) 
            {
                if (startIndex != 0)
                    break;

                if (paragraph.Substring(0, 50).Contains($"<b>{pageTitle.Replace("_", " ")}"))
                    startIndex = index;
                index++;

                
            }

            
            for (int i = startIndex; i < wikiParagraphs.Length; i++) 
            {
                string paragraph = wikiParagraphs[i];
                
                //checks if this is the last parsable paragraph on the page
                if (paragraph.Substring(0, 50).Contains("id=\"See_also\""))
                    break;
                
                string[] paragraphSentences = paragraph.Split(".");
                StringBuilder formattedSentence = new StringBuilder();
                foreach (string sentence in paragraphSentences) 
                {

                    //returns formattedSentence as the sentence in plain text
                    bool inTag = false;
                    foreach (char c in sentence) 
                    {
                        if (c == '<')
                            inTag = true;
                    
                        if (!inTag)
                            formattedSentence.Append(c);

                        if (c ==  '>')
                            inTag = false;
                    }


                    //creates a list of link words in each sentence
                    int linkIndex = 0;
                    List<string> keywords = new();
                while (linkIndex < sentence.Length) 
                    {
                        
                        linkIndex = sentence.IndexOf("title=\"") + 7;
                        if (linkIndex >= 0)
                        {
                            int sentenceLength = sentence.Length-1;
                            int wordLength = sentence.Substring(linkIndex, sentence.Length-1-linkIndex).IndexOf("\"");
                            keywords.Add(sentence.Substring(linkIndex, wordLength));
                            
                        }
                        else 
                        {
                            linkIndex = sentence.Length;
                        }
                    }


                }


            }
        }
        


    }
}
