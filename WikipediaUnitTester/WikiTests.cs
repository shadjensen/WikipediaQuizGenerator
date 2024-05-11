using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using Wikipedia;


namespace WikipediaUnitTester
{
    [TestClass]
    public class WikiTests
    {
        [TestMethod]
        public void TestGetHtml ()
        {
            Debug.WriteLine("Ted Is SO Cool");
            Console.WriteLine("This is not a drill");

            WebClient client = new WebClient ();
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
            
            WebClient client = new WebClient ();
            foreach (string line in lines) 
            {
                string printLine = client.DownloadString(line);
                Console.WriteLine (printLine);
            
            }



        }

        [TestMethod]
        public void TestParseBasicHtml() 
        {
            WikiPage page = new WikiPage();
        
            
        }
    }
}
