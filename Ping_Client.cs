using System;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Ping_Client
{
    class Ping_Client
    {
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
            string host = "127.0.0.1"; // change to other IP address when capturing packets on wireshark (10.40.111.200)
            create_Client(host, port);
        }

        // method to create the UDP client
        public static void create_Client(string host, int port)
        {
            UdpClient udpClient = new UdpClient();
            udpClient.Connect(host, port);
            IPAddress remoteIP = IPAddress.Parse(host);
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(remoteIP, port);

            // send 10 pings from client to server
            send_N_Pings(10, RemoteIpEndPoint, udpClient);
        }

        // method to send N pings from client to server, and display RTT when reply is received
        public static void send_N_Pings(int N, IPEndPoint RemoteIpEndPoint, UdpClient udpClient)
        {
            for (int i = 0; i < N; i++)
            {
                Console.WriteLine("SENT PING: " + i + " AT TIME: " + System.DateTime.Now.TimeOfDay + " \r\n");
                // Create packet of data
                byte[] rdata = Encoding.ASCII.GetBytes("PING " + i + " " + System.DateTime.Now.TimeOfDay + " \r\n");
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                // Send data
                udpClient.Send(rdata, rdata.Length);

                // set timeout to receive reply
                udpClient.Client.ReceiveTimeout = 1000;

                try
                {
                    udpClient.Receive(ref RemoteIpEndPoint);
                    stopWatch.Stop();
                    TimeSpan RTT = stopWatch.Elapsed;
                    Console.WriteLine("Ping Answered RTT= " + RTT.Seconds + "." + RTT.Milliseconds + "seconds \r\n");
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Packet assumed to be dropped - took more than 1 second for reply.\n");
                }

            }
        }
    }
}
