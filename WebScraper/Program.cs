using System.IO;
using System.Net;

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
			var file = new StreamReader(@"C:\Users\Burak\Desktop\loesdau.de_for_scraping_-_Sheet0.txt");
			string line;
			while ((line = file.ReadLine()) != null)
			{
				var html = HttpGet(line);
				var name = GetName(html);
				var description = GetDescription(html);
				var images = GetImages(html);

			}
			file.Close();

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
			var startIndex = html.IndexOf("<div id=\"product-thumbnails\">");
			startIndex = html.IndexOf(">", startIndex);
			var endIndex = html.IndexOf("</div>", startIndex);
			return html.Substring(startIndex + 1, endIndex - startIndex);
		}

		private static string GetVariants(string html)
		{
			var startIndex = html.IndexOf("<div class=\"dropDown\">");
			startIndex = html.IndexOf(">", startIndex);
			var endIndex = html.IndexOf("</div>", startIndex);
			return html.Substring(startIndex + 1, endIndex - startIndex);
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
