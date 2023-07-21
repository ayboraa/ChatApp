using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Client : IDisposable
    {
        private string _guid;
        private TcpClient _tClient;
        private string _roomID;
        private Room _room;
        private Server _server;

        public long LastMessageTime;

        private delegate void Del(int diff);
        private Del ClientPingCheck;

        protected bool IsDisposed { get; private set; }

        public async void OnClientPingCheck(int diff)
        {

            if(diff >= 15000)
            {
                
                this.ChangeRoom(null);
                Console.WriteLine("Connection timed out. {0}", _guid);
                // dispose client
                await _server.RemoveClient(this);


            }
            else
            {

                this.LastMessageTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            }

        }

        private CancellationToken readCancel;

        public async Task CheckPing()
        {
            while (true)
            {

                if (IsDisposed)
                    break;

                int diff = (int)(DateTimeOffset.Now.ToUnixTimeMilliseconds() - this.LastMessageTime);
                OnClientPingCheck(diff);
                await Task.Delay(3000);
            }
        }

        public Client(TcpClient theClient, Server theServer) {

            _tClient = theClient;
            _roomID = null;
            _room = null;

            // used on clients
            _guid = this.GetHashCode().ToString();

            _server = theServer;

            LastMessageTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            ClientPingCheck += OnClientPingCheck;



        }

        public TcpClient GetClient()
        {

            return _tClient;

        }
        
        public Server GetHost()
        {

            return _server;

        }
        
        public Room GetRoom()
        {

            return _room;

        }

        public string GetGUID()
        {

            return _guid;

        }

        public async Task ChangeRoom(string roomID)
        {
            if(roomID == null && _roomID != null)
            {
                Room theRoom = this._server.RoomsDict.GetValueOrDefault(_roomID, null);


                if (theRoom != null)
                {
                    theRoom.RemoveClient(this);
                    _roomID = null;
                    _room = null;
                }


                return;

            }


            if(_roomID != roomID)
            {
                try{

                    Room theRoom;
                    bool roomFound = this._server.RoomsDict.TryGetValue(roomID, out theRoom);
                    if (roomFound)
                    {

                        theRoom.AddClient(this);
                        _roomID = theRoom.GetId();
                        _room = theRoom;

                    }
                    else
                    {

                        theRoom = new Room(roomID, this.GetHost());
                        theRoom.AddClient(this);
                        _roomID = theRoom.GetId();
                        _room = theRoom;
                    }
                    
                }
                catch (ArgumentNullException ex)
                {
                    Room newRoom = new Room(roomID, this.GetHost());
                    newRoom.AddClient(this);
                    _roomID = newRoom.GetId();
                    _room = newRoom;
                }
                finally
                {
                    DataPacket packet = new DataPacket();
                    packet.Data = _roomID;
                    packet.FunctionType = FunctionTypes.CreateRoom;
                    await this.Message(packet);

                }
               

            }
              

        }

        public static void CallbackFunc(IAsyncResult res){}

        // to be tested
        public async Task Message(DataPacket packet) {

            string msg = JsonConvert.SerializeObject(packet);
            Byte[] bytes= Encoding.UTF8.GetBytes(msg);
            _tClient.GetStream().BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(CallbackFunc), null);

        }


        public async Task StartReading() {


            if (_tClient.Connected)
            {

                NetworkStream myStream = _tClient.GetStream();

                byte[] buffer = new byte[1024]; 
                int bytesRead;

                String data = null;
                  

                 while ((bytesRead = await myStream.ReadAsync(buffer, 0, buffer.Length, readCancel).ConfigureAwait(false)) != 0)
                 {

                    if (IsDisposed)
                        break;


                    data = System.Text.Encoding.UTF8.GetString(buffer);
                    Console.WriteLine("Parsing data: " + data);
                    Core.ParseMessage(this, data);

                    // clean it
                    buffer = new byte[1024];

                }

            }


        }

        ~Client()
        {
            this.Dispose(false);
        }


        public void Dispose()
        {

            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (disposing)
                {
                    // Perform managed cleanup here.

                }

                // Perform unmanaged cleanup here.

                this.IsDisposed = true;
            }
        }
    }
}
