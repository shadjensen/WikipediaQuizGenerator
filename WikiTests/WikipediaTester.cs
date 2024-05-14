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
        public void TestReadFile()
        {
            //retrieves filepath of WikipediaUrls.txt

            string binPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string filepath = binPath.Substring(0, binPath.Length - 26) + "WikipediaUrls.txt";
            filepath = @"C:\Users\jense\source\repos\PersonalProjects\WikipediaQuizGenerator\WikipediaUrls.txt";

            string[] urls = File.ReadAllLines(filepath);


            WebClient client = new WebClient();
            Assert.IsTrue(urls.Contains("https://en.wikipedia.org/wiki/Japan"));
            Assert.IsTrue(urls.Contains("https://en.wikipedia.org/wiki/Germany"));


        }

        [TestMethod]
        public void TestParseBasicHtml()
        {
            WikiPage page = new WikiPage("https://en.wikipedia.org/wiki/Japan");

            Console.WriteLine("no");
        }

        [TestMethod]
        public void TestGeneratePageTitleOnePage() 
        {
            WikiPage page = new WikiPage("https://en.wikipedia.org/wiki/Japan");
            


        }
    }
}