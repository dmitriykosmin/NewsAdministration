using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NewsAdministration
{
    public class NewsParser
    {
        public class Data
        {
            public NewsItem[] articles { get; set; }
            public Data()
            {

            }

            public Data(params NewsItem[] news)
            {
                articles = news;
            }
        }

        public static List<NewsItem> ParseToList(string json)
        {
            Data data = JsonConvert.DeserializeObject<Data>(json);
            List<NewsItem> temp = new List<NewsItem>(data.articles);
            return temp;
        }
        public static string ParseToString(List<NewsItem> news)
        {
            return JsonConvert.SerializeObject(news);
        }
    }
}
