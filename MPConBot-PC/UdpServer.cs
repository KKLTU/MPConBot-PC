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
            
            LoCoMoCo MyBot = new LoCoMoCo("COM3"); // com port number
            //var VideoToken = new CancellationTokenSource(); //create token for the cancel
            var MainToken = new CancellationTokenSource(); //create token for the cancel
            //var VideoStreamToken = new CancellationTokenSource(); //create token for the cancel

            UdpClient MainServerSocket = new UdpClient(15000);
            UdpClient VideoServerSocket = new UdpClient(16000);
            UdpClient L2BotServerSocket = new UdpClient(17000);
            byte[] MainDataReceived = new byte[1024];
            //byte[] VideoDataReceived = new byte[1024];
            //byte[] L2BotDataReceived = new byte[1024];
            string MainStringData = "";
            //string VideoStringData = "";
            //string L2BotStringData = "";

            int TotalMessageCount = 0;

            while (true) // this while for keeping the main server "listening"
            {
                try {
                        Console.WriteLine("Waiting for a UDP client...");                                   // display stuff
                        // IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);
                        IPEndPoint MainClient = new IPEndPoint(IPAddress.Any,0);
                        MainDataReceived = MainServerSocket.Receive(ref MainClient);                                // receive packet
                        MainStringData = Encoding.ASCII.GetString(MainDataReceived, 0, MainDataReceived.Length);        // get string from packet
                        Console.WriteLine("Response from " + MainClient.Address);                               // display stuff
                        Console.WriteLine("Message " + TotalMessageCount++ + ": " + MainStringData + "\n");                     // display client's string

                        if (MainStringData.Equals("Picture"))
                        {
                            MainToken = new CancellationTokenSource();
                            Task.Run(() => SendPicture(MainServerSocket, MainClient), MainToken.Token); //start method on another thread
                        }

                        if (MainStringData.Equals("StopV"))
                            MainToken.Cancel();

                        if (MainStringData.Equals("Forward"))
                            MyBot.forward();

                        if (MainStringData.Equals("Backward"))
                            MyBot.backward();

                        if (MainStringData.Equals("Left"))
                            MyBot.turnleft();

                        if (MainStringData.Equals("Right"))
                            MyBot.turnright();

                        if (MainStringData.Equals("Stop"))
                            MyBot.stop();

                } catch (Exception e)
                    { }
            }
        }

        static public void SendPicture(UdpClient MainSocket, IPEndPoint MainClient)
        {
            Capture capture = new Capture();
            Image<Bgr, Byte> frame;
            byte[] dataToSend;
            Thread.Sleep(500);
            try
            {
                frame = capture.QueryFrame();
                var ms = new MemoryStream();
                frame.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);                  // save it in the memory stream
                dataToSend = ms.ToArray();                                                      // convert to byte 
                MainSocket.Send(dataToSend, dataToSend.Length, MainClient);                       // Here I am sending back
            }
            catch (Exception e)
            { }
        }
    }
}






