using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Server
{
    public class Server
    {

        private TcpListener _server;
        private bool _active;
        private List<Client> _clients = new List<Client>();


        public Dictionary<string, Room> RoomsDict = new Dictionary<string, Room>();



        public Server()
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            _server = new TcpListener(localAddr, 13000);

        }

        public async Task StartListening()
        {


            try{

                // start listening...
                _server.Start();
                this._active = true;

                Console.WriteLine("Started");

                // accept incoming clients async.
                while (this._active)
                {
                    Console.WriteLine("Waiting for client");
                    var newClient = await _server.AcceptTcpClientAsync().ConfigureAwait(false);
                    AddNewClient(newClient);


                }


            }
            catch(SocketException ex){

                Console.WriteLine(ex.ToString());
            
            }

            
        }



        private async Task AddNewClient(TcpClient client) {

            Client newClient = new Client(client, this);
            _clients.Add(newClient);

            Console.WriteLine("A new client is connected.");
            // start reading.
            DataPacket testPacket = new DataPacket();
            testPacket.FunctionType = FunctionTypes.Test;
            newClient.Message(testPacket);
            
            newClient.StartReading();

        }


    }
}
