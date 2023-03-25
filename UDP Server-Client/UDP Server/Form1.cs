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
using static System.Net.Mime.MediaTypeNames;
using System.Net.NetworkInformation;
using System.Xml.Linq;
using System.Threading;

namespace UDP_Server
{
    public partial class Form1 : Form
    {
        private UdpClient test1;
        private IPEndPoint testIP;
        private IPEndPoint testLocal;
        private byte[] buffer;
        private Thread thr;
        public class UdpState
        {
            public UdpClient u;
            public IPEndPoint e;
            public UdpState(UdpClient u, IPEndPoint e)
            {
                this.u = u;
                this.e = e;
            }
        }
        public Form1()
        {
            InitializeComponent();
            thr = new Thread(receiver);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                return;
            Encoding encoding = new UTF8Encoding();
            byte[] send = new byte[1024];
            send = encoding.GetBytes(textBox1.Text);
            test1.Send(send, send.Length, testLocal);

            addText(textBox1.Text);
            textBox1.Text = "";
        }
        delegate void SetTextCallback(string text);
        private void addText(string text) 
        {
            textBox2.AppendText(text);
            textBox2.AppendText(Environment.NewLine); 
        }
        private void receiver()
        {
            while (true)
            {
                    byte[] buffer = test1.Receive(ref testLocal);
                    string msg = Encoding.UTF8.GetString(buffer);
                    msg = msg.Replace("\0", String.Empty);


                    if (this.textBox2.InvokeRequired)
                    {
                        SetTextCallback d = new SetTextCallback(addText);
                        this.Invoke(d, new object[] { msg });
                    }
                    else
                    {
                        addText(msg);
                    }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            testLocal = new IPEndPoint(IPAddress.Any, 20000);
            test1 = new UdpClient(testLocal);

            thr.Start();
           
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
