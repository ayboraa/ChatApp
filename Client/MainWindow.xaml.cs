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

        private void NetworkClient_ConnectionStatusChanged(object sender, bool isConnected)
        {
            Dispatcher.Invoke(() => UpdateConnectionStatus(isConnected));
        }
        
        private void NetworkClient_MessageReceived(object sender, string msg)
        {
            Dispatcher.Invoke(() => UpdateChatBox(msg));
        }

        public void UpdateConnectionStatus(bool isConnected)
        {

            if (isConnected)
                this.label1.Content = "Connection: Established.";
            else
                this.label1.Content = "Connection: Waiting...";

        }
        
        public void UpdateChatBox(string msg)
        {

            this.chatBox.AppendText(msg + Environment.NewLine);

        }


        public async Task NetworkProcess(TcpClient client)
        {

            if (client.Connected)
            {
                MyClient = new NetworkClient(client);
                MyClient.ConnectionStatusChanged += NetworkClient_ConnectionStatusChanged;
                MyClient.MessageReceieved += NetworkClient_MessageReceived;
                MyClient.StartReading();
                MyClient.EstablishConnection();
            }

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


        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (this.input1.IsFocused)
                {
                   // if (MyClient.GetRoomID() != null)
                   // {
                        DataPacket myPacket = new DataPacket();
                        myPacket.FunctionType = FunctionTypes.ChatMessage;
                        myPacket.Data = this.input1.Text;
                        MyClient.SendMessage(myPacket);

                        this.input1.Text = "";
                   // }
                }

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            DataPacket myPacket = new DataPacket();
            myPacket.FunctionType = FunctionTypes.CreateRoom;
            MyClient.SendMessage(myPacket);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(MyClient.GetRoomID() != null)
            {
                DataPacket myPacket = new DataPacket();
                myPacket.FunctionType = FunctionTypes.ChatMessage;
                myPacket.Data = this.input1.Text;
                MyClient.SendMessage(myPacket);

                this.input1.Text = "";
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DataPacket myPacket = new DataPacket();
            myPacket.FunctionType = FunctionTypes.JoinRoom;
            myPacket.Data = this.rInput.Text;
            MyClient.SendMessage(myPacket);
        }

    }
}
