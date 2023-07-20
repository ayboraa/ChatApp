using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
using System.Windows.Threading;



namespace Client
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public NetworkClient MyClient;


        public async Task NetworkProcess(TcpClient client)
        {

            if (client.Connected)
            {
                Console.WriteLine("connected");
                
                MyClient = new NetworkClient(client, this);
                MyClient.StartReading();
                MyClient.EstablishConnection();

            }
            

        }

        public void UpdateConnectionStatus(bool isConnected) {

            if (isConnected)
                this.label1.Content = "Connection: Established.";
            else
                this.label1.Content = "Connection: Waiting...";

        }


        public MainWindow()
        {
            InitializeComponent();


            //try to connect to server
            TcpClient tClient = new TcpClient();
            tClient.Connect("127.0.0.1", 13000);
            Thread thread = new Thread(() => NetworkProcess(tClient));
            thread.Start();

            //todo: repeat process?
        }
    }
}
