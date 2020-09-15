using System;  
using System.Text;  
using System.Net;  
using System.Net.Sockets;  
using System.Collections.Generic;  
using System.Linq;  
using System.Threading;  


namespace Server
{
    public class ClientObject
    {
        protected internal string Id
        {
            get;
            private set;
        }
        string userName;

        protected internal Socket client;
        ServerObject server;

        public ClientObject(Socket socket, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = socket;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                string message = GetMessage(client);
                userName = message;
                message = "\n" + DateTime.Now.ToShortTimeString() + " - " + userName + " entered the chat";
                server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message);
                while (true)
                {
                    try
                    {
                        string get_message = GetMessage(client);
                        message = String.Format("\n{0} - {1}: {2}", DateTime.Now.ToShortTimeString(), userName, get_message); Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                        if (get_message == "!quit")
                        {
                            message = String.Format("\n{0} - {1}: left the chat", DateTime.Now.ToShortTimeString(), userName);
                            Console.WriteLine(message);
                            server.BroadcastMessage(message, this.Id);
                            break;
                        }
                    }
                    catch
                    {
                        message = String.Format("\n{0} - {1}: left the chat", DateTime.Now.ToShortTimeString(), userName);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close(client);
            }
        }

        private string GetMessage(Socket handler)
        {

            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = handler.Receive(data);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (handler.Available > 0);
            return builder.ToString();
        }
        protected internal void Close(Socket handler)
        {
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }
}

