using System;
using System.Collections.Generic;
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
            Console.WriteLine("[{0}] Added client to room.", _id);

        }
        public void RemoveClient(Client client)
        {

            participants.Remove(client);
            Console.WriteLine("[{0}] Removed client from room.", _id);

        }

        public void BroadcastMessage(DataPacket packet)
        {

            participants.ForEach(e =>
            {
                if(e.GetClient().Connected)
                    e.Message(packet);

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
