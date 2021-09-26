using System;
using Lidgren.Network;
using System.Net;

namespace Client_console_test
{
    public class PlayerDB //data
    {
        public string name { get; set; }
        public string password { get; set; }
        public string ip { get; set; }
        public int gem { get; set; }
    }
    class Program
    {
        private static NetClient client;

        public static void Main(string[] args)
        {
            var config = new NetPeerConfiguration("enctest");
            client = new NetClient(config);
            client.Start();

            System.Threading.Thread.Sleep(100); // give server time to start up

            client.Connect("localhost", 14242);
            PlayerDB my = new PlayerDB();
            // loop forever
            while (true)
            {
                // read messages
                var inc = client.ReadMessage();
                if (inc != null)
                {
                    Console.WriteLine(inc.MessageType.ToString());
                    switch (inc.MessageType)
                    {
                        case NetIncomingMessageType.ErrorMessage:
                            Console.WriteLine("ERROR: " + inc.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            var status = (NetConnectionStatus)inc.ReadByte();
                            Console.WriteLine(inc.SenderConnection + " (" + status + ") " + inc.ReadString());
                            break;
                        case NetIncomingMessageType.Data:
                            Console.WriteLine("Data: " + inc.ReadString());
                            break;
                    }
                }

                // if we're connected, get input and send
                if (client.ServerConnection != null && client.ServerConnection.Status == NetConnectionStatus.Connected)
                {
                    Console.WriteLine("Type a message:");
                    var input = Console.ReadLine();
                    sendmessage(input);
                }
            }
        }

        private static void sendmessage(string msgs)
        {
            var msg = client.CreateMessage(msgs);
            var ok = client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

    }
    
}
