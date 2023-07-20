using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Client
{
    public class NetworkClient
    {

        MainWindow app;
        private string _guid;
        private TcpClient _tClient;
        private string _roomID;

        private CancellationToken readCancel;


        public async Task SendMessage(TcpClient theClient, DataPacket packet)
        {
            Byte[] bytes = new byte[1024];
            string data = JsonConvert.SerializeObject(packet);

            // test(print data in textbox)
            Console.WriteLine(data);


            app.Dispatcher.Invoke(() => app.UpdateConnectionStatus(true));
            
            //

            bytes = System.Text.Encoding.ASCII.GetBytes(data);
            theClient.GetStream().BeginWrite(bytes, 0, bytes.Length, null, null);


        }


        // todo: sync ping time with server and check timeout
        //public long LastMessageTime;



        // create tcpclient -> connect -> get id -> create client
        public NetworkClient(TcpClient theClient, MainWindow app)
        {

            _tClient = theClient;
            _guid = null;
            this.app = app;

            // LastMessageTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();



        }

        public TcpClient GetClient()
        {

            return _tClient;

        }


        public string GetGUID()
        {

            return _guid;

        }
        
        public void EstablishConnection()
        {

            // get our guid from server
            DataPacket packet = new DataPacket();
            packet.FunctionType = FunctionTypes.Connect;
            SendMessage(_tClient, packet);

        }
        
        public async Task StartReading()
        {


            if (_tClient.Connected)
            {

                NetworkStream myStream = _tClient.GetStream();

                byte[] buffer = new byte[1024];
                int bytesRead;

                String data = null;


                while ((bytesRead = await myStream.ReadAsync(buffer, 0, buffer.Length, readCancel).ConfigureAwait(false)) != 0)
                {



                    data = System.Text.Encoding.ASCII.GetString(buffer);
                    Console.WriteLine("Parsing data: " + data);
                   // Core.ParseMessage(this, data);

                    // clean it
                    buffer = new byte[1024];

                }

            }


        }



    }
}
