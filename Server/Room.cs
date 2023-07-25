using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Room
    {

        private string _id;
        private List<Client> participants = new List<Client>();
        private Server theServer;
        
        
        public void AddClient(Client client)
        {


            participants.Add(client);


            DataPacket packet = new DataPacket();
            packet.FunctionType = FunctionTypes.JoinRoom;
            packet.ClientID = client.GetGUID();
            packet.Data = _id;
            this.BroadcastMessage(packet);


            Console.WriteLine("[{0}] Added client to room. {1}", _id, client.GetGUID());


            // give information about room to new client
            DataPacket infoPacket = new DataPacket();
            packet.FunctionType = FunctionTypes.JoinRoom;
            packet.Data = this._id;
            
            participants.ForEach(async e =>
            {
                

                if (e.GetClient().Connected)
                {
                    infoPacket.ClientID = e.GetGUID();
                    client.Message(infoPacket);
                }


            });


        }
        public void RemoveClient(Client client)
        {

            // let everyone say goodbye
            DataPacket packet = new DataPacket();
            packet.FunctionType = FunctionTypes.LeaveRoom;
            packet.ClientID = client.GetGUID();
            this.BroadcastMessage(packet);


            participants.Remove(client);
            Console.WriteLine("[{0}] Removed client from room. {1}", _id, client.GetGUID());

        }

        public async Task BroadcastMessage(DataPacket packet)
        {

            participants.ForEach(async e =>
            {
                

                if (e.GetClient().Connected)
                {
                    Console.WriteLine("Sending message to clients: {0}", packet.Data);
                    await e.Message(packet);
                }
                   

            });

        }  

        public Room(string id, Server serv) {

            this.theServer = serv;

            if (theServer.RoomsDict.ContainsKey(id))
            {
                bool emptyKeyFound = false;
                while (!emptyKeyFound)
                {
                    string newId = Core.CreateRoomID();
                    if (!theServer.RoomsDict.ContainsKey(newId))
                    {
                        emptyKeyFound = true;
                        id = newId;

                    }
                }
            }
            else
            {

                theServer.RoomsDict.Add(id, this);

            }

            Console.WriteLine("[{0}] Room created.", id);

            this._id = id;

        }

        public string GetId() {

            return this._id;

        }
        

       


    }
}
