    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    namespace Client
    {
        class Program
        {
            static string userName;
            static int port = 8888; // порт сервера  
            static string address = "127.0.0.1"; // адрес сервера  

            static void SendMessage(Socket socket)
            {
                while (true)
                {
                    Console.Write("You : ");
                    string message = Console.ReadLine();
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    while (message == "")
                    {
                        message = Console.ReadLine();
                        data = Encoding.Unicode.GetBytes(message);
                    }
                    socket.Send(data);
                    if (message == "!quit")
                    {
                        Disconnect(socket);
                        break;
                    }
                }
            }

            static void ReceiveMessage(object obj)
            {
                Socket socket = (Socket)obj; while (true)
                {
                    try
                    {
                        byte[] data = new byte[256];
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = socket.Receive(data, data.Length, 0);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (socket.Available > 0);
                        string message = builder.ToString();
                        //string message1 = String.Format("\n{0} : {1}", DateTime.Now.ToShortTimeString(), message);  
                        Console.WriteLine(message);
                        Console.Write("You : ");
                    }
                    catch
                    {
                        Console.WriteLine("Connection lost!");

                        Disconnect(socket);
                    }
                }
            }

            static void Disconnect(object obj)
            {
                Socket socket = (Socket)obj;
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                Environment.Exit(0);
            }



            static void Main(string[] args)
            {
                Console.WriteLine("Enter your LOGIN:\n");
                userName = Console.ReadLine();

                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    socket.Connect(ipPoint);
                    string message = userName;
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    socket.Send(data);
                    Thread receiveThread = new Thread(ReceiveMessage);
                    receiveThread.Start(socket);
                    Console.WriteLine("Welcome, {0}", userName + "! Say HELLO to other users:");
                    SendMessage(socket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Disconnect(socket);
                }
            }

        }
    }

