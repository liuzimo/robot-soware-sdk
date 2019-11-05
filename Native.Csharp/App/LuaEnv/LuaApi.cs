﻿using Native.Csharp.Sdk.Cqp.Enum;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Native.Csharp.App.LuaEnv
{
    class LuaApi
    {
        /// <summary>
        /// 获取图片对象
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="length">高度</param>
        /// <returns>图片对象</returns>
        public static Bitmap GetBitmap(int width, int length)
        {
            Bitmap bmp = new Bitmap(width, length);
            return bmp;
        }

        /// <summary>
        /// 摆放文字
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="text">文字内容</param>
        /// <param name="type">字体名称</param>
        /// <param name="size">字体大小</param>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        /// <returns>图片对象</returns>
        public static Bitmap PutText(Bitmap bmp, int x, int y, string text, string type = "宋体", int size = 9,
            int r = 0, int g = 0, int b = 0)
        {
            Graphics pic = Graphics.FromImage(bmp);
            Font font = new Font(type, size);
            Color myColor = Color.FromArgb(r, g, b);
            SolidBrush myBrush = new SolidBrush(myColor);
            pic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            pic.DrawString(text, font, myBrush, new PointF() { X = x, Y = y });
            return bmp;
        }

        /// <summary>
        /// 填充矩形
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <param name="x">起始x坐标</param>
        /// <param name="y">起始y坐标</param>
        /// <param name="xx">结束x坐标</param>
        /// <param name="yy">结束y坐标</param>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        /// <returns>图片对象</returns>
        public static Bitmap PutBlock(Bitmap bmp, int x, int y, int xx, int yy,
            int r = 0, int g = 0, int b = 0)
        {
            Color myColor = Color.FromArgb(r, g, b);
            //遍历矩形框内的各象素点
            for (int i = x; i <= xx; i++)
            {
                for (int j = y; j <= yy; j++)
                {
                    bmp.SetPixel(i, j, myColor);//设置当前象素点的颜色
                }
            }
            return bmp;
        }

        /// <summary>
        /// 摆放图片
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <param name="x">起始x坐标</param>
        /// <param name="y">起始y坐标</param>
        /// <param name="path">图片路径</param>
        /// <param name="xx">摆放图片宽度</param>
        /// <param name="yy">摆放图片高度</param>
        /// <returns>图片对象</returns>
        public static Bitmap SetImage(Bitmap bmp, int x, int y, string path, int xx = 0, int yy = 0)
        {
            if (!File.Exists(path))
                return bmp;
            Bitmap b = new Bitmap(path);
            Graphics pic = Graphics.FromImage(bmp);
            if (xx != 0 && yy != 0)
                pic.DrawImage(b, x, y, xx, yy);
            else
                pic.DrawImage(b, x, y);
            return bmp;
        }

        /// <summary>
        /// 保存并获取图片路径
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <returns>图片路径</returns>
        public static string GetDir(Bitmap bmp)
        {
            string result = Tools.GetRandomString(32, true, false, false, false, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
            bmp.Save(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/image/" + result + ".luatemp", ImageFormat.Jpeg);
            return result + ".luatemp";
        }


        /// <summary>
        /// 获取程序运行目录
        /// </summary>
        /// <returns>主程序运行目录</returns>
        public static string GetPath()
        {
            return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }

        /// <summary>
        /// 获取插件资源目录
        /// </summary>
        /// <returns>插件资源目录</returns>
        public static string GetAppName()
        {
           string[] name = Common.AppDirectory.Split('\\');
           return name[name.Length-2];
        }
        /// <summary>
        /// 获取qq消息中图片的路径
        /// </summary>
        /// <param name="image">图片字符串，如“[CQ:image,file=123123]”</param>
        /// <returns>网址</returns>
        public static string GetImagePath(string image)
        {
            string fileName = Tools.Reg_get(image, "\\[CQ:image,file=(?<name>.*?)\\]", "name");//获取文件
            if (fileName == "")
                return "";
            return Common.CqApi.ReceiveImage(fileName);
        }


        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="Url">文件网址</param>
        /// <param name="fileName">路径</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>下载结果</returns>
        public static bool HttpFileDownload(string Url, string fileName, int timeout = 5000)
        {
            //fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/" + fileName;
            try
            {
                //请求前设置一下使用的安全协议类型 System.Net
                if (Url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
                    {
                        return true; //总是接受
                    });
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = timeout;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.ContentLength < 1024 * 1024 * 20)//超过20M的文件不下载
                {
                    return Tools.SaveBinaryFile(response, fileName);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "下载文件错误", e.ToString());
            }
            return false;
        }


        /// <summary>
        /// 爬取图片
        /// </summary>
        /// <param name="Url">图片网址</param>
        /// <param name="path">路径</param>
        /// <param name="count">保存图片数量</param>
        /// <returns>下载结果</returns>
        public static void HttpImageDownload(string Url,string path, int count = 10)
        {
            string dpath = Common.AppDirectory;
            dpath = dpath.Substring(0, dpath.LastIndexOf("\\"));
            dpath = dpath.Substring(0, dpath.LastIndexOf("\\"));
            dpath = dpath.Substring(0, dpath.LastIndexOf("\\") + 1);
            if (!Directory.Exists(dpath + path + "\\"))
            {
                Directory.CreateDirectory(dpath + path + "\\");

                try
                {
                    WebClient MyWebClient = new WebClient();
                    MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
                    Byte[] pageData = MyWebClient.DownloadData(Url); //从指定网站下载数据
                    //string pageHtml = Encoding.Default.GetString(pageData);  //如果获取网站页面采用的是GB2312，则使用这句    
                    string pageHtml = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句

                    //document获取节点
                    //var wc = new WebClient();
                    //wc.Encoding = Encoding.GetEncoding("UTF-8"); //转格式
                    //var html = wc.DownloadString(url); //获取内容
                    //HtmlDocument doc = new HtmlDocument();
                    //doc.LoadHtml(html);
                    //HtmlNode nodeinfo = doc.GetElementbyId("content");//获取该id内容
                    //string htmlstr = nodeinfo.OuterHtml;

                    string pattern = @"<img\b[^<>]*?\boriginal[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>";
                    int i = 0;

              
                    foreach (Match match in Regex.Matches(pageHtml, pattern))
                    {
                        i++;
                        byte[] Bytes = MyWebClient.DownloadData(match.Groups["imgUrl"].Value);
                        using (MemoryStream ms = new MemoryStream(Bytes))
                        {
                            Image outputImg = Image.FromStream(ms);
                            outputImg.Save(dpath + path + "\\"+i+".jpg");
                        }
                        if (i == count) break;
                    }
                }
                catch (Exception e)
                {
                    Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "抓取图片错误", e.ToString());
                }
            }
        }


        /// <summary>
        /// GET 请求与获取结果
        /// </summary>
        public static string HttpGet(string Url, string postDataStr = "", int timeout = 5000,
            string cookie = "")
        {
            try
            {
                //请求前设置一下使用的安全协议类型 System.Net
                if (Url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
                    {
                        return true; //总是接受
                    });
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = timeout;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";
                if (cookie != "")
                    request.Headers.Add("cookie", cookie);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(encoding));

                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "get错误", e.ToString());
            }
            return "";
        }

        /// <summary>
        /// POST请求与获取结果
        /// </summary>
        public static string HttpPost(string Url, string postDataStr, int timeout = 5000,
            string cookie = "",string contentType = "application/x-www-form-urlencoded")
        {
            try
            {
                //请求前设置一下使用的安全协议类型 System.Net
                if (Url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
                    {
                        return true; //总是接受
                    });
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.Timeout = timeout;
                request.ContentType = contentType + "; charset=UTF-8";
                byte[] byteResquest = Encoding.UTF8.GetBytes(postDataStr);
                request.ContentLength = byteResquest.Length;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";
                if (cookie != "")
                    request.Headers.Add("cookie", cookie);

                Stream stream = request.GetRequestStream();
                stream.Write(byteResquest, 0, byteResquest.Length);
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                string retString = reader.ReadToEnd();
                return retString;
            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "post错误", e.ToString());
            }
            return "";
        }
        /// <summary>
        /// 获取返回的header
        /// </summary>
        /// 
        public static string GetResponseHeaders(String url,string postdata="",string cookie="")
        {
            try
            {
                Dictionary<string, string> HeaderList = new Dictionary<string, string>();

                WebRequest WebRequestObject = HttpWebRequest.Create(url);
                //cookie
                if (cookie != "")
                    WebRequestObject.Headers.Add("cookie", cookie);
                //post数据
                if (postdata !="")
                {
                    //请求方式
                    WebRequestObject.Method = "POST";
                    byte[] byteResquest = Encoding.UTF8.GetBytes(postdata);
                    WebRequestObject.ContentLength = byteResquest.Length;
                    Stream stream = WebRequestObject.GetRequestStream();
                    stream.Write(byteResquest, 0, byteResquest.Length);
                    stream.Close();
                }

                //获取response header头
                WebResponse ResponseObject = WebRequestObject.GetResponse();
                foreach (string HeaderKey in ResponseObject.Headers)

                    HeaderList.Add(HeaderKey, ResponseObject.Headers[HeaderKey]);

                ResponseObject.Close();

                //dict转换成json
                string responseheaders = JsonConvert.SerializeObject(HeaderList);

                return responseheaders;

            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "ResponseHeaders获取错误", e.ToString());
            }
            return "";
        }

