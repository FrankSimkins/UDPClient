using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;

namespace UDPClient
{
    class AwaitMessages
    {
        //Reference to the original form(thread)
        public Form1 form;

        public bool running;

        public void WaitForMessages()
        {
            //
            //Declare udp client variable on port 8085
            //
            UdpClient udpClient = new UdpClient(0);

            running = true;
            while (running)
            {
                //Whenever we get a new connection we want to try to receive bytes
                IPEndPoint udpServer = new IPEndPoint(IPAddress.Any, 0);

                try
                {
                    //Once we receive bytes we want to change them to a string and print them in our chatbox.
                        byte[] receivedBytes = udpClient.Receive(ref udpServer);
                        string returnData = Encoding.ASCII.GetString(receivedBytes);
                        form.UpdateTextbox(returnData);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }


            }

        }

        
        
}
}
