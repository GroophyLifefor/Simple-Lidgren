using System;
using Lidgren.Network;
using System.IO;
using System.Threading;
using System.Net;

namespace Server_test
{
    class Program
    {
        private static NetServer server;

        public class PlayerDB //data
        {
            public string name;
            public string password;
            public string ip;
            public int gem;
        }

        public static void Main()
        {
            Console.Title = "Server";
            var config = new NetPeerConfiguration("enctest");
            config.MaximumConnections = 2;
            config.Port = 14242;
            server = new NetServer(config);
            server.Start();

            // loop forever
            while (true)
            {
                var inc = server.ReadMessage();
                if (inc != null)
                {
                    Console.WriteLine(inc.MessageType.ToString());
                    NetPeer peer = inc.SenderConnection.Peer;
                    NetConnection con = inc.SenderConnection;
                    Object playerDB = peer.Tag;
					
                    switch (inc.MessageType)
                    {
                          
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            Console.WriteLine(inc.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            var status = (NetConnectionStatus)inc.ReadByte();
                            Console.WriteLine(inc.SenderConnection + " (" + status + ") " + inc.ReadString());
                            if (status == NetConnectionStatus.Connected)
                            {
                                inc.SenderConnection.Peer.Tag = new PlayerDB();
								sendmessage("Welcome to Server", con, NetDeliveryMethod.ReliableOrdered);
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            Console.WriteLine("Data: " + inc.ReadString());
							//receive
                            break;
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
        }

        private static void sendmessage(string msgs, NetConnection peer, NetDeliveryMethod method)
        {
            var msg = server.CreateMessage(msgs);
            server.SendMessage(msg, peer, method);
        }

    }
}
