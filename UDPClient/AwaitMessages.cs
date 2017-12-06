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
        public string serverIP;
        public bool running;
        public int lastMessage;
        public string userName;

        public void WaitForMessages()
        {
            //Declare udp client variable on port 8085
            UdpClient udpClient = new UdpClient();
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            running = true;
            while (running)
            {
                //Whenever we get a new connection we want to try to receive bytes
                //IPEndPoint udpServer = new IPEndPoint(IPAddress.Any, 0);
                IPEndPoint udpServer = new IPEndPoint(IPAddress.Parse(serverIP), 8080);
                string message = $"send:{lastMessage}:{userName}";
                socket.SendTo(Encoding.ASCII.GetBytes(message), udpServer);
                var port = socket.LocalEndPoint.ToString().Split(':')[1];

                try
                {
                    //Once we receive bytes we want to change them to a string and print them in our chatbox.
                    //byte[] receivedBytes = udpClient.Receive(ref udpServer);
                    //string returnData = Encoding.ASCII.GetString(receivedBytes);

                    IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, int.Parse(port));
                    byte[] buffer = new byte[1024];
                    socket.Receive(buffer);

                    form.UpdateTextbox(Encoding.ASCII.GetString(buffer));
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }


            }

        }

        
        
}
}
