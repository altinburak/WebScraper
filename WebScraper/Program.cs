using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Translation.V2;
using Google.Apis.Auth.OAuth2;

namespace WebScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            StartTheProcess();
        }

        private static void StartTheProcess()
        {
            var lines = new string[1943];
            var file = new StreamReader(@"D:\list1.txt");
            string line;
            var index = 0;
            while ((line = file.ReadLine()) != null)
            {
                var html = HttpGet(line);
                var name = ConvertToEnglish(GetName(html));
                var description = ConvertToEnglish(GetDescription(html));
                var images = GetImages(html);
                var variants = GetVariants(html);

                lines[index] = "\"" + name + "\";\"" + description + "\";\"" + variants + "\";\"" + images;
                index++;
            }
            file.Close();

            File.WriteAllLines(@"D:\result.txt", lines);
        }

        public static string ConvertToEnglish(string german)
        {
            GoogleCredential credential = GoogleCredential.GetApplicationDefault();
            var client = TranslationClient.Create(credential, TranslationModel.NeuralMachineTranslation);
            var response = client.TranslateText(german, "en");
            return response.TranslatedText;
        }


        private static string GetName(string html)
        {
            var startIndex = html.IndexOf("<h2 class=\"subline js_only\">");
            startIndex = html.IndexOf(">", startIndex);
            var endIndex = html.IndexOf("</h2>", startIndex);
            return html.Substring(startIndex + 1, endIndex - startIndex);
        }

        private static string GetDescription(string html)
        {
            var startIndex = html.IndexOf("<div itemprop=\"description\"");
            startIndex = html.IndexOf(">", startIndex);
            var endIndex = html.IndexOf("</div>", startIndex);
            return html.Substring(startIndex + 1, endIndex - startIndex);
        }

        private static string GetImages(string html)
        {
            var retVal = string.Empty;
            var startIndex = html.IndexOf("<div id=\"product-thumbnails\">");
            startIndex = html.IndexOf(">", startIndex);
            var endIndex = html.IndexOf("</div>", startIndex);
            var tempResult = html.Substring(startIndex + 1, endIndex - startIndex);

            var links = Regex.Matches(tempResult, "data-big-src=").Cast<Match>().Count();
            startIndex = 0;

            for (int i = 0; i < links; i++)
            {
                startIndex = tempResult.IndexOf("data-big-src=", startIndex);
                startIndex = tempResult.IndexOf("\"", startIndex);
                endIndex = tempResult.IndexOf("target=", startIndex);
                retVal += tempResult.Substring(startIndex + 1, endIndex - startIndex - 3).Trim() + "\n";
                startIndex += 30;
            }
            return retVal;
        }

        private static string GetVariants(string html)
        {
            var retVal = string.Empty;
            var startIndex = html.IndexOf("<div class=\"dropDown\">");
            startIndex = html.IndexOf(">", startIndex);
            var endIndex = html.IndexOf("</div>", startIndex);
            var result = html.Substring(startIndex + 1, endIndex - startIndex);

            startIndex = result.IndexOf("<strong class=\"label\">");
            startIndex = result.IndexOf(">", startIndex);
            endIndex = result.IndexOf("</strong>", startIndex);
            retVal += result.Substring(startIndex + 1, endIndex - startIndex - 1) + ":";

            var count = Regex.Matches(result, "class=\"option").Cast<Match>().Count();
            startIndex = 0;
            for (int i = 0; i < count; i++)
            {
                startIndex = result.IndexOf("class=\"option\"", startIndex);
                startIndex = result.IndexOf(">", startIndex);
                endIndex = result.IndexOf("</option>", startIndex);
                retVal += result.Substring(startIndex + 1, endIndex - startIndex - 2).Replace('\n',' ').Replace('\t', ' ').Trim() + ',';
                startIndex += 10;
            }
            return retVal;
        }
        public static string HttpGet(string URI)
        {
            var req = WebRequest.Create(URI);
            var resp = req.GetResponse();
            var sr = new StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

    }
}
