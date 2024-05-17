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
        public string pageTitle;
        /// <summary>
        /// Creates a new Wiki page object from a string url. This url should be in the format https://en.wikipedia.org/wiki/ + the title of the page.
        /// </summary>
        /// <param name="url"></param>
        public WikiPage(string url) 
        {
            sentencePairs = new();
            allKeywords = new();

            WebClient client = new WebClient();

            html = client.DownloadString(url);
            this.pageTitle = url.Substring(30);

            parseHtml(html);



        }

        public void parseHtml(string html) 
        {
            html = Regex.Replace(html, @"(?<=\d)\.(?=\d)|(?<=[A-Z])\.", "_");

            string[] wikiParagraphs = html.Split("\n");


            
            for (int i = 0; i < wikiParagraphs.Length; i++) 
            {
                
                bool includeSentence = true;
                string paragraph = wikiParagraphs[i];

                if (!paragraph.StartsWith("<p>"))
                    continue;

                
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


                    //removes html tags that start with &# and end with ; and a space or just ;
                    Regex regex = new Regex(@"&#.*?;\s");
                    string formattedSentence = regex.Replace(formattedSentenceBuilder.ToString(), " ");
                    formattedSentence = Regex.Replace(formattedSentence, @"&#.*?;", "") + ".";


                    //creates a list of link words in each sentence
                    int linkIndex = 0;
                    int indexPointer = 0;
                    List<string> keywords = new();
                    try
                    {
                        while (linkIndex < sentence.Length) 
                        {
                        
                            linkIndex = sentence.Substring(indexPointer).IndexOf("title=\"") + 7;
                            //the base return is -1 if no sample string is found, so if the link index is less than or equal to 7 none is found as 7 is the addition from "title="
                            if (linkIndex >= 7)
                            {
                                //this finds the index of the close carrot after the index of title= and adds one so the word does not include the >
                                linkIndex = sentence.Substring(indexPointer + linkIndex).IndexOf(">") + linkIndex + 1;
                                int sentenceLength = sentence.Length - 1;
                                string remainingSubsentence = sentence.Substring(linkIndex + indexPointer);
                                int wordLength = remainingSubsentence.IndexOf("<") ;
                                if (wordLength > 0)
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




                    //only add sentences to our list if they are nonempty and they haven't caused errors
                    if (includeSentence && formattedSentenceBuilder.ToString().Length > 0)
                    {
                        sentencePairs.Add(new Tuple<string, List<string>>(formattedSentence, keywords));
                        foreach(string keyword in keywords)
                            allKeywords.Add(keyword);
                    }
                                      

                }

            }
        }
    }
}
