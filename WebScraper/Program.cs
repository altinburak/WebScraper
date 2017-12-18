using System.IO;
using System.Linq;
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
			var count = html.Count(x => x.Equals("<div class=\"dropDown\">"));
			var startIndex = html.IndexOf("<div class=\"dropDown\">");
			startIndex = html.IndexOf(">", startIndex);
			var endIndex = html.IndexOf("</div>", startIndex);
			var result = html.Substring(startIndex + 1, endIndex - startIndex);
			for (int i = 1; i < count; i++)
			{
				startIndex = html.IndexOf("<div class=\"dropDown\">");
				startIndex = html.IndexOf(">", startIndex);
				endIndex = html.IndexOf("</div>", startIndex);
				result += html.Substring(startIndex + 1, endIndex - startIndex);
			}
			return result;
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
