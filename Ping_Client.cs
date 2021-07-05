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
            string host = "127.0.0.1";
            create_Client(host, port);
        }

        public static void create_Client(string host, int port)
        {
            UdpClient udpClient = new UdpClient();
            udpClient.Connect(host, port);
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, port);

            send_N_Pings(10, RemoteIpEndPoint, udpClient);
        }

        public static void send_N_Pings(int N, IPEndPoint RemoteIpEndPoint, UdpClient udpClient)
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("CLIENT SENT PING: " + i + " TIME: " + System.DateTime.Now.TimeOfDay + " \r\n");
                byte[] rdata = Encoding.ASCII.GetBytes("PING " + i + " " + System.DateTime.Now.TimeOfDay + " \r\n");
                Stopwatch stopWatch = new Stopwatch(); 
                stopWatch.Start(); 
                udpClient.Send(rdata, rdata.Length);


                var task = Task.Run(() => udpClient.Receive(ref RemoteIpEndPoint));
                if (task.Wait(TimeSpan.FromSeconds(1)))
                {
                    stopWatch.Stop();
                    TimeSpan RTT = stopWatch.Elapsed;
                    Console.WriteLine("CLIENT RECIEVED PING: " + i + " RTT: " + RTT.Seconds+"."+RTT.Milliseconds + "seconds \r\n");
                }
                else { 
                Console.WriteLine("PACKET LOST DURING TRANSMISSION PING: " + i + " TIME: " + System.DateTime.Now.TimeOfDay + " \r\n");
                    continue;
                }
            }
        }
    }
}
