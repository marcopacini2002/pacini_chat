using System;
using System.Collections.Generic;
using System.Linq;
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
using pacini_SocketAsyncLib;
namespace pacini_chat_client
{
    /// <summary>
    /// Logica di interazione per login.xaml
    /// </summary>
    public partial class login : Window
    {

        AsyncSocketClient mClient;

        public login()
        {
            InitializeComponent();

            mClient = new AsyncSocketClient();
        }

        

        private async void btn_login_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {

                mClient.SetServerIPAddress(txt_ip.Text);
                mClient.SetServerPort(txt_porta.Text);

               await  mClient.ConnettiAlServer();

                if (mClient.IsConnected())
                {
                    MessageBox.Show("connesso");

                    mClient.Invia(txt_username.Text);
                    MainWindow win2 = new MainWindow(mClient);
                    win2.Show();
                    this.Close();


                }
                else MessageBox.Show("Impossibile connettersi al server controllare ip o porta");








            }
            catch (Exception)
            {
                MessageBox.Show("Errore in esecuzione di programma");
            }
        }
    }
}
