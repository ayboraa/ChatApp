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

        }
        public void RemoveClient(Client client)
        {

            participants.Remove(client);

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


            this._id = id;

        }

        public string GetId() {

            return this._id;

        }
        

       


    }
}
