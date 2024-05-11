namespace Wikipedia
{
    public class WikiPage
    {
        List<Tuple<string, List<int>>> sentences;
        List<string> allKeywords;
        string html;
        public WikiPage(string baseHtml) 
        {
            html = baseHtml;
            parseHtml(html);



        }

        public void parseHtml(string html) 
        {
            
        
        }
        


    }
}
