using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

// Test Changes

namespace MPConBot
{
    // This class is responsible of running the server side on the PC. 
    class UdpServer
    {
        static void Main(string[] args)
        {
            byte[] data = new byte[1024];
            UdpClient serverSocket = new UdpClient(15000);
            int i = 0;

            while (true)
            {
                Console.WriteLine("Waiting for a UDP client...");
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                data = serverSocket.Receive(ref sender);

                string stringData = Encoding.ASCII.GetString(data, 0, data.Length);
                Console.WriteLine("Response from " + sender.Address);
                Console.WriteLine("Message " + i++ + ": " + stringData + "\n");
            }

        }
    }
}
