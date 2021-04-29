using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Pacini_SocketAsyncLib
{
   public class NicknameModel
    {

        public String NickName { get; set; }
        public TcpClient Client { get; set; }
    }
}
