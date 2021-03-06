﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace UDPClient
{
    public partial class Form1 : Form
    {

        //Declare Global variables
        static UdpClient udpClient = new UdpClient();
        public static Form1 form2;
        static IPAddress serverAddress;
        static AwaitMessages messageThread = new AwaitMessages();
        static Thread thread1;
        static int lastMessage;

        public Form1()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            //When the send button is clicked, get string, convert to bytes, then send to server.
            string messageToSend = $"receive:0:{UserNameTextbox.Text}:{MessageTextbox.Text}";
            byte[] byteMessage = Encoding.ASCII.GetBytes(messageToSend);
            int portInt = 8080;

            IPEndPoint udpServer = new IPEndPoint(serverAddress, portInt);

            //Try to send the byte array to the server
            try
            {
                udpClient.Send(byteMessage, byteMessage.Length, udpServer);

            }
            catch (Exception exception)
            {

                StatusLabel.Text = exception.ToString();
            }

        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            //Declare local variables
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            string userName = UserNameTextbox.Text;
            string serverIP = addressTextbox.Text;
            string port = PortTextbox.Text;
            bool parsedIP;
            bool parsedPort;
            Int32 portInt;


            //Parse IP to make sure they have entered something valid
            parsedIP = IPAddress.TryParse(serverIP, out serverAddress);
            parsedPort = Int32.TryParse(port, out portInt);


            if (!parsedIP || !parsedPort)
            {
                StatusLabel.Text = "Invalid IP address or port";
            }
            else
            {
                //If the IP and port are valid then we send a connect string to the server
                IPEndPoint udpServer = new IPEndPoint(serverAddress, portInt);
                byte[] byteMessage = Encoding.ASCII.GetBytes($"{userName}:");

                socket.SendTo(Encoding.ASCII.GetBytes($"connect:0:{userName}: "), udpServer);
                var portVar = socket.LocalEndPoint.ToString().Split(':')[1];

                try
                {

                    IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, int.Parse(port));
                    byte[] buffer = new byte[1024];
                    socket.Receive(buffer);

                    string receivedBytes = Encoding.ASCII.GetString(buffer);
                    string lmessage = receivedBytes.Split(':')[0];
                    int.TryParse(lmessage, out lastMessage);


                    HideForms();

                }
                catch (Exception exception)
                {

                    StatusLabel.Text = exception.ToString();
                }
                

            }


            AwaitingMessage(udpClient);

        }

        private void AwaitingMessage(UdpClient udpClient)
        {
            //Here we create a second thread, and send variable info to it.
            form2 = this;
            messageThread.form = form2;
            messageThread.serverIP = addressTextbox.Text;
            messageThread.lastMessage = lastMessage;
            messageThread.userName = UserNameTextbox.Text;
            ThreadStart s = messageThread.WaitForMessages;
            thread1 = new Thread(s);
            thread1.Start();
        }

        public void UpdateTextbox(string returnData)
        {
            //We need to make sure it is ok for another thread to change this form item so we see if invoke is required.
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateTextbox(returnData)));
                return;
            }
            else
            {
                //This prints out the message string received from the server.
                Chatbox1.Items.Add(returnData);
            }
            
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            thread1.Abort();

            ShowForms();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            thread1.Abort();
            Application.ExitThread();
            Environment.Exit(0);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            thread1.Abort();
            Application.ExitThread();
            Environment.Exit(0);
        }

        private void HideForms()
        {
            DisconnectButton.Visible = true;
            Chatbox1.Visible = true;
            MessageTextbox.Visible = true;
            SendButton.Visible = true;

            ConnectButton.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            label4.Visible = false;
            StatusLabel.Visible = false;
            UserNameTextbox.Visible = false;
            addressTextbox.Visible = false;
            PortTextbox.Visible = false;
        }

        private void ShowForms()
        {
            DisconnectButton.Visible = false;
            Chatbox1.Visible = false;
            MessageTextbox.Visible = false;
            SendButton.Visible = false;

            ConnectButton.Visible = true;
            label1.Visible = true;
            label2.Visible = true;
            label4.Visible = true;
            StatusLabel.Visible = true;
            UserNameTextbox.Visible = true;
            addressTextbox.Visible = true;
            PortTextbox.Visible = true;
        }
    }
}
