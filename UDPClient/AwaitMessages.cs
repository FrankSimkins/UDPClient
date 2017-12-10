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
        //Declare Public variables to be set from the original thread.
        public Form1 form;
        public string serverIP;
        public int lastMessage = 0;
        public string userName;

        public void WaitForMessages()
        {
            //Create our socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);



            while (true)
            {

                //Declare local viariables
                IPEndPoint udpServer = new IPEndPoint(IPAddress.Parse(serverIP), 8080);
                string message = $"send:{lastMessage}:{userName}: ";

                //Send our message string to our server address
                socket.SendTo(Encoding.ASCII.GetBytes(message), udpServer);
                var port = socket.LocalEndPoint.ToString().Split(':')[1];

                try
                {
                    
                    IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, int.Parse(port));
                    byte[] buffer = new byte[1024];

                    //Wait here until we receive bytes back from the server.
                    socket.Receive(buffer);
                    string buff = Encoding.ASCII.GetString(buffer);
                    string lmessage = buff.Split(':')[0];
                    int lmsg; int.TryParse(lmessage, out lmsg);


                    //Here we check to see if have printed this message yet, if we haven't then we print it.
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
