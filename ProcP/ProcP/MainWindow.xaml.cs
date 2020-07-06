using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ProcP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string connInfo = "server=studmysql01.fhict.local;" + "database=dbi403931;" + "user=dbi403931;" + "password=kalina03;" + "connect timeout =30;";
        public MySqlConnection connection;

        public MainWindow()
        {
            InitializeComponent();


        }

        private void BtnClickDC(object sender, RoutedEventArgs e)
        {
            try
            {
                connection = new MySqlConnection(connInfo);
                string user_password = tbPassword.Password.ToString();
                string email = tbEmail.Text;
                string SQL = $"SELECT COUNT(*) from users where user_pwd='{user_password}' and user_email='{email}'";
                MySqlCommand c = new MySqlCommand(SQL, connection);
                connection.Open();
                MySqlDataReader reader = c.ExecuteReader();
                bool isLogged = false;
                while (reader.Read())
                {
                    if (reader[0].ToString() == "1")
                    {
                        isLogged = true;
                    }
                    else
                    {
                        return;
                    }
                }
                if (isLogged)
                {
                    connection.Close();

                    var windowDC = new DetailsConfiguration();
                    windowDC.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Wrong credentials!");
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Please connect to 'Cisco AnyConnect' vdi.fhict.nl");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void tbPassword_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
