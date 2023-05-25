using Newtonsoft.Json;
using Server;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;

namespace Server
{
    public class ServerMain
    {


        public static async Task Main(string[] args)
        {

            Server mainServer = new Server();
            await mainServer.StartListening();

        }


    }
}