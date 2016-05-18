
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
            
            LoCoMoCo MyBot = new LoCoMoCo("COM3"); // com port number
            var MainToken = new CancellationTokenSource(); //create token for the cancel

            UdpClient MainServerSocket = new UdpClient(15000);
            UdpClient VideoServerSocket = new UdpClient(16000);
            UdpClient L2BotServerSocket = new UdpClient(17000);
            byte[] MainDataReceived = new byte[1024];
            string MainStringData = "";


            Capture capture = new Capture();
            Image<Bgr, Byte> frame;
            int TotalMessageCount = 0;

            while (true) // this while for keeping the main server "listening"
            {
                try {
                    // Thread.Sleep(100);
                    frame = capture.QueryFrame();
                    Console.WriteLine("Waiting for a UDP client...");                                   // display stuff
                    IPEndPoint MainClient = new IPEndPoint(IPAddress.Any,0);
                    MainDataReceived = MainServerSocket.Receive(ref MainClient);                                // receive packet
                    MainStringData = Encoding.ASCII.GetString(MainDataReceived, 0, MainDataReceived.Length);        // get string from packet
                    Console.WriteLine("Response from " + MainClient.Address);                               // display stuff
                    Console.WriteLine("Message " + TotalMessageCount++ + ": " + MainStringData + "\n");                     // display client's string

                    if (MainStringData.Equals("Picture"))
                    {
                        MainToken = new CancellationTokenSource();
                        Task.Run(() => SendPicture(MainServerSocket, MainClient, frame), MainToken.Token); //start method on another thread
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

        static public void SendPicture(UdpClient MainSocket, IPEndPoint MainClient, Image<Bgr,Byte> frame)
        {
            try
            {
                var ms = new MemoryStream();
                frame.Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);                  // save it in the memory stream
                byte[] dataToSend;
                dataToSend = ms.ToArray();                                                       // convert to byte 
                MainSocket.Send(dataToSend, dataToSend.Length, MainClient);                      // Here I am sending back
                //MainSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't Capture Frame.\n");
            }
        }

        static public void StartCamera()
        {
            while (true)
            {
                //Capture capture = new Capture();
                //Thread.Sleep(100);
                //frame = capture.QueryFrame();
            }
        }

    }
}






