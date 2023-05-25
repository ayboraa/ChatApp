using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{

    public enum FunctionTypes { 
    
        CreateRoom,
        LeaveRoom,
        JoinRoom,
        Connect,
        ChatMessage,
        Test

    };

    [Serializable]
    public class DataPacket
    {
        public FunctionTypes FunctionType { get; set; }
        public string? Data { get; set; }

    }
}
