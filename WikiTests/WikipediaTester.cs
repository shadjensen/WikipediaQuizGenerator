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
            WikiPage page = new WikiPage();
            page.html = "<p><b>Japan</b><sup id=\"cite_ref-11\" class=\"reference\"><a href=\"#cite_note-11\">&#91;a&#93;</a></sup> is an <a href=\"/wiki/Island_country\" title=\"Island country\">island country</a> in <a href=\"/wiki/East_Asia\" title=\"East Asia\">East Asia</a>. It is in the northwest <a href=\"/wiki/Pacific_Ocean\" title=\"Pacific Ocean\">Pacific Ocean</a> and is bordered on the west by the <a href=\"/wiki/Sea_of_Japan\" title=\"Sea of Japan\">Sea of Japan</a>, extending from the <a href=\"/wiki/Sea_of_Okhotsk\" title=\"Sea of Okhotsk\">Sea of Okhotsk</a> in the north toward the <a href=\"/wiki/East_China_Sea\" title=\"East China Sea\">East China Sea</a>, <a href=\"/wiki/Philippine_Sea\" title=\"Philippine Sea\">Philippine Sea</a>, and <a href=\"/wiki/Taiwan\" title=\"Taiwan\">Taiwan</a> in the south. Japan is a part of the <a href=\"/wiki/Ring_of_Fire\" title=\"Ring of Fire\">Ring of Fire</a>, and spans <a href=\"/wiki/Japanese_archipelago\" title=\"Japanese archipelago\">an archipelago</a> of <a href=\"/wiki/List_of_islands_of_Japan\" title=\"List of islands of Japan\">14,125 islands</a>, with the five main islands being <a href=\"/wiki/Hokkaido\" title=\"Hokkaido\">Hokkaido</a>, <a href=\"/wiki/Honshu\" title=\"Honshu\">Honshu</a> (the \"mainland\"), <a href=\"/wiki/Shikoku\" title=\"Shikoku\">Shikoku</a>, <a href=\"/wiki/Kyushu\" title=\"Kyushu\">Kyushu</a>, and <a href=\"/wiki/Okinawa_Island\" title=\"Okinawa Island\">Okinawa</a>. <a href=\"/wiki/Tokyo\" title=\"Tokyo\">Tokyo</a> is <a href=\"/wiki/Capital_of_Japan\" title=\"Capital of Japan\">the country's capital</a> and <a href=\"/wiki/Largest_cities_in_Japan_by_population_by_decade\" title=\"Largest cities in Japan by population by decade\">largest city</a>, followed by <a href=\"/wiki/Yokohama\" title=\"Yokohama\">Yokohama</a>, <a href=\"/wiki/Osaka\" title=\"Osaka\">Osaka</a>, <a href=\"/wiki/Nagoya\" title=\"Nagoya\">Nagoya</a>, <a href=\"/wiki/Sapporo\" title=\"Sapporo\">Sapporo</a>, <a href=\"/wiki/Fukuoka\" title=\"Fukuoka\">Fukuoka</a>, <a href=\"/wiki/Kobe\" title=\"Kobe\">Kobe</a>, and <a href=\"/wiki/Kyoto\" title=\"Kyoto\">Kyoto</a>.";
            page.parseHtml(page.html);

        }
    }
}