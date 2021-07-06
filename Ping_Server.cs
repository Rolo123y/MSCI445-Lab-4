using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Ping_Server
{
    class Ping_Server
    {
        private const double LOSS_RATE = 0.3;
        private const double AVG_DELAY = 50; //milliseconds
        static void Main(string[] args)
        {
            string val;
            // get port number from cli input
            Console.Write("Enter your port number: ");
            val = Console.ReadLine();

            if (val.Length < 4 || val.Length > 5)
            {
                Console.WriteLine("Required arguments: the length of the port number should be between 4 and 5!");
                return;
            }
            if (val.All(char.IsDigit) == false)
            {
                Console.WriteLine("Invalid arguments: port number should only contain positive integer digits!");
                return;
            }
            if (Convert.ToInt32(val) <= 1024)
            {
                Console.WriteLine("Invalid arguments: port number should be greater than 1024!");
                return;
            }
            int port = Convert.ToInt32(val);


            IPEndPoint localpt = new IPEndPoint(IPAddress.Any, port);
            // Create random number generator for use in simulating
            // packet loss and network delay.
            Random random = new Random();
            // Create a datagram socket for receiving and sending UDP packets
            // through the port specified on the command line.
            UdpClient socket = new UdpClient();
            socket.Client.SetSocketOption(SocketOptionLevel.Socket,
           SocketOptionName.ReuseAddress, true);
            socket.Client.Bind(localpt);
            System.Net.IPEndPoint ep = null;

            // Processing loop.
            while (true)
            {
                // Block until the host receives a UDP packet.
                byte[] rdata = socket.Receive(ref ep);
                Console.WriteLine("Received UDP data");
                // Print the recieved data.
                string name_string = Encoding.ASCII.GetString(rdata);
                Console.WriteLine("Just received: " + name_string);

                // Decide whether to reply, or simulate packet loss.
                if (random.NextDouble() < LOSS_RATE)
                {
                    Console.WriteLine("     Reply not sent.\n");
                    continue;
                }
                // Simulate network delay.
                Thread.Sleep((int)(random.NextDouble() * 2 * AVG_DELAY));
                // Send reply.
                socket.Send(rdata, rdata.Length, ep);
                Console.WriteLine("     Reply sent.\n");
            }
        }
    }
}
