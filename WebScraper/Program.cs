using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

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
			var file = new StreamReader(@"C:\Users\Burak\Documents\scrape\list1.txt");
			string line;
			var index = 0;
			while ((line = file.ReadLine()) != null)
			{
				Console.WriteLine(index);
				string brand;
				var html = HttpGet(line);
				var name = TranslateText(GetName(html, out brand));
				var description = GetDescription(html);
				var images = GetImages(html);
				var variants = TranslateText(GetVariants(html));

				lines[index] = "\"" + brand + "\";\"" + name + "\";\"" + description + "\";\"" + variants + "\";\"" + images;
				index++;
			}
			file.Close();

			using (var w = File.AppendText(@"C:\Users\Burak\Documents\scrape\result.txt"))
			{
				foreach (var lineToWrite in lines)
				{
					w.WriteLine(lineToWrite);
				}
			}


			//File.WriteAllLines(@"D:\scrape\result.txt", lines);
			Console.Write("Done !");
			Console.Read();
		}


		public static string TranslateText(string input)
		{
			var url = $"http://www.google.com/translate_t?hl=en&ie=UTF8&text={input}&langpair=de|en";
			var webClient = new WebClient { Encoding = System.Text.Encoding.UTF8 };

			var result = webClient.DownloadString(url);
			var start = result.IndexOf("TRANSLATED_TEXT=") + 17;
			var end = result.IndexOf('\'', start + 3);
			result = result.Substring(start, end - start);

			return result;
		}

		private static string GetName(string html, out string brand)
		{
			brand = "";
			var startIndex = html.IndexOf("sProductName = ''");
			if (startIndex > 0)
			{
				startIndex = html.IndexOf("sProductName", startIndex + 1);
				startIndex = html.IndexOf("'", startIndex + 1);
				var endIndex = html.IndexOf("'", startIndex + 3);
				var name = html.Substring(startIndex + 1, endIndex - startIndex);


				startIndex = html.IndexOf("sBrand", startIndex);
				startIndex = html.IndexOf("'", startIndex + 1);
				endIndex = html.IndexOf("'", startIndex + 3);
				brand = html.Substring(startIndex + 1, endIndex - startIndex);
				return name;
			}
			return "";
		}

		private static string GetDescription(string html)
		{
			var retVal = string.Empty;
			var startIndex = html.IndexOf("<div itemprop=\"description\"");
			if (startIndex > 0)
			{
				startIndex = html.IndexOf(">", startIndex);
				var endIndex = html.IndexOf("</div>", startIndex);
				var desc = html.Substring(startIndex + 1, endIndex - startIndex);

				var cnt = desc.Split('>').Length;
				var strInd = 0;
				for (int i = 0; i < cnt-1; i++)
				{
					strInd = desc.IndexOf('>',strInd);
					var endInd = desc.IndexOf('<',strInd);
					var beforeTrans = desc.Substring(strInd + 1, endInd - strInd - 1).Trim();
					if (string.IsNullOrEmpty(beforeTrans)==false)
					{
						retVal += beforeTrans + "|";
					}
					strInd += 3;
				}
				var afterTrans = TranslateText(retVal);
				return afterTrans.TrimEnd('|');
			}
			return "";
		}

		private static string GetImages(string html)
		{
			var retVal = string.Empty;
			var startIndex = html.IndexOf("<div id=\"product-thumbnails\">");
			if (startIndex > 0)
			{
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
					retVal += tempResult.Substring(startIndex + 1, endIndex - startIndex - 3).Trim() + "|";
					startIndex += 30;
				}
			}
			return retVal;
		}

		private static string GetVariants(string html)
		{
			var retVal = string.Empty;
			var startIndex = html.IndexOf("<div class=\"dropDown\">");
			if (startIndex > 0)
			{
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
					retVal +=
						result.Substring(startIndex + 1, endIndex - startIndex - 2).Replace('\n', ' ').Replace('\t', ' ').Trim() + ',';
					startIndex += 10;
				}
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
