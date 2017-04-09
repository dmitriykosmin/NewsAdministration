using Newtonsoft.Json;
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
    /// Логика взаимодействия для Log_in.xaml
    /// </summary>
    public partial class Log_in : Window
    {
        public Log_in()
        {
            InitializeComponent();
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            var pairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>( "grant_type", "password" ),
                    new KeyValuePair<string, string>( "username", Login.Text),
                    new KeyValuePair<string, string> ( "Password", Password.Password)
                };
            var content = new FormUrlEncodedContent(pairs);
            using (var client = new HttpClient())
            {
                try
                {
                    var response = client.PostAsync(@"http://localhost:1848/Token", content).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Учётные данные были введены неправильно.",
                            "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    var result = response.Content.ReadAsStringAsync().Result;
                    // Десериализация полученного JSON-объекта
                    Dictionary<string, string> tokenDictionary =
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                    MainWindow.Token = tokenDictionary["access_token"];
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Нет соединения с сервером.\n\n" + ex.Message,
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            Close();
        }
    }
}
