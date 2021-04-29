using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pacini_SocketAsyncLib;
namespace pacini_chat_server
{
   public class Program
    {
        static void Main(string[] args)
        {
            AsyncSocketServer mServer;
            mServer
                = new AsyncSocketServer();
            mServer.InAscolto();

            Console.ReadLine();
        }
    }
}
