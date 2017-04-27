using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;

namespace SocketServer
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(new IPEndPoint(IPAddress.Any, 4530));

            socket.Listen(4);

            socket.BeginAccept(new AsyncCallback(ClientAccepted), socket);
            /*socket.BeginAccept(new AsyncCallback((ar) =>
             {
                 var client = socket.EndAccept(ar);

                 client.Send(Encoding.Unicode.GetBytes("Hi there,I accept you requesr at " + DateTime.Now.ToString()));

                 var timer = new System.Timers.Timer();
                 timer.Interval = 2000d;
                 timer.Enabled = true;
                 timer.Elapsed += (o, a) =>
                   {
                       if (client.Connected)
                       {
                           try
                           {
                               client.Send(Encoding.Unicode.GetBytes("message from server at" + DateTime.Now.ToString()));
                           }
                           catch (SocketException ex)
                           {
                               Console.WriteLine(ex.Message);
                           }

                       }
                       else
                       {
                           timer.Stop();
                           timer.Enabled = false;
                           Console.WriteLine("Client is disconnected,the timer is stop.");
                       }
                   };
                 timer.Start();

                 client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);
             }), null);*/

            Console.WriteLine("Server is ready!");
            Console.Read();

        }

        public static void ClientAccepted(IAsyncResult ar)
        {
            var socket = ar.AsyncState as Socket;
            var client = socket.EndAccept(ar);
            client.Send(Encoding.Unicode.GetBytes("Hi there,I accept you request at" + DateTime.Now.ToString()));

            var timer = new System.Timers.Timer();
            timer.Interval = 2000d;
            timer.Enabled = true;
            timer.Elapsed += (o, a) =>
            {
                if (client.Connected)
                {
                    try
                    {
                        client.Send(Encoding.Unicode.GetBytes("Message from server at" + DateTime.Now.ToString()));
                    }
                    catch(SocketException ex)
                    {
                        Console.WriteLine(ex.Message);
                     }
                }
                else
                {
                    timer.Stop();
                    timer.Enabled = false;
                    Console.WriteLine("Client is disconnected,the timer is stop.");
                }
            };
            timer.Start();
            client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);
            socket.BeginAccept(new AsyncCallback(ClientAccepted), socket);
        }
        static byte[] buffer = new byte[1024];

        public static void ReceiveMessage(IAsyncResult ar)
        {
            try
            {
                var socket = ar.AsyncState as Socket;
                var length = socket.EndReceive(ar);
                var message = Encoding.Unicode.GetString(buffer, 0, length);
                Console.WriteLine(message);

                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), socket);

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
