using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Room
    {

        private string _id;
        private List<string> participants = new List<string>();
        
        
        public void AddClient(string clientID)
        {
            participants.Add(clientID);
        }
        public void RemoveClient(string clientID)
        {
            participants.Remove(clientID);
        }

        public Room(string id) {

            this._id = id;

        }

        public string GetId() {

            return this._id;

        }
        

    }
}
