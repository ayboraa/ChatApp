using Newtonsoft.Json;
using Server;
using System.Diagnostics;
using System.Net.Sockets;

public class TestApp
{


    public static async Task ReadThread(TcpClient client)
    {
        CancellationToken readCancel = new CancellationToken();

        try
        {
             

            if (client.Connected)
            {

                NetworkStream myStream = client.GetStream();

                byte[] buffer = new byte[1024];
                int bytesRead;

                String data = null;


                while ((bytesRead = await myStream.ReadAsync(buffer, 0, buffer.Length, readCancel).ConfigureAwait(false)) != 0)
                {

                    data = System.Text.Encoding.ASCII.GetString(buffer);
                    Console.WriteLine("Parsing data: " + data);

                    // clean it
                    buffer = new byte[1024];

                }

            }



        }
        catch (Exception ex)
        {
            
        }
    }


    public static async Task SendMessage(TcpClient theClient, DataPacket packet)
    {
        Byte[] bytes = new byte[1024];
        string data = JsonConvert.SerializeObject(packet);

        // test
        await Console.Out.WriteLineAsync(data);

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

                    Thread thread = new Thread(() => ReadThread(client));
                    thread.Start();

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
                    await SendMessage(client, myPacket);
              }
             


            }
            else if (key.Equals("2"))
            {

                if (client.Connected)
                {
                    DataPacket myPacket = new DataPacket();
                    myPacket.FunctionType = FunctionTypes.LeaveRoom;
                    await SendMessage(client, myPacket);
                }



            }
            else if (key.Equals("3"))
            {

                if (client.Connected)
                {
                    Console.WriteLine("Enter Room ID: ");
                    string id = Console.ReadLine();
                    DataPacket myPacket = new DataPacket();
                    myPacket.FunctionType = FunctionTypes.JoinRoom;
                    myPacket.Data = id;
                    await SendMessage(client, myPacket);
                }

            }
            else if (key.Equals("4"))
            {
                if (client.Connected)
                {
                    Console.WriteLine("Enter Message: ");
                    string id = Console.ReadLine();
                    DataPacket myPacket = new DataPacket();
                    myPacket.FunctionType = FunctionTypes.ChatMessage;
                    myPacket.Data = id;
                    await SendMessage(client, myPacket);
                }

            }



        }

    }



}