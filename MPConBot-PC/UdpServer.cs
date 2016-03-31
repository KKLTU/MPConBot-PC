using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading;
using System.Threading.Tasks;

namespace MPConBot
{
    class UdpServer
    {
        bool VideoStreamStatus = false;

        static void Main(string[] args)
        {
            Capture capture = new Capture();
            Image<Bgr, Byte> frame;
            byte[] dataToSend;
            byte[] dataReceived = new byte[1024];

            var VideoStreamToken = new CancellationTokenSource(); //create token for the cancel

            UdpClient serverSocket = new UdpClient(15000);

            //string LocalImagePath = "C:\\Users\\kkhalaf\\Desktop\\Phone.PNG";                                 // image path
            //FileStream fs = new FileStream(LocalImagePath, FileMode.Open, FileAccess.Read);                   // prepare
            //BinaryReader br = new BinaryReader(fs);                                                           // prepare
            //byte[] dataToSend = br.ReadBytes(Convert.ToInt16(fs.Length));                                     //Convert image to byte[]

            int i = 0;
            while (true) // this while for keeping the server "listening"
            {
                Console.WriteLine("Waiting for a UDP client...");                                   // display stuff
                IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);                               // prepare
                dataReceived = serverSocket.Receive(ref client);                                    // receive packet
                string stringData = Encoding.ASCII.GetString(dataReceived, 0, dataReceived.Length); // get string from packet
                Console.WriteLine("Response from " + client.Address);                               // display stuff
                Console.WriteLine("Message " + i++ + ": " + stringData + "\n");                     // display client's string

                //Capture capture = new Capture();
                //Image<Bgr, Byte> frame;
                //byte[] dataToSend;
                while (true)
                {
                    frame = capture.QueryFrame();
                    var ms = new MemoryStream();
                    frame.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);                  // save it in the memory stream
                    dataToSend = ms.ToArray();                                                      // convert to byte 
                    serverSocket.Send(dataToSend, dataToSend.Length, client);                       // Here I am sending back
                }
            }
        }

//        if(stringData == "StartVideo")
//                {
//                    VideoStreamToken = new CancellationTokenSource();
//        Task.Run(() => StartVideoStream(ref serverSocket, ref client), VideoStreamToken.Token); //start method on another thread
//                }
//                if(stringData == "StopVideo")
//                {
//                    VideoStreamToken.Cancel();
//                    serverSocket.Close();
//                }
//static public void StartVideoStream(ref UdpClient serverSocket, ref IPEndPoint client)
//        {
//            Capture capture = new Capture();
//            Image<Bgr, Byte> frame;
//            byte[] dataToSend;
//            while (true)
//            {
//                frame = capture.QueryFrame();
//                var ms = new MemoryStream();
//                frame.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);                  // save it in the memory stream
//                dataToSend = ms.ToArray();                                                      // convert to byte 
//                serverSocket.Send(dataToSend, dataToSend.Length, client);                       // Here I am sending back
//            }
//        }
    }
}


