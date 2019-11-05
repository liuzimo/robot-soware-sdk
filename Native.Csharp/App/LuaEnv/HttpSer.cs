using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Native.Csharp.App.LuaEnv
{
    /// <summary>  
    /// HttpListenner监听Post请求参数值实体  
    /// </summary>  
    public class HttpListenerPostValue
    {
        /// <summary>  
        /// 0=> 参数  
        /// 1=> 文件  
        /// </summary>  
        public int type = 0;
        public string name;
        public byte[] datas;
    }

    /// <summary>  
    /// 获取Post请求中的参数和值帮助类  
    /// </summary>  
    public class HttpListenerPostParaHelper
    {
        private HttpListenerContext request;

        public HttpListenerPostParaHelper(HttpListenerContext request)
        {
            this.request = request;
        }

        private bool CompareBytes(byte[] source, byte[] comparison)
        {
            try
            {
                int count = source.Length;
                if (source.Length != comparison.Length)
                    return false;
                for (int i = 0; i < count; i++)
                    if (source[i] != comparison[i])
                        return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private byte[] ReadLineAsBytes(System.IO.Stream SourceStream)
        {
            var resultStream = new MemoryStream();
            while (true)
            {
                int data = SourceStream.ReadByte();
                resultStream.WriteByte((byte)data);
                if (data == 10)
                    break;
            }
            resultStream.Position = 0;
            byte[] dataBytes = new byte[resultStream.Length];
            resultStream.Read(dataBytes, 0, dataBytes.Length);
            return dataBytes;
        }

        /// <summary>  
        /// 获取Post过来的参数和数据  
        /// </summary>  
        /// <returns></returns>  
        public string GetHttpListenerPostValue()
        {
            try
            {
                List<HttpListenerPostValue> HttpListenerPostValueList = new List<HttpListenerPostValue>();
                var text = "";
                var cleaned_data = "";
                using (var reader = new StreamReader(request.Request.InputStream, Encoding.UTF8))
                {
                    text = reader.ReadToEnd();
                    cleaned_data = System.Web.HttpUtility.UrlDecode(text);
                }
                return cleaned_data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static HttpListener httpPostRequest = new HttpListener();
        private static void httpPostRequestHandle()
        {
            while (true)
            {
                HttpListenerContext requestContext = httpPostRequest.GetContext();
                Thread threadsub = new Thread(new ParameterizedThreadStart((requestcontext) =>
                {
                    HttpListenerContext request = (HttpListenerContext)requestcontext;
                    //获取Post请求中的参数和值帮助类  
                    HttpListenerPostParaHelper httppost = new HttpListenerPostParaHelper(request);
                    //获取Post过来的参数和数据  
                    string lst = httppost.GetHttpListenerPostValue();
                    

                    //Response  
                    request.Response.StatusCode = 200;
                    request.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    request.Response.ContentType = "application/json";
                    requestContext.Response.ContentEncoding = Encoding.UTF8;
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new { success = "true", msg = "" }));
                    request.Response.ContentLength64 = buffer.Length;
                    var output = request.Response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();

                    //处理数据
                    lst = lst.Substring(lst.IndexOf("{"),lst.LastIndexOf("}")- lst.IndexOf("{")+1);
                    LuaEnv.RunLua(
                    "",
                    "envent/ReceiveExpressPush.lua",
                    new ArrayList() {
                        "data", lst,
                    });
                }));
                threadsub.Start(requestContext);
            }
        }

        public static void ListenStart()
        {
            httpPostRequest.Prefixes.Add("http://172.16.196.77:2000/recive/");
            httpPostRequest.Start();

            Thread ThrednHttpPostRequest = new Thread(new ThreadStart(httpPostRequestHandle));
            ThrednHttpPostRequest.Start();
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "tcp server", "Server Get Message:" + "开启");//把客户端传来的信息显示出来

        }

    }
}