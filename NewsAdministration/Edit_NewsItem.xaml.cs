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
    /// Логика взаимодействия для Edit_NewsItem.xaml
    /// </summary>
    public partial class Edit_NewsItem : Window
    {
        private NewsItem newsItem { get; set; }
        public Edit_NewsItem(NewsItem item)
        {
            InitializeComponent();
            newsItem = item;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            newsItem.title = Title.Text;
            newsItem.author = Author.Text;
            newsItem.description = Description.Text;
            newsItem.url = URL.Text;
            newsItem.urlToImage = URLToImage.Text;
            EditNews(newsItem);
            DialogResult = true;
            Close();
        }

        private async void EditNews(NewsItem newsItem)
        {
            string query = MainWindow.LinkToServer + "/Edit";
            try
            {
                using (HttpClient client = MainWindow.CreateClient())
                using (HttpResponseMessage response = await client.PostAsJsonAsync(query, newsItem))
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title.Text = newsItem.title;
            Author.Text = newsItem.author;
            Description.Text = newsItem.description;
            URL.Text = newsItem.url;
            URLToImage.Text = newsItem.urlToImage;
        }
    }
}
