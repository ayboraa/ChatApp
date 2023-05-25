using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Client
    {
        private string _guid;
        private IPAddress _ip;
        private TcpClient _tClient;
        private string _roomID;

        public DateTime LastMessage;

        private CancellationToken readCancel;

        public Client(TcpClient theClient) {

            _tClient = theClient;
            _roomID = null;

            // create mongodb object
            _guid = null;


        }

        public TcpClient GetClient()
        {

            return _tClient;

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

                    data = System.Text.Encoding.ASCII.GetString(buffer);
                    Console.WriteLine(data);
               
                 }

            }


        }

    }
}
