using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;

namespace pacini_SocketAsyncLib
{
    public class AsyncSocketClient
    {
        IPAddress mServerIpAddress;
        int mServerPort;
        TcpClient mClient;

        public List<string> Messaggi = new List<string>();
        public event EventHandler OnNewMessage;

        protected virtual void OnNewMessageHandler(EventArgs e)
        {
            EventHandler handler = OnNewMessage;
            handler?.Invoke(this, e);
        }

        public IPAddress ServerIpAddress
        {
            get
            {
                return mServerIpAddress;
            }
        }
        public int ServerPort
        {
            get
            {
                return mServerPort;
            }
        }
        public bool SetServerIPAddress(string str_IPAddress)
        {
            IPAddress ipaddr = null;
            if (!IPAddress.TryParse(str_IPAddress, out ipaddr))
            {
                Debug.WriteLine("Ip non valido.");
                return false;
            }

            mServerIpAddress = ipaddr;
            return true;
        }
        public bool SetServerPort(string str_port)
        {
            int port = -1;
            if (!int.TryParse(str_port, out port))
            {
                Console.WriteLine("Porta non valida");
                return false;
            }
            if (port < 0 || port > 65535)
            {
                Debug.WriteLine("La porta deve essere compressa tra 0 e 65535");
                return false;
            }
            mServerPort = port;
            return true;
        }


        public async Task ConnettiAlServer()
        {
            if (mClient == null)
            {
                mClient = new TcpClient();
            }

            try
            {
                await mClient.ConnectAsync(mServerIpAddress, mServerPort);
                Debug.WriteLine("Connesso al server IP/Port: {0} / {1}",
                                    mServerIpAddress.ToString(), mServerPort);


                RiceviMessaggi();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

            }
        }
        public bool IsConnected()// metodo per vedere se è connesso
        {


            if (mClient == null || !mClient.Connected)
            {
                return false;

            }

            return true;



        }
        public async Task RiceviMessaggi()
        {
            NetworkStream stream = null;
            StreamReader reader = null;
            try
            {
                stream = mClient.GetStream();
                reader = new StreamReader(stream);
                char[] buff = new char[512];
                int nBytes = 0;

                while (true)
                {
                    nBytes = await reader.ReadAsync(buff, 0, buff.Length);
                    if (nBytes == 0)
                    {
                        Debug.WriteLine("Disconnesso.");
                        break;
                    }

                    string recvMessage = new string(buff, 0, nBytes);

                    Messaggi.Add(recvMessage);


                    EventArgs e = new EventArgs();
                    OnNewMessageHandler(e);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public string Messaggio()
        {

            return "d";
        }
        public void Invia(string messaggio)
        {
            if (mClient == null)
            {
                return;
            }
            if (!mClient.Connected)
            {
                Debug.WriteLine("Not connected");
                return;
            }

            try
            {
                byte[] buff = Encoding.ASCII.GetBytes(messaggio);
                mClient.GetStream().WriteAsync(buff, 0, buff.Length);
                Debug.WriteLine("Sended");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);

            }
        }

        public AsyncSocketClient()
        {
            mServerIpAddress = null;
            mServerPort = -1;
            mClient = null;
        }


    }
}