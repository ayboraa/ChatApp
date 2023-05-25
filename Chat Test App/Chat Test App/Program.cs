using Newtonsoft.Json;
using Server;
using System.Net.Sockets;

public class TestApp
{

    public static async Task SendMessage(TcpClient theClient, DataPacket packet)
    {
        Byte[] bytes = new byte[1024];
        string data = JsonConvert.SerializeObject(packet);
        bytes = System.Text.Encoding.ASCII.GetBytes(data);
        theClient.GetStream().BeginWrite(bytes, 0, bytes.Length, null, null);


    }

    public static async Task Main()
    {

        // TODO: ping server to detect disconnects


        TcpClient client = new TcpClient();
       
        while (true)
        {

            string key = Console.ReadLine();


            if (key.Equals("c"))
            {
                if (!client.Connected)
                {
                    client.Connect("127.0.0.1", 13000);

                    DataPacket myPacket = new DataPacket();
                    myPacket.FunctionType = FunctionTypes.Connect;
                    await SendMessage(client, myPacket);

                }




            }else if (key.Equals("1"))
            {

              if (client.Connected)
              {
                  DataPacket myPacket = new DataPacket();
                  myPacket.FunctionType = FunctionTypes.CreateRoom;
                  SendMessage(client, myPacket);
              }
             


            }
            else if (key.Equals("2"))
            {

                if (client.Connected)
                {
                    DataPacket myPacket = new DataPacket();
                    myPacket.FunctionType = FunctionTypes.LeaveRoom;
                    SendMessage(client, myPacket);
                }



            }
            else if (key.Equals("3"))
            {



            }else if (key.Equals("4"))
            {



            }

           if (client.Connected)
           {
          
               NetworkStream stream = client.GetStream();
               byte[] buffer = new byte[1024];
               int bytesRead;
               string data = null;
               bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
               if(bytesRead != 0)
                {
                    data = System.Text.Encoding.ASCII.GetString(buffer);
                    Console.WriteLine(data);
                    continue;
                }
            }




        }

    }



}