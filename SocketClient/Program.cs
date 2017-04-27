using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

namespace SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect("localhost", 4530);
            Console.WriteLine("connect to the serve");
            // Console.WriteLine("localhost", 4530);
            //var buffer = new byte[1024];
            /*socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback((ar) =>
                {
                    var length = socket.EndReceive(ar);
                    var message = Encoding.Unicode.GetString(buffer, 0, length);

                    Console.WriteLine(message);
                }), null);*/
           

            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), socket);
            while (true)
            {
                var message = "Message from client:" + Console.ReadLine();
                var outputBuffer = Encoding.Unicode.GetBytes(message);
                socket.BeginSend(outputBuffer, 0, outputBuffer.Length, SocketFlags.None, null, null);
            }
            //Console.Read();

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
