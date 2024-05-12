using System.Diagnostics;
using System.Net;
using Wikipedia;

namespace WikiTests
{
    [TestClass]
    public class WikipediaTester
    {
        [TestMethod]
        public void TestGetHtml()
        {
            Debug.WriteLine("Ted Is SO Cool");
            Console.WriteLine("This is not a drill");

            WebClient client = new WebClient();
            string page = client.DownloadString("https://en.wikipedia.org/wiki/Japan");



        }

        [TestMethod]
        public void TestGetHtmlFromFile()
        {
            //retrieves filepath of WikipediaUrls.txt
            string binPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string filepath = binPath.Substring(0, binPath.Length - 23) + "WikipediaUrls.txt";
            filepath = filepath.Replace("\\", "\\\\");

            string[] lines = File.ReadAllLines(filepath);

            Console.WriteLine(lines);

            WebClient client = new WebClient();
            foreach (string line in lines)
            {
                string printLine = client.DownloadString(line);
                Console.WriteLine(printLine);

            }



        }

        [TestMethod]
        public void TestParseBasicHtml()
        {
            WikiPage page = new WikiPage("https://en.wikipedia.org/wiki/Japan");

            Console.WriteLine("no");
        }
    }
}