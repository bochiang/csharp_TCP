using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace 客户端
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        //客户端与服务器之间的连接状态
        public bool bConnected = false;
        //监听线程
        public Thread tAcceptMsg = null;
        //用于socket通信的ip地址和通信端口
        public IPEndPoint IPP = null;
        //socket通信
        public Socket socket = null;
        //网络访问的基础数据流
        public NetworkStream nStream = null;
        //创建读取器
        public TextReader tReader = null;
        //创建编写器
        public TextWriter wReader = null;


        public void AcceptMessage()
        {
            string sTemp;
            while (bConnected)
            {
                try
                {
                    sTemp = tReader.ReadLine();
                    if (sTemp.Length != 0)
                    {
                        lock (this)
                        {
                            richTextBox1.Text = "服务器:" + sTemp + "\n" + richTextBox1.Text;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("无法与服务器通信");
                }
            }
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                IPP = new IPEndPoint(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text));
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket.Connect(IPP);
                if (socket.Connected)
                {
                    nStream = new NetworkStream(socket);
                    tReader = new StreamReader(nStream);
                    wReader = new StreamWriter(nStream);
                    tAcceptMsg = new Thread(new ThreadStart(this.AcceptMessage));
                    tAcceptMsg.Start();
                    bConnected = true;
                    button1.Enabled = false;
                    MessageBox.Show("与服务器成功连接，可以通信了!");
                }
            }
            catch
            {
                MessageBox.Show("无法与服务器通信!");

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bConnected)
            {
                try
                {
                    lock (this)
                    {
                        richTextBox1.Text = "客户端:" + richTextBox2.Text + "\n" + richTextBox1.Text;
                        wReader.WriteLine(richTextBox2.Text);
                        wReader.Flush();
                        richTextBox2.Text = "";
                        richTextBox2.Focus();
                    }
                }
                catch
                {
                    MessageBox.Show("与服务器连接断开了");
                }
            }
            else
            {
                MessageBox.Show("未与服务器建立连接，不能通信.");
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                socket.Close();
                tAcceptMsg.Abort();
            }
            catch
            {

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

///////////////////

    }
}
