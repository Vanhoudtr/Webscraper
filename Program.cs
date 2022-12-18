using System;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WebScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            // Prompt the user for the search term
            Console.Write("Enter the search term: ");
            var searchTerm = Console.ReadLine();

            // Scrape YouTube data
            ScrapeYouTubeData(searchTerm);

            // Prompt the user for the ICT job search term
            Console.Write("Enter the ICT job search term: ");
            searchTerm = Console.ReadLine();

            // Scrape ICT job data
            string[] data = ScrapeData(searchTerm);

            // Print the extracted ICT job data to the console
            for (int i = 0; i < data.Length / 5; i++)
            {
                Console.WriteLine($"Title: {data[i * 5 + 0]}");
                Console.WriteLine($"Company: {data[i * 5 + 1]}");
                Console.WriteLine($"Location: {data[i * 5 + 2]}");
                Console.WriteLine($"Keywords: {data[i * 5 + 3]}");
                Console.WriteLine($"Link: {data[i * 5 + 4]}");
                Console.WriteLine();
            }
        }
        private static string[] ScrapeData(string searchTerm)
        {
            // Set up Chrome driver
            var driver = new ChromeDriver("chromedriver");

            // Navigate to the ICT job search page
            driver.Navigate().GoToUrl("https://www.ictjob.be/nl/it-vacatures-zoeken?keywords=" + searchTerm);

            // Set timeout
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            // Find the elements containing the job information
            var elements = driver.FindElements(By.XPath("//*[@class='job-listing']"));

            // Initialize the array to hold the extracted data
            string[] data = new string[elements.Count * 5];

            // Loop through the elements and extract the information
            int i = 0;
            foreach (var element in elements)
            {
                // Extract the title of the job
                data[i * 5 + 0] = element.FindElement(By.XPath(".//*[@class='job-title']")).Text;

                // Extract the name of the company
                data[i * 5 + 1] = element.FindElement(By.XPath(".//*[@class='company-name']")).Text;

                // Extract the location of the job
                data[i * 5 + 2] = element.FindElement(By.XPath(".//*[@class='location']")).Text;

                // Extract the keywords for the job
                data[i * 5 + 3] = element.FindElement(By.XPath(".//*[@class='keywords']")).Text;

                // Extract the link to the job listing
                data[i * 5 + 4] = element.FindElement(By.XPath(".//*[@class='job-title']")).GetAttribute("href");

                i++;
            }

            return data;
        }


        private static void ScrapeYouTubeData(string searchTerm)
        {
            // Set up Chrome driver
            var driver = new ChromeDriver("chromedriver");

            // Navigate to the YouTube search page
            driver.Navigate().GoToUrl("https://www.youtube.com/results?search_query=" + searchTerm);

            // Set timeout
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            // Find the elements containing the video information
            var elements = driver.FindElements(By.XPath("//*[@id=\"dismissible\"]")).Take(5);

            // Open a StreamWriter to write to the CSV file
            using (StreamWriter writer = new StreamWriter("youtube_data.csv"))
            {
                // Write the header row
                writer.WriteLine("Link,Title,Uploader,Views");

                // Loop through the elements and extract the information
                foreach (var element in elements)
                {
                    // Extract the link of the video
                    var link = element.FindElement(By.XPath(".//*[@id='video-title']")).GetAttribute("href");

                    // Extract the title of the video
                    var title = element.FindElement(By.XPath(".//*[@id='video-title']")).Text;

                    // Extract the name of the uploader
                    var uploader = element.FindElement(By.XPath(".//*[@class='yt-simple-endpoint style-scope yt-formatted-string']\r\n")).Text;
                    // Extract the number of views
                    var views = element.FindElement(By.XPath(".//*[@class='style-scope ytd-video-meta-block']/span[1]")).Text;

                    // Write the information to the CSV file
                    writer.WriteLine($"{link},{title},{uploader},{views}");
                }
            }
        }



    }
}

