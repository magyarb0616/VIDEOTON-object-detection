using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MLNET_ObjectDetection_WinForms
{
	class Server
	{
        private readonly TcpListener listener;
        private bool listen;
        public event Action<TcpClient> Connected;
        public event Action<TcpClient> Disconnected;
        public event Action<TcpClient, string> DataReceived;
        private TcpClient client;
        
        public Server(IPAddress ipAddress, int port)
        {
            listener = new TcpListener(ipAddress, port);
            listen = false;
        }

        static void ShowThreadInfo(String s)
        {
            Console.WriteLine("{0} thread ID: {1}", s, Thread.CurrentThread.ManagedThreadId);
        }

        public void Start()
        {
            listener.Start();
            listen = true;
            AcceptClients(listener);
        }

        private void AcceptClients(TcpListener listener)
        {
            while (listen)
            {
                client = listener.AcceptTcpClient();
                InvokeConnected(client);
                ReceiveMessages(client);
            }
        }

        private void ReceiveMessages(TcpClient client)
        {
            var reader = new StreamReader(client.GetStream());

            try
            {
                while (listen)
                {
                    var message = reader.ReadLine();
                    InvokeDataReceived(client, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                InvokeDisconnected(client);
            }
        }

        public void SendMessage(string msg)
		{

            if (client != null)
			{
                var writer = new StreamWriter(client.GetStream());
                writer.AutoFlush = true;
                //NetworkStream stream = client.GetStream();
                //byte[] byteMSG = Encoding.ASCII.GetBytes(msg);
                //stream.Write(Encoding.ASCII.GetBytes(msg), 0, msg.Length);
                writer.WriteLine(msg);
            }
		}

        public void Stop()
        {
            listen = false;
            if(client != null)
			{
                client.Close();
            }
            listener.Stop();
        }

        private void InvokeConnected(TcpClient client)
        {
            if (Connected != null)
                Connected(client);
        }

        private void InvokeDisconnected(TcpClient client)
        {
            if (Disconnected != null)
                Disconnected(client);
        }

        private void InvokeDataReceived(TcpClient client, string message)
        {
            if (DataReceived != null)
                DataReceived(client, message);

        }
    }
}

