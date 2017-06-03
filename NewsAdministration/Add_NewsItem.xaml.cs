using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NewsAdministration
{
    /// <summary>
    /// Логика взаимодействия для AddNewsItem.xaml
    /// </summary>
    public partial class Add_NewsItem : Window
    {
        public Add_NewsItem()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            NewsItem item = new NewsItem()
            {
                title = Title.Text,
                author = Author.Text,
                description = Description.Text,
                url = URL.Text,
                urlToImage = URLToImage.Text,
                publishedAt = DateTime.Now.ToString(),
                RowKey = string.Format("{0:10}_{1}", 
                DateTime.MaxValue.Ticks - DateTime.Now.Ticks, Guid.NewGuid())
            };
            AddNews(item);
            DialogResult = true;
            Close();
        }

        private async void AddNews(params NewsItem[] news)
        {
            string query = MainWindow.LinkToServer + "/Post";
            NewsParser.Data data = new NewsParser.Data(news);
            try
            {
                using (HttpClient client = MainWindow.CreateClient())
                using (HttpResponseMessage response = await client.PostAsJsonAsync(query, data))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred while sending request to the server.\n\n" + e.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
