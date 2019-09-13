using SimpleTCP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;




namespace Native.Csharp.App.LuaEnv
{
    class TcpSer
    {
        public static void service(int port)
        {

            try
            {
                string host = "0.0.0.0";
                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint ipe = new IPEndPoint(ip, port);
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建一个Socket类
                s.Bind(ipe);//绑定2000端口
                s.Listen(0);//开始监听
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "tcp server","Wait for connect");
                Socket temp = s.Accept();//为新建连接创建新的Socket。
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "tcp server","Get a connect");
                string recvStr = "";
                byte[] recvBytes = new byte[1024];
                int bytes;
                bytes = temp.Receive(recvBytes, recvBytes.Length, 0);//从客户端接受信息
                recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "tcp server","Server Get Message:" + recvStr);//把客户端传来的信息显示出来
                string sendStr = "Ok!Client Send Message Sucessful!";
                byte[] bs = Encoding.ASCII.GetBytes(sendStr);
                temp.Send(bs, bs.Length, 0);//返回客户端成功信息
                temp.Close();
                s.Close();
            }
            catch (ArgumentNullException e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "tcp server", "ArgumentNullException:" + e);
            }
            catch (SocketException e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "tcp server", "SocketException: " + e);
            }
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "tcp server", "Press Enter to Exit");

        }

        public static void client(int port,string host)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint ipe = new IPEndPoint(ip, port);//把ip和端口转化为IPEndPoint实例
                Socket c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建一个Socket
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "tcp server", "Conneting...");
                c.Connect(ipe);//连接到服务器
                string sendStr = "hello!This is a socket test";
                byte[] bs = Encoding.ASCII.GetBytes(sendStr);
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "tcp server", "Send Message");
                c.Send(bs, bs.Length, 0);//发送测试信息
                string recvStr = "";
                byte[] recvBytes = new byte[1024];
                int bytes;
                bytes = c.Receive(recvBytes, recvBytes.Length, 0);//从服务器端接受返回信息
                recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "tcp server", "Client Get Message:"+ recvStr);//显示服务器返回信息
                c.Close();
            }
            catch (ArgumentNullException e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "tcp server", "ArgumentNullException: "+e);
            }
            catch (SocketException e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "tcp server", "SocketException: "+ e);
            }
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "tcp server", "Press Enter to Exit");
        }
    }
}
