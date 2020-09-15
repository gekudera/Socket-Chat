using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace Server { 
        public class ServerObject
        {
            static int port = 8888;
            static Socket Soc_Listener;
            List<ClientObject> clients = new List<ClientObject>();
            protected internal void AddConnection(ClientObject clientObject)
            {
                clients.Add(clientObject);
            }

            protected internal void RemoveConnection(string id)
            {
                ClientObject client = clients.FirstOrDefault(c => c.Id == id);
                if (client != null)
                    clients.Remove(client);
            }

            protected internal void Listen()
            {
                try
                {
                    IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                    Soc_Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Soc_Listener.Bind(ipPoint);
                    Console.WriteLine("Chat started. We are waiting for new connections ...");
                    while (true)
                    {
                        Soc_Listener.Listen(10);
                        Socket handler = Soc_Listener.Accept();
                        ClientObject clientObject = new ClientObject(handler, this);
                        Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                        clientThread.Start();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Disconnect();
                }
            }

            protected internal void BroadcastMessage(string message, string id)
            {
                byte[] data = Encoding.Unicode.GetBytes(message);
                for (int i = 0; i < clients.Count; i++)
                {
                    if (clients[i].Id != id)
                    {
                        clients[i].client.Send(data);
                    }
                }
            }

            protected internal void Disconnect()
            {
                Soc_Listener.Close();
                for (int i = 0; i < clients.Count; i++)
                {
                    clients[i].client.Close();
                }
                Environment.Exit(0);
            }
        }
    }


