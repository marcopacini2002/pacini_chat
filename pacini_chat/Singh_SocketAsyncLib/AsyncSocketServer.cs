using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

using Pacini_SocketAsyncLib;

namespace pacini_SocketAsyncLib
{
    public class AsyncSocketServer
    {
        IPAddress mIP;
        int mPort;
        TcpListener mServer;
        List<NicknameModel> mClients;

        public AsyncSocketServer()
        {
            mClients = new List<NicknameModel>();
        }

        // Server inizia as ascoltare
        public async void InAscolto(IPAddress ipaddr = null, int port = 23000)
        {
            //controlli generali
            if (ipaddr == null)
            {
                ipaddr = IPAddress.Any;
            }
            if (port < 0 || port > 65535)
            {
                port = 23000;
            }

            mIP = ipaddr;
            mPort = port;

            mServer = new TcpListener(mIP, mPort);

            Console.WriteLine("Server in ascolto su IP: {0} - Porta: {1}"
                                 , mIP.ToString(), mPort.ToString());
            mServer.Start();

            while (true)
            {
                TcpClient client = await mServer.AcceptTcpClientAsync();

                RegistraClient(client);

                RiceviMessaggio(client);
            }

        }



        public async void RiceviMessaggio(TcpClient client)
        {
            NetworkStream stream = null;
            StreamReader reader = null;
            try
            {
                stream = client.GetStream();
                reader = new StreamReader(stream);
                char[] buff = new char[512];
                byte[] buffsend = null;
                int nBytes = 0;
                while (true)
                {
                    Console.WriteLine("In attesa di un messaggio .");
                    //ricezione messaggio asincrono
                    nBytes = await reader.ReadAsync(buff, 0, buff.Length);
                    if (nBytes == 0)
                    {
                        RimuoviClient(client);
                        Console.WriteLine("Client Disconnesso");
                        break;
                    }
                    string recText = new string(buff);

                    NicknameModel nickClient = mClients.Where(e => e.Client == client).FirstOrDefault();
                    string risposta = $"{nickClient.NickName}:{recText}";
                    InviaATutti(risposta);

                }

            }
            catch (Exception ex)
            {
                RimuoviClient(client);

                Console.WriteLine("Errore: " + ex.Message);
            }


        }
        public async void RegistraClient(TcpClient client)
        {
            NetworkStream stream = null;
            StreamReader reader = null;
            try
            {
                stream = client.GetStream();
                reader = new StreamReader(stream);
                char[] buff = new char[512];
                int nBytes = 0;

                //ricezione messaggio asincrono
                nBytes = await reader.ReadAsync(buff, 0, buff.Length);

                string recvText = new string(buff);
                NicknameModel newClient = new NicknameModel();
                newClient.NickName = recvText;
                newClient.Client = client;

                mClients.Add(newClient);

            }
            catch (Exception ex)
            {

                Console.WriteLine("Errore: " + ex.Message);
            }


        }
        private void RimuoviClient(TcpClient client)
        {
            NicknameModel nm = mClients.Where(riga => riga.Client == client).FirstOrDefault();

            if (nm != null)
            {
                mClients.Remove(nm);
            }
        }


        public void InviaATutti(string messaggio)
        {

            try
            {
                foreach (NicknameModel client in mClients)
                {
                    byte[] buff = Encoding.ASCII.GetBytes(messaggio);
                    client.Client.GetStream().WriteAsync(buff, 0, buff.Length);

                }
                // Console.WriteLine("Invia tutti" +messaggio);
            }
            catch (Exception ex)
            {

                Console.WriteLine("Errore: " + ex.Message);
            }

        }
        public void Disconnetti()
        {
            try
            {
                foreach (NicknameModel client in mClients)
                {
                    client.Client.Close();
                    RimuoviClient(client.Client);
                }
                mServer.Stop();


            }
            catch (Exception ex)
            {

                Console.WriteLine("Errore: " + ex.Message);
            }

        }

    }
}
