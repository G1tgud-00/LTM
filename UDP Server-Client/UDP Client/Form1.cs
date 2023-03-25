using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Drawing.Text;

namespace UDP_Client
{
    public partial class Form1 : Form
    {
        private Socket test2;
        private EndPoint testIP;
        private EndPoint testLocal;
        private byte[] buffer;
        private void MessageCallBack(IAsyncResult ar)
        {
            try
            {
                byte[] recieve = new byte[1024];
                recieve = (byte[])ar.AsyncState;
                Encoding encoding = new UTF8Encoding();
                string texts = encoding.GetString(recieve);
                texts = texts.Replace("\0", String.Empty);
                
                textBox2.AppendText(texts);
                textBox2.AppendText(Environment.NewLine);

                test2.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref testIP, new AsyncCallback(MessageCallBack), buffer);
            } catch (Exception ex) { }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                return;
            Encoding encoding = new UTF8Encoding();
            byte[] send = new byte[1024];
            send = encoding.GetBytes(textBox1.Text);
            test2.Send(send);
            textBox2.AppendText(textBox1.Text);
            textBox2.AppendText(Environment.NewLine);
            textBox1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                test2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                test2.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                //testLocal = new IPEndPoint(IPAddress.Any, 20000);
                //test2.Bind(testLocal);
                testIP = new IPEndPoint(IPAddress.Loopback, 20000);
                test2.Connect(testIP);
                buffer = new byte[1024];
                test2.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref testIP, new AsyncCallback(MessageCallBack), buffer);
                Encoding encoding = new UTF8Encoding();
                test2.Send(encoding.GetBytes("Connected!"));
                button1.Enabled = false;
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
