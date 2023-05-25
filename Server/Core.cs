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

namespace Server
{
    static class Core
    {

        public static void ParseMessage(Client theClient, string msg) { 
        
            DataPacket newPack = JsonConvert.DeserializeObject<DataPacket>(msg);

            switch (newPack.FunctionType)
            {

                case FunctionTypes.Connect:
                    
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
                default:
                    Console.WriteLine("Unknown Message.");
                    break;

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

        private static void CreateRoom(Client client) {

            Room newRoom = new Room(CreateRoomID(), client.GetHost());
            client.ChangeRoom(newRoom.GetId());

        }

        private static bool LeaveRoom(Client client) {
            bool isSuccessful = false;


            return isSuccessful;
        }

        private static bool JoinRoom(Client client, string roomId) {
            bool isSuccessful = false;


            return isSuccessful;
        }

        private static bool ChatMessage(Client client, string msg)
        {
            bool isSuccessful = false;


            return isSuccessful;
        }

    }
}
