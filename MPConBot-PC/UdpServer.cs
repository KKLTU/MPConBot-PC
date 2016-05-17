//using System;
//using System.Text;
//using System.Net;
//using System.Net.Sockets;
//using System.IO;
//using Emgu.CV;
//using Emgu.CV.Structure;
//using System.Threading;
//using System.Threading.Tasks;

//namespace MPConBot
//{
//    class UdpServer
//    {
//        static void Main(string[] args)
//        {
//            // variables declaration block
//            byte[] PacketReceived = new byte[1024];
//            var MyToken = new CancellationTokenSource(); //create token for the thread cancel
//            UdpClient serverSocket = new UdpClient(15000);
//            string PacketMessage = "";

//            int i = 0;
//            while (true) // this while for keeping the server "listening"
//            {

//                Console.WriteLine("Waiting for a UDP client...");                                   // display stuff
//                IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);                               // prepare
//                PacketReceived = serverSocket.Receive(ref client);                                  // receive packet
//                PacketMessage = Encoding.ASCII.GetString(PacketReceived, 0, PacketReceived.Length); // get string from packet
//                Console.WriteLine("Response from " + client.Address);                               // display stuff
//                Console.WriteLine("Message " + i++ + ": " + PacketMessage + "\n");                  // display received string

//                if (PacketMessage == "Start")
//                {
//                    MyToken = new CancellationTokenSource(); // for the restart, need a new token
//                    Task.Run(() => Start(ref serverSocket, ref client), MyToken.Token); //start method on another thread
//                }

//                if (PacketMessage == "Stop")
//                {
//                    MyToken.Cancel();
//                }
//            }
//        }

//        static public void Start(ref UdpClient serverSocket, ref IPEndPoint client)
//        {
//            int i = 0;
//            byte[] dataToSend;
//            while (true)
//            {
//                Thread.Sleep(50);
//                try
//                {
//                    dataToSend = Encoding.ASCII.GetBytes(i.ToString());
//                    serverSocket.Send(dataToSend, dataToSend.Length, client);
//                    i++;
//                }
//                catch (Exception e)
//                { }
//            }
//        }
//    }
//}



using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading;
using System.Threading.Tasks;
using L2Bot_Controller;


namespace MPConBot
{
    class UdpServer
    {
        static void Main(string[] args)
        {
            Capture capture = new Capture();
            byte[] dataReceived = new byte[1024];
            LoCoMoCo MyBot = new LoCoMoCo("COM4"); // com port number
            var VideoStreamToken = new CancellationTokenSource(); //create token for the cancel
            UdpClient serverSocket = new UdpClient(15000);
            string stringData = "";
            int i = 0;

            while (true) // this while for keeping the server "listening"
            {
                try {
                    Console.WriteLine("Waiting for a UDP client...");                                   // display stuff
                    IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);                               // prepare
                    dataReceived = serverSocket.Receive(ref client);                                    // receive packet
                    stringData = Encoding.ASCII.GetString(dataReceived, 0, dataReceived.Length);        // get string from packet
                    Console.WriteLine("Response from " + client.Address);                               // display stuff
                    Console.WriteLine("Message " + i++ + ": " + stringData + "\n");                     // display client's string

                    if (stringData.Equals("StartV"))
                    {
                        VideoStreamToken = new CancellationTokenSource();
                        Task.Run(() => StartVideoStream(serverSocket, client), VideoStreamToken.Token); //start method on another thread
                    }

                    if (stringData.Equals("StopV"))
                        VideoStreamToken.Cancel();

                    if (stringData.Equals("Forward"))
                        MyBot.forward();

                    if (stringData.Equals("Backward"))
                        MyBot.backward();

                    if (stringData.Equals("Left"))
                        MyBot.turnleft();

                    if (stringData.Equals("Right"))
                        MyBot.turnright();

                    if (stringData.Equals("Stop"))
                        MyBot.stop();

                } catch (Exception e)
                { }
            }
        }

        static public void StartVideoStream(UdpClient serverSocket, IPEndPoint client)
        {

            //int i = 0;
            //byte[] dataToSend;
            //while (true)
            //{
            //    Thread.Sleep(50);
            //    try
            //    {
            //        dataToSend = Encoding.ASCII.GetBytes(i.ToString());
            //        serverSocket.Send(dataToSend, dataToSend.Length, client);
            //        i++;
            //    }
            //    catch (Exception e)
            //    { }
            //}

            Capture capture = new Capture();
            Image<Bgr, Byte> frame;
            byte[] dataToSend;
            while (true)
            {
                Thread.Sleep(50);
                try
                {
                    frame = capture.QueryFrame();
                    var ms = new MemoryStream();
                    frame.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);                  // save it in the memory stream
                    dataToSend = ms.ToArray();                                                      // convert to byte 
                    serverSocket.Send(dataToSend, dataToSend.Length, client);                       // Here I am sending back
                }
                catch (Exception e)
                { }
            }
        }
    }
}






