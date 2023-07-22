using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using HtmlAgilityPack;

namespace WebMonitoringScript
{
    class Program
    {
        static void Main(string[] args)
        {
            string websiteUrl = "https://tickets.manutd.com/en-GB/events/manchester%20united%20v%20rc%20lens/2023-8-5_12.45/old%20trafford?hallmap";
            string proxyAddress = "ultra.marsproxies.com:44443"; // Replace with the proxy address and port

            while (true)
            {
                bool containsN3402 = CheckAreaInfoForN3402(websiteUrl, proxyAddress);
                if (containsN3402)
                {
                    Console.WriteLine("The <h3> element contains <div class=\"name\">N3402</div> on the website.");
                    Console.WriteLine("Website Link: " + websiteUrl);

                    // Automatically open the website in the default web browser
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = websiteUrl,
                        UseShellExecute = true
                    });
                }
                else
                {
                    Console.WriteLine("The <h3> element does not contain <div class=\"name\">N3402</div> on the website. Refreshing the page...");

                    // Sleep for a few seconds before refreshing to avoid excessive requests
                    System.Threading.Thread.Sleep(2);

                    // Continue to the next iteration and refresh the website
                    continue;
                }

                System.Threading.Thread.Sleep(2); // Wait for 1 minute before checking again
            }
        }

        static bool CheckAreaInfoForN3402(string url, string proxyAddress)
        {
            try
            {
                using (var handler = new HttpClientHandler())
                {
                    if (!string.IsNullOrEmpty(proxyAddress))
                    {
                        handler.Proxy = new WebProxy(proxyAddress);
                        handler.UseProxy = true;
                    }

                    using (var client = new HttpClient(handler))
                    {
                        var response = client.GetAsync(url).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            string html = response.Content.ReadAsStringAsync().Result;

                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(html);

                            var selectedAreaInfoDiv = doc.DocumentNode.SelectSingleNode("//*[@id='selectedAreaInfo']/div[3]/div[10]/h3");
                            if (selectedAreaInfoDiv != null && selectedAreaInfoDiv.InnerHtml.Contains("<div class=\"name\">N3402</div>"))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }
    }
}
