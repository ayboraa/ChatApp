using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Server
{
    static class Core
    {

        public static void ParseMessage(Client theClient, string msg) {

            try
            {
                DataPacket newPack = new DataPacket();
                newPack = JsonConvert.DeserializeObject<DataPacket>(msg);

                bool isInvalid = false;

                switch (newPack.FunctionType)
                {

                    case FunctionTypes.Connect:
                        Core.Connect(theClient);
                        break;
                    case FunctionTypes.CreateRoom:
                        Core.CreateRoom(theClient);

                        break;
                    case FunctionTypes.LeaveRoom:
                        Core.LeaveRoom(theClient);
                        break;
                    case FunctionTypes.JoinRoom:
                        string roomID = newPack.Data;
                        Core.JoinRoom(theClient, roomID);
                        break;
                    case FunctionTypes.ChatMessage:
                        string message = newPack.Data;
                        Core.ChatMessage(theClient, message);
                        break;
                    case FunctionTypes.Ping: // todo: check if ping is working

                        break;
                    default:
                        isInvalid = true;
                        Console.WriteLine("Unknown Message.");
                        break;

                }

                if(!isInvalid)
                    theClient.LastMessageTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            }
            catch (Exception ex)
            {

                Debug.Write(ex.ToString());

            }
            

        }

        public static string CreateRoomID()
        {

            Random ran = new Random();

            String b = "abcdefghijklmnopqrstuvwxyz";

            int length = 6;

            String random = "";

            for (int i = 0; i < length; i++)
            {
                int a = ran.Next(26);
                random = random + b.ElementAt(a);
            }

            return random;


        }

        private static void Connect(Client client) {

            DataPacket packet = new DataPacket();
            packet.FunctionType = FunctionTypes.Connect;
            packet.ClientID = client.GetGUID();
            client.Message(packet);

        }
        
        private static void CreateRoom(Client client) {

            Room newRoom = new Room(CreateRoomID(), client.GetHost());
            client.ChangeRoom(newRoom.GetId());

        }

        private static void LeaveRoom(Client client) {


            client.ChangeRoom(null);


        }

        private static void JoinRoom(Client client, string roomId) {

            client.ChangeRoom(roomId);


        }

        private static void ChatMessage(Client client, string msg)
        {

            Room curRoom = client.GetRoom();

            if(curRoom != null)
            {
                DataPacket packet = new DataPacket();
                packet.FunctionType = FunctionTypes.ChatMessage;
                packet.Data = msg;
                packet.ClientID = client.GetGUID();
                curRoom.BroadcastMessage(packet);
               
            }


        }

    }
}