/// <summary>
/// 获取本地图片的base64结果，会转成jpeg
/// </summary>
/// <param name="url"></param>
/// <returns></returns>
public static string Base64File(string path)
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(path));
        }

        /// <summary>
        /// 获取图片宽度
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int GetPictureWidth(string path)
        {
            try
            {
                Bitmap bmp = new Bitmap(path);
                return bmp.Width;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取图片高度
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int GetPictureHeight(string path)
        {
            try
            {
                Bitmap bmp = new Bitmap(path);
                return bmp.Height;
            }
            catch
            {
                return 0;
            }
        }

        private static Dictionary<string, string> luaTemp = new Dictionary<string, string>();
        /// <summary>
        /// 把值存入ram
        /// </summary>
        /// <param name="n"></param>
        /// <param name="d"></param>
        public static void SetVar(string n,string d)
        {
            luaTemp[n] = d;
        }
        /// <summary>
        /// 取出某值
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string GetVar(string n)
        {
            if (luaTemp.ContainsKey(n))
                return luaTemp[n];
            else
                return "";
        }
        /// <summary>
        /// 获取xml下面的文件夹列表
        /// </summary>
        /// <param name="p">路径</param>
        /// <returns></returns>
        public static ArrayList DirectoryList(String p)
        {
            ArrayList array = new ArrayList();
            String url = Common.AppDirectory + "xml\\";
            if (p != "")
            {
                url = Common.AppDirectory + "xml\\" + p + "\\";
            }
            //获取文件地址，此时返回的文件夹包含文件夹的整体路径
            String[] directorieStrings = Directory.GetDirectories(url);
            //如果需要获取文件夹的名称集合
            String[] dlist = directorieStrings.Select(d => d.Substring(d.LastIndexOf('\\') + 1 )).ToArray();
            array.Add(dlist.Length-1);
            array.Add(dlist);
            return array;
        }
        
        /// <summary>
         /// 获取xml下面的文件夹列表
         /// </summary>
         /// <param name="p">路径</param>
         /// <returns></returns>
        public static ArrayList FileList(String p)
        {
            ArrayList array = new ArrayList();
            String url = Common.AppDirectory + "xml\\";
            if (p != "")
            {
                url = Common.AppDirectory + "xml\\" + p + "\\";
            }
            var files = Directory.GetFiles(url, "*.*").Select(d => d.Substring(d.LastIndexOf('\\') + 1)).ToArray(); ;

            array.Add(files.Length - 1);
            array.Add(files);
            return array;
        }

        /// <summary>
        /// 获取字符串ascii编码的hex串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetAsciiHex(string str)
        {
            return BitConverter.ToString(Encoding.Default.GetBytes(str)).Replace("-", "");
        }

        /// <summary>
        /// 设置定时脚本运行间隔时间
        /// </summary>
        /// <param name="wait"></param>
        public static void SetTimerScriptWait(int wait) => TimerRun.luaWait = wait;

        public static string CqCode_At(long qq) => Common.CqApi.CqCode_At(qq);
        //获取酷Q "At某人" 代码
        public static string CqCode_Emoji(int id) => Common.CqApi.CqCode_Emoji(id);
        //获取酷Q "emoji表情" 代码
        public static string CqCode_Face(int id) => Common.CqApi.CqCode_Face((Sdk.Cqp.Enum.Face)id);
        //获取酷Q "表情" 代码
        public static string CqCode_Shake() => Common.CqApi.CqCode_Shake();
        //获取酷Q "窗口抖动" 代码
        public static string CqCode_Trope(string str) => Common.CqApi.CqCode_Trope(str);
        //获取字符串的转义形式
        public static string CqCode_UnTrope(string str) => Common.CqApi.CqCode_UnTrope(str);
        //获取字符串的非转义形式
        public static string CqCode_ShareLink(string url, string title, string content, string imgUrl) => Common.CqApi.CqCode_ShareLink(url, title, content, imgUrl);
        //获取酷Q "链接分享" 代码
        public static string CqCode_ShareCard(string cardType, long id) => Common.CqApi.CqCode_ShareCard(cardType, id);
        //获取酷Q "名片分享" 代码
        public static string CqCode_ShareGPS(string site, string detail, double lat, double lon, int zoom) => Common.CqApi.CqCode_ShareGPS(site, detail, lat, lon, zoom);
        //获取酷Q "位置分享" 代码
        public static string CqCode_Anonymous(bool forced) => Common.CqApi.CqCode_Anonymous(forced);
        //获取酷Q "匿名" 代码
        public static string CqCode_Image(string path) => Common.CqApi.CqCode_Image(path);
        //获取酷Q "图片" 代码
        public static string CqCode_Music(long id) => Common.CqApi.CqCode_Music(id);
        //获取酷Q "音乐" 代码
        public static string CqCode_MusciDIY(string url, string musicUrl, string title, string content, string imgUrl) => Common.CqApi.CqCode_MusciDIY(url, musicUrl, title, content, imgUrl);
        //获取酷Q "音乐自定义" 代码
        public static string CqCode_Record(string path) => Common.CqApi.CqCode_Record(path);
        //获取酷Q "语音" 代码
        public static int SendGroupMessage(long groupId, string message) => Common.CqApi.SendGroupMessage(groupId, message);
        //发送群消息
        public static int SendPrivateMessage(long qqId, string message) => Common.CqApi.SendPrivateMessage(qqId, message);
        //发送私聊消息
        public static int SendDiscussMessage(long discussId, string message) => Common.CqApi.SendDiscussMessage(discussId, message);
        //发送讨论组消息
        public static int SendPraise(long qqId, int count) => Common.CqApi.SendPraise(qqId, count);
        //发送赞
        public static int RepealMessage(int id) => Common.CqApi.RepealMessage(id);
        //撤回消息
        public static long GetLoginQQ() => Common.CqApi.GetLoginQQ();
        //取登录QQ
        public static string GetLoginNick() => Common.CqApi.GetLoginNick();
        //获取当前登录QQ的昵称
        public static string GetAppDirectory() => Common.AppDirectory;
        //取应用目录
        public static NLua.LuaTable GetMemberInfo(NLua.LuaTable t, long g, long q, bool a)
        {
            // 当地时区
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

            Sdk.Cqp.Model.GroupMemberInfo m = Common.CqApi.GetMemberInfo(g, q, a);
            t["Age"] = m.Age;
            t["Area"] = m.Area;
            t["Card"] = m.Card;
            t["JoiningTime"] = (long)(m.JoiningTime - startTime).TotalSeconds;
            t["LastDateTime"] = (long)(m.LastDateTime - startTime).TotalSeconds;
            t["Level"] = m.Level;
            t["Nick"] = m.Nick;
            t["PermitType"] = (int)m.PermitType;
            t["Sex"] = (int)m.Sex;
            t["SpecialTitle"] = m.SpecialTitle;
            return t;
        }
        //获取群成员信息

        public static ArrayList GetMemberList(long g)
        {
            // 当地时区
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

            List<Sdk.Cqp.Model.GroupMemberInfo> memberInfos = Common.CqApi.GetMemberList(g);

            ArrayList td = new ArrayList();
            ArrayList ts = new ArrayList();
            td.Add(memberInfos.Count - 1);
            foreach (var m in memberInfos)
            {
                Dictionary<string, object> t = new Dictionary<string, object>
                {
                    ["QQ"] = m.QQId,
                    ["Age"] = m.Age,
                    ["Card"] = m.Card,
                    ["JoiningTime"] = (long)(m.JoiningTime - startTime).TotalSeconds,
                    ["LastDateTime"] = (long)(m.LastDateTime - startTime).TotalSeconds,
                    ["Nick"] = m.Nick,
                    ["PermitType"] = (int)m.PermitType,
                    ["Sex"] = (int)m.Sex,
                    ["SpecialTitle"] = m.SpecialTitle
                };
                ts.Add(t);
            }
            td.Add(ts);
            return td;
        }
        //获取全部群成员信息
        public static ArrayList GetGroupList()
        {
            List<Sdk.Cqp.Model.GroupInfo> groupInfos = Common.CqApi.GetGroupList();

            ArrayList td = new ArrayList();
            ArrayList ts = new ArrayList();
            td.Add(groupInfos.Count - 1);

            foreach (var g in groupInfos)
            {
                Dictionary<string, object> t = new Dictionary<string, object>
                {
                    ["Id"] = g.Id,
                    ["Name"] = g.Name
                };
                ts.Add(t);
            }
            td.Add(ts);
            return td;
        }
        //获取群列表
        public static int AddLoger(int level, string type, string content) => Common.CqApi.AddLoger((Sdk.Cqp.Enum.LogerLevel)level, type, content);
        //添加日志
        public static int AddFatalError(string msg) => Common.CqApi.AddFatalError(msg);
        //添加致命错误提示
        public static int SetGroupWholeBanSpeak(long groupId, bool isOpen) => Common.CqApi.SetGroupWholeBanSpeak(groupId, isOpen);
        //置全群禁言
        public static int SetGroupSpecialTitle(long groupId, long qqId, string specialTitle, int time)
        {
            TimeSpan span = new TimeSpan(time / 60 / 60 / 24, time / 60 / 60 % 60, time / 60 % 60, time % 60);
            return Common.CqApi.SetGroupSpecialTitle(groupId, qqId, specialTitle, span);
        }
        //置群成员专属头衔

        public static int SetGroupAnonymousBanSpeak(long groupId, string anonymous, int time)
        {
            TimeSpan span = new TimeSpan(time / 60 / 60 / 24, time / 60 / 60 % 60, time / 60 % 60, time % 60);
            return Common.CqApi.SetGroupAnonymousBanSpeak(groupId, anonymous, span);
        }
        //置匿名成员禁言
        public static int SetGroupBanSpeak(long groupId, long qqId, int time)
        {
            TimeSpan span = new TimeSpan(time / 60 / 60 / 24, time / 60 / 60 % 60, time / 60 % 60, time % 60);
            return Common.CqApi.SetGroupBanSpeak(groupId, qqId, span);
        }
        //置成员禁言
        public static int SetFriendAddRequest(string tag,int respond,string msg) => Common.CqApi.SetFriendAddRequest(tag, (Sdk.Cqp.Enum.ResponseType)respond, msg);
        //置好友添加请求
        public static int SetGroupAddRequest(string tag, int request, int respond, string msg) => Common.CqApi.SetGroupAddRequest(tag, (Sdk.Cqp.Enum.RequestType)request, (Sdk.Cqp.Enum.ResponseType)respond, msg);
        //置群添加请求
        public static int SetGroupMemberNewCard(long groupId, long qqId, string newNick) => Common.CqApi.SetGroupMemberNewCard(groupId, qqId, newNick);
        //置群成员名片
        public static int SetGroupManager(long groupId, long qqId, bool isCalcel) => Common.CqApi.SetGroupManager(groupId, qqId, isCalcel);
        //置群管理员
        public static int SetAnonymousStatus(long groupId, bool isOpen) => Common.CqApi.SetAnonymousStatus(groupId, isOpen);
        //置群匿名设置
        public static int SetGroupMemberRemove(long groupId, long qqId, bool notAccept=false) => Common.CqApi.SetGroupMemberRemove(groupId, qqId, notAccept);
        //置群员移除
        public static int SetDiscussExit(long discussId) => Common.CqApi.SetDiscussExit(discussId);
        //置讨论组退出
        public static int SetGroupExit(long groupId) => Common.CqApi.SetGroupExit(groupId);
        // 置群退出


        /// <summary>
        /// 群内和QQ同时发送延时消息
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="qqId"></param>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <param name="time"></param>
        public static void SendDelyMessage(long groupId, long qqId, string m1, string m2, int time)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < time)//毫秒
            {
            }
            Common.CqApi.SendGroupMessage(groupId, m1);//调用处理事件
            Common.CqApi.SendPrivateMessage(qqId, m2);//调用处理事件
        }

        /// <summary>
        /// 快递即时查询
        /// </summary>
        /// <param name="shippercode">快递公司标识</param>
        /// <param name="logisticode">快递号</param>
        /// <param name="EBusinessID">用户id</param>
        /// <param name="AppKey">密钥</param>
        public static string NowSearch(string shippercode, string logisticode, string EBusinessID = "1577459", string AppKey = "c50d7be3-298a-4cb5-b765-cda725b9d728")
        {
            ExpressApi kd = new ExpressApi();
            return kd.getOrderTracesByJson(shippercode, logisticode, EBusinessID , AppKey );
        }

        /// <summary>
        /// Json方式  单号识别
        /// </summary>
        /// <param name="logisticode">快递号</param>
        /// <param name="EBusinessID">用户id</param>
        /// <param name="AppKey">密钥</param>
        public static string OrderSearch(string logisticode, string EBusinessID = "1577459", string AppKey = "c50d7be3-298a-4cb5-b765-cda725b9d728")
        {
            ExpressApi kd = new ExpressApi();
            return kd.orderTracesSubByJson(logisticode, EBusinessID, AppKey);
        }


        /// <summary>
        /// Json方式  物流信息订阅
        /// </summary>
        /// <returns></returns>
        public static string OrderSub(  string shippercode, string logisticode, 
                                        string recname, string recphone,string recpro,string recity,string recexp,string recaddr,
                                        string sename, string sephone, string secpro, string secity, string secexp, string secaddr,
                                        string EBusinessID = "1577459", string AppKey = "c50d7be3-298a-4cb5-b765-cda725b9d728")
        {
            ExpressApi kd = new ExpressApi();
            return kd.orderTracesSubByJson(  shippercode, logisticode,
                                             recname, recphone, recpro, recity, recexp, recaddr,
                                             sename,  sephone,  secpro, secity, secexp, secaddr,
                                             EBusinessID, AppKey );
        }
    }
}
