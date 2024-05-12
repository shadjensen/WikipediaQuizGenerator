using System.Diagnostics.Tracing;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Wikipedia
{
    public class WikiPage
    {
        public List<Tuple<string, List<string>>> sentencePairs;
        public List<string> allKeywords;
        public string html;
        string pageTitle;
        public WikiPage(string url) 
        {
            sentencePairs = new();
            allKeywords = new();

            WebClient client = new WebClient();

            html = client.DownloadString(url);
            
            parseHtml(html);



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

                if (paragraph.Contains($"<b>{pageTitle.Replace("_", " ")}"))
                    startIndex = index;
                index++;

                
            }

            
            for (int i = 0; i < wikiParagraphs.Length; i++) 
            {
                
                bool includeSentence = true;
                string paragraph = wikiParagraphs[i];

                if (!paragraph.StartsWith("<p>"))
                    continue;

                //checks if this is the last parsable paragraph on the page
                if (paragraph.Contains("id=\"See_also\""))
                    break;
                
                string[] paragraphSentences = paragraph.Split(".");
                foreach (string sentence in paragraphSentences) 
                {
                    if (!includeSentence)
                    {
                        includeSentence = true;
                        continue;
                    }
                    if(sentence.Length > 0 && !Regex.IsMatch(sentence[0].ToString(), "[ A-Z<]"))
                        continue;

                    StringBuilder formattedSentenceBuilder = new StringBuilder();

                    //returns formattedSentence as the sentence in plain text
                    bool inTag = false;
                    foreach (char c in sentence) 
                    {
                        if (c == '<')
                            inTag = true;
                    
                        if (!inTag)
                            formattedSentenceBuilder.Append(c);

                        if (c ==  '>')
                            inTag = false;
                    }

                    string formattedSentence = Regex.Replace(formattedSentenceBuilder.ToString(), @"&(\w+)\s", "");

                    //creates a list of link words in each sentence
                    int linkIndex = 0;
                    int indexPointer = 0;
                    List<string> keywords = new();
                    try
                    {
                        while (linkIndex < sentence.Length) 
                        {
                        
                            linkIndex = sentence.Substring(indexPointer).IndexOf("title=\"") + 7;
                            if (linkIndex >= 7)
                            {
                                int sentenceLength = sentence.Length - 1;
                                string remainingSubsentence = sentence.Substring(linkIndex + indexPointer);
                                int wordLength = remainingSubsentence.IndexOf("\"");
                                keywords.Add(sentence.Substring(linkIndex + indexPointer, wordLength));
                                indexPointer = linkIndex + wordLength + indexPointer;
                            }
                            else
                            {
                                linkIndex = sentence.Length;
                            }
                        }


                    }
                    catch 
                    {
                        //TODO:- don't throw away sentences that have periods in the middle of the links
                        includeSentence = false;
                    }

                    /**
                    string[] sentenceAsWords = formattedSentence.ToString().Split(" ");
                    List<int> keywordsAsIndexes = new();
                    
                    foreach (string keyword in keywords) 
                    {
                        int wordIndex = 0;
                        foreach (string word in sentenceAsWords) 
                        {
                            if (word.Equals(keyword, StringComparison.OrdinalIgnoreCase))
                                keywordsAsIndexes.Add(wordIndex);
                            wordIndex++;
                        }

                        
                    }
                    **/
                    


                    if (includeSentence && formattedSentenceBuilder.ToString().Length > 0)
                        sentencePairs.Add(new Tuple<string, List<string>>(formattedSentence, keywords));
                                      

                }

            }
        }
    }
}
