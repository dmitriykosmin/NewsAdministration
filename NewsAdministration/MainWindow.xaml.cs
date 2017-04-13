using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewsAdministration
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string LinkToServer = "http://127.0.0.1:1848/News";
        public static string Token { get; set; }
        private string JsonString { get; set; }
        private IEnumerable<NewsItem> News { get; set; }
        private DateTime CurrentDate { get; set; }
        private NewsView View { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            News = new List<NewsItem>();
            CurrentDate = DateTime.Now.Date;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Log_in window = new Log_in();
            window.ShowDialog();
            if (Token == null)
            {
                Close();
            }
            Date.SelectedDate = CurrentDate;
            await GetNews(CurrentDate);
        }

        public async Task GetNews(DateTime date)
        {
            string query = LinkToServer + "/Get";
            if (date != DateTime.Now.Date) query += "/" + date.ToString("MMddyyyy");
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(query))
                {
                    JsonString = await response.Content.ReadAsStringAsync();
                    News = NewsParser.ParseToList(JsonString);
                }
                if (View == null)
                {
                    View = new NewsView();
                }
                NewsView.SetView(News);
                UpdateView();
            }
            catch
            {
                MessageBox.Show("Соединение с сервером не установлено. Пожалуйста, проверьте ваше интернет соединение.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateView()
        {
            SetNewsPriview();
            NewsList.ItemsSource = null;
            NewsList.ItemsSource = View;
            if (News.Count() == 0)
            {
                NoNews.Visibility = Visibility.Visible;
            }
            else
            {
                NoNews.Visibility = Visibility.Hidden;
            }
        }

        private void SetNewsPriview()
        {
            NewsPreview.Items.Clear();
            NewsPreview.Items.Add(new TextBlock(new Run("Быстрое переключение между новостями")));
            for (int i = 0; i < News.Count(); i++)
            {
                Button temp = new Button()
                {
                    Content = News.ElementAt(i).title,
                    ClickMode = ClickMode.Release,
                    Name = "button" + i.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    MinWidth = 1200
                };
                temp.Click += Preview_Click;
                NewsPreview.Items.Add(temp);
            }
            NewsPreview.SelectedIndex = 0;
        }

        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            Button temp = (Button)sender;
            int pos = Convert.ToInt32(temp.Name.Remove(0, 6));
            NewsList.Items.MoveCurrentToPosition(pos);
            NewsList.ScrollIntoView(NewsList.Items.CurrentItem);
            NewsPreview.SelectedIndex = pos + 1;
            NewsList.SelectedIndex = pos;
        }

        private async void Date_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (Date.SelectedDate != CurrentDate)
            {
                CurrentDate = Date.SelectedDate.Value;
                await GetNews(CurrentDate);
            }
        }

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            await GetNews(CurrentDate);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void AddNewsItem_Click(object sender, RoutedEventArgs e)
        {
            Add_NewsItem window = new Add_NewsItem();
            bool? result = window.ShowDialog();
            if (result.Value)
            {
                CurrentDate = DateTime.Now.Date;
                Date.SelectedDate = CurrentDate;
                Thread.Sleep(5000);
                await GetNews(CurrentDate);
            }
        }

        public static HttpClient CreateClient()
        {
            var client = new HttpClient();
            if (!string.IsNullOrWhiteSpace(Token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            }
            return client;
        }

        private async void EditNewsItem_Click(object sender, RoutedEventArgs e)
        {
            if (NewsList.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите новость, которую хотите отредактировать.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var newsItem = News.ElementAt(NewsList.SelectedIndex);
            Edit_NewsItem window = new Edit_NewsItem(newsItem);
            bool? result = window.ShowDialog();
            if (result.Value)
            {
                Thread.Sleep(5000);
                await GetNews(CurrentDate);
            }
        }

        private async void DeleteNewsItem_Click(object sender, RoutedEventArgs e)
        {
            if (NewsList.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите новость, которую хотите удалить.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var answer = MessageBox.Show("Вы действительно хотите удалить новость?",
                "Подтвердите удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (answer == MessageBoxResult.Yes)
            {
                var newsItem = News.ElementAt(NewsList.SelectedIndex);
                Delete_NewsItem(newsItem);
                Thread.Sleep(5000);
                await GetNews(CurrentDate);
            }
        }

        public async void Delete_NewsItem(NewsItem newsItem)
        {
            string query = LinkToServer + "/Delete/" + newsItem.RowKey;
            try
            {
                using (HttpClient client = CreateClient())
                using (HttpResponseMessage response = await client.DeleteAsync(query))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("При запросе к серверу возникла ошибка.\n\n" + e.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
