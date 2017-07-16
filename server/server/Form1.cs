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

namespace 服务器
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }


        //客户端和服务器之间的连接状态
        private bool bConnected = false;
        //监听线程
        private Thread tAcceptMsg = null;

        //用于socket通信的ip地址和端口
        private IPEndPoint IPP = null;
        //socket通信
        private Socket socket = null;
        private Socket clientSocket = null;
        //网络访问的基础数据流；
        private NetworkStream nStream = null;
        //创建读取器
        private TextReader tReader = null;
        //创建编写器
        private TextWriter wReader = null;

        public void AcceptMessage()
        {
            //接受客户端的请求
            clientSocket = socket.Accept();
            if (clientSocket != null)
            {
                bConnected = true;
                try
                {
                    this.label1.Text = "与客户" + clientSocket.RemoteEndPoint.ToString() + "连接成功.";
                }
                catch { }
            }


            nStream = new NetworkStream(clientSocket);
            tReader = new StreamReader(nStream);
            wReader = new StreamWriter(nStream);
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

                            richTextBox1.Text = "客户机:" + sTemp + "\n" + richTextBox1.Text;
                        }
                    }
                }
                catch
                {
                    tAcceptMsg.Abort();
                    MessageBox.Show("无法与客户机通信");
                }
            }
            //禁止当前SOCKET上的发送和接受
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPP = new IPEndPoint(IPAddress.Any, 65535);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(IPP);
            socket.Listen(0);

                tAcceptMsg = new Thread(new ThreadStart(this.AcceptMessage));
                tAcceptMsg.Start();
                button1.Enabled = false;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bConnected)
            {
                try
                {
                    lock (this)
                    {
                        richTextBox1.Text = "服务器: " + richTextBox2.Text + "\n" + richTextBox1.Text;//


                        wReader.WriteLine(richTextBox2.Text);
                        wReader.Flush();

                        richTextBox2.Text = "";
                        richTextBox2.Focus();
                    }
                }
                catch
                {
                    MessageBox.Show("无法与客户机通信");
                }
            }
            else
            {
                MessageBox.Show("未与客户机建立连接，不能通信");
            }
        }
        //显示信息
//////////////////////////////
    }
}
