﻿using Newtonsoft.Json;
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
        private TcpClient _tClient;
        private string _roomID;
        private Room _room;
        private Server _server;

        public DateTime LastMessage;

        private CancellationToken readCancel;

        public Client(TcpClient theClient, Server theServer) {

            _tClient = theClient;
            _roomID = null;
            _room = null;

            _guid = null;

            _server = theServer;


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

        public async Task ChangeRoom(string roomID)
        {
            if(roomID == null && _roomID != null)
            {
                Room theRoom = this._server.RoomsDict.GetValueOrDefault(_roomID, null);

                if(theRoom != null)
                {
                    theRoom.RemoveClient(this);
                    _roomID = null;
                    _room = null;
                }


                DataPacket packet = new DataPacket();
                packet.FunctionType = FunctionTypes.LeaveRoom;
                await this.Message(packet);
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

        public static void CallbackFunc(IAsyncResult res)
        {


        }

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
                    
                    data = System.Text.Encoding.ASCII.GetString(buffer);
                    Console.WriteLine("Parsing data: " + data);
                    Core.ParseMessage(this, data);

                    // clean it
                    buffer = new byte[1024];

                }

            }


        }

    }
}
