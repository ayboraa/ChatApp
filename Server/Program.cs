using Newtonsoft.Json;
using Server;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;

namespace Server
{
    public class ServerMain
    {


        public static void Main(string[] args)
        {

            Server mainServer = new Server();
            mainServer.StartListening().GetAwaiter().GetResult();

        }


    }
}