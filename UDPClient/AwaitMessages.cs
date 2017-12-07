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
        public int lastMessage = 0;
        public string userName;

        public void WaitForMessages()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            running = true;
            while (true)
            {
                IPEndPoint udpServer = new IPEndPoint(IPAddress.Parse(serverIP), 8080);
                string message = $"send:{lastMessage}:{userName}: ";
                socket.SendTo(Encoding.ASCII.GetBytes(message), udpServer);
                var port = socket.LocalEndPoint.ToString().Split(':')[1];

                try
                {
                    //Once we receive bytes we want to change them to a string and print them in our chatbox.

                    IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, int.Parse(port));
                    byte[] buffer = new byte[1024];
                    socket.Receive(buffer);
                    string buff = Encoding.ASCII.GetString(buffer);
                    string lmessage = buff.Split(':')[0];
                    int lmsg; int.TryParse(lmessage, out lmsg);

                    if (lmsg > lastMessage)
                    {
                        string userName = buff.Split(':')[1];
                        string stringMessage = buff.Split(':')[2];
                        form.UpdateTextbox(userName + ": " + stringMessage);
                        lastMessage++;
                    }
                    
                }
                catch (Exception exception)
                {
                    form.UpdateTextbox(exception.ToString());
                }


            }

        }

        
        
}
}
