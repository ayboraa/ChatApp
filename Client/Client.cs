using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public delegate void ConnectionStatusChangedEventHandler(object sender, bool isConnected);
        public event ConnectionStatusChangedEventHandler ConnectionStatusChanged;
        public delegate void MessageReceivedEventHandler(object sender, string msg, string id);
        public event MessageReceivedEventHandler MessageReceieved;


        private string _guid;
        private TcpClient _tClient;
        private string _roomID;

        private CancellationToken readCancel;

        private void ParseMessage(string msg)
        {

            try
            {
                DataPacket newPack = new DataPacket();
                newPack = JsonConvert.DeserializeObject<DataPacket>(msg);
                string clientID = null;
                string roomID = null;

                bool isInvalid = false;

                switch (newPack.FunctionType)
                {

                    case FunctionTypes.Connect:
                        clientID = newPack.ClientID;
                        Connect(clientID);
                        break;
                    case FunctionTypes.CreateRoom:
                        roomID = newPack.Data;
                        CreateRoom(roomID);
                        break;
                    case FunctionTypes.LeaveRoom:
                        clientID = newPack.ClientID;
                        LeaveRoom(clientID);
                        break;
                    case FunctionTypes.JoinRoom:
                        roomID = newPack.Data;
                        clientID = newPack.ClientID;
                        JoinRoom(roomID, clientID);
                        break;
                    case FunctionTypes.ChatMessage:
                        string message = newPack.Data;
                        clientID = newPack.ClientID;
                        ChatMessage(message, clientID);
                        break;
                    case FunctionTypes.Ping:

                    default:
                        isInvalid = true;
                        Console.WriteLine("Unknown Message.");
                        break;

                }


            }
            catch (Exception ex)
            {

                Debug.Write(ex.ToString());

            }


        }


        private void Connect(string id)
        {

            this.SetGUID(id);
            ConnectionStatusChanged?.Invoke(this, true);

        }

        private void CreateRoom(string id)
        {
            _roomID = id;
        }

        private void LeaveRoom(string id)
        {
            _roomID = null;

        }

        private void JoinRoom(string roomId, string id)
        {

           // _roomID = roomId;

        }

        private void ChatMessage(string msg, string id)
        {
            MessageReceieved?.Invoke(this, msg, id);

        }

        public async Task SendMessage(DataPacket packet)
        {
            Byte[] bytes = new byte[1024];
            string data = JsonConvert.SerializeObject(packet);

            // test(print data in textbox)
            Console.WriteLine(data);
            //

            bytes = System.Text.Encoding.UTF8.GetBytes(data);
            _tClient.GetStream().BeginWrite(bytes, 0, bytes.Length, null, null);


        }


        // todo: sync ping time with server and check timeout
        //public long LastMessageTime;



        // create tcpclient -> connect -> get id -> create client
        public NetworkClient(TcpClient theClient)
        {

            _tClient = theClient;
            _guid = null;

            // LastMessageTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();



        }

        public TcpClient GetClient()
        {

            return _tClient;

        }

        public string GetRoomID()
        {

            return _roomID;

        }


        public string GetGUID()
        {

            return _guid;

        }

        public void SetGUID(string id)
        {
            if(_guid == null && id != null)
            {
                this._guid = id;

            }

        }
        
        public void EstablishConnection()
        {

            DataPacket packet = new DataPacket();
            packet.FunctionType = FunctionTypes.Connect;
            SendMessage(packet);

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



                    data = System.Text.Encoding.UTF8.GetString(buffer);
                    Console.WriteLine("Parsing data: " + data);
                    ParseMessage(data);

                    // clean it
                    buffer = new byte[1024];

                }

            }


        }



    }
}
