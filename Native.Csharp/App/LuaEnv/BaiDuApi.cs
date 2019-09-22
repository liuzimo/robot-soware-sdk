using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;

namespace Native.Csharp.App.LuaEnv
{

    class BaiDuApi
    {
        //本地图片文字识别
        public static string GeneralBasic(string filename)
        {
            try { 
                //var APP_ID = "你的 App ID";
                var API_KEY = "M5uerfD1rEdXT6ddwCYK2tYs";
                var SECRET_KEY = "ZTG6GYmrmXU655njhs6RVAMGFGY7Is8E";
                var client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);


                string dpath = Common.AppDirectory;
                dpath = dpath.Substring(0, dpath.LastIndexOf("\\"));
                dpath = dpath.Substring(0, dpath.LastIndexOf("\\"));
                dpath = dpath.Substring(0, dpath.LastIndexOf("\\") + 1);

                var image = File.ReadAllBytes(dpath+"image\\"+filename);
                // 调用通用文字识别, 图片参数为本地图片，可能会抛出网络等异常，请使用try/catch捕获
                //var result = client.GeneralBasic(image);
                //Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "图片识别错误", result+"");
                // 如果有可选参数
                var options = new Dictionary<string, object>{
                    {"language_type", "CHN_ENG"},
                    {"detect_direction", "false"},
                    {"detect_language", "false"},
                    {"probability", "false"}
                };
                // 带参数调用通用文字识别, 图片参数为本地图片
                var result = client.GeneralBasic(image, options);
                return result+"";
            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "图片识别错误", e.ToString());
                return "";
            }
        }

        //链接图片识别文字
        public static void GeneralBasicUrl(string url)
        {
            try { 
                //var APP_ID = "你的 App ID";
                var API_KEY = "M5uerfD1rEdXT6ddwCYK2tYs";
                var SECRET_KEY = "ZTG6GYmrmXU655njhs6RVAMGFGY7Is8E";
                var client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);

                // 调用通用文字识别, 图片参数为远程url图片，可能会抛出网络等异常，请使用try/catch捕获
                var result = client.GeneralBasicUrl(url);
                Console.WriteLine(result);
                // 如果有可选参数
                var options = new Dictionary<string, object>{
                    {"language_type", "CHN_ENG"},
                    {"detect_direction", "true"},
                    {"detect_language", "true"},
                    {"probability", "true"}
                };
                // 带参数调用通用文字识别, 图片参数为远程url图片
                result = client.GeneralBasicUrl(url, options);
                Console.WriteLine(result);
            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "网络图片识别错误", e.ToString());
            }
        }

        
        // 通用物体和场景识别
        public static string AdvancedGeneral(string filename)
        {
            try
            {
                string token = "24.5e680d741ae5c7ce3528d02aa3923528.2592000.1571734533.282335-17306771";
                string host = "https://aip.baidubce.com/rest/2.0/image-classify/v2/advanced_general?access_token=" + token;
                Encoding encoding = Encoding.Default;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
                request.Method = "post";
                request.KeepAlive = true;
                // 图片的base64编码
                string dpath = Common.AppDirectory;
                dpath = dpath.Substring(0, dpath.LastIndexOf("\\"));
                dpath = dpath.Substring(0, dpath.LastIndexOf("\\"));
                dpath = dpath.Substring(0, dpath.LastIndexOf("\\") + 1);

                string base64 = GetFileBase64(dpath + "image\\" + filename);
                String str = "image=" + HttpUtility.UrlEncode(base64);
                byte[] buffer = encoding.GetBytes(str);
                request.ContentLength = buffer.Length;
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string result = reader.ReadToEnd();
                return result;
            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "网络图片识别错误", e.ToString());
                return "";
            }
}

        public static String GetFileBase64(String fileName)
        {
            FileStream filestream = new FileStream(fileName, FileMode.Open);
            byte[] arr = new byte[filestream.Length];
            filestream.Read(arr, 0, (int)filestream.Length);
            string baser64 = Convert.ToBase64String(arr);
            filestream.Close();
            return baser64;
        }
        
    }
}
