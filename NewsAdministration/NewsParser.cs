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
            public IEnumerable<NewsItem> articles { get; set; }
            public Data()
            {

            }

            public Data(IEnumerable<NewsItem> news)
            {
                articles = news;
            }
        }

        public static IEnumerable<NewsItem> ParseToList(string json)
        {
            Data data = JsonConvert.DeserializeObject<Data>(json);
            return data.articles;
        }
        public static string ParseToString(IEnumerable<NewsItem> news)
        {
            return JsonConvert.SerializeObject(news);
        }
    }
}
