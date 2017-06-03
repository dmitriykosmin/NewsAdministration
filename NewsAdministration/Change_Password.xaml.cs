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
    /// Логика взаимодействия для Change_Password.xaml
    /// </summary>
    public partial class Change_Password : Window
    {
        public Change_Password()
        {
            InitializeComponent();
        }

        private async void Change_Click(object sender, RoutedEventArgs e)
        {
            string query = "http://localhost:1848/api/Account/ChangePassword";
            var changePasswordModel = new
            {
                OldPassword = OldPassword.Password,
                NewPassword = NewPassword.Password,
                ConfirmPassword = NewPassword.Password
            };
            try
            {
                using (HttpClient client = MainWindow.CreateClient())
                using (HttpResponseMessage response = await client.PostAsJsonAsync(query, changePasswordModel))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(response.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        MessageBox.Show("Password successfully changed.", "Complete",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while sending request to the server.\n\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
