using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;

namespace Native.Csharp.App.LuaEnv
{

    class ExpressApi
    {
        /// <summary>
        /// Json方式  单号识别
        /// </summary>
        /// <returns></returns>
        public string orderTracesSubByJson(string logisticode, string EBusinessID, string AppKey)
        {
            string requestData = "{'LogisticCode': '" + logisticode + "'}";

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("RequestData", HttpUtility.UrlEncode(requestData, Encoding.UTF8));
            param.Add("EBusinessID", EBusinessID);
            param.Add("RequestType", "2002");
            string dataSign = encrypt(requestData, AppKey, "UTF-8");
            param.Add("DataSign", HttpUtility.UrlEncode(dataSign, Encoding.UTF8));
            param.Add("DataType", "2");

            string result = sendPost("http://api.kdniao.com/Ebusiness/EbusinessOrderHandle.aspx", param);

            //根据公司业务处理返回的信息......

            return result;
        }


        /// <summary>
        /// Json方式 查询订单物流轨迹
        /// </summary>
        /// <returns></returns>
        public string getOrderTracesByJson(string shippercode, string logisticode, string EBusinessID, string AppKey)
        {
            string requestData = "{'OrderCode':'','ShipperCode':'" + shippercode+ "','LogisticCode':'" + logisticode + "'}";

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("RequestData", HttpUtility.UrlEncode(requestData, Encoding.UTF8));
            param.Add("EBusinessID", EBusinessID);
            param.Add("RequestType", "1002");
            string dataSign = encrypt(requestData, AppKey, "UTF-8");
            param.Add("DataSign", HttpUtility.UrlEncode(dataSign, Encoding.UTF8));
            param.Add("DataType", "2");

            string result = sendPost("http://api.kdniao.com/Ebusiness/EbusinessOrderHandle.aspx", param);

            //根据公司业务处理返回的信息......

            return result;
        }


        /// <summary>
        /// Json方式  物流信息订阅
        /// </summary>
        /// <returns></returns>
        public string orderTracesSubByJson( string shippercode, string logisticode,
                                            string sename, string sephone, string secpro, string secity, string secexp, string secaddr,
                                            string recname, string recphone, string recpro, string recity, string recexp, string recaddr, 
                                            string EBusinessID, string AppKey)
        {
            string requestData = "{'OrderCode': ''," +
                                "'ShipperCode':'"+ shippercode + "'," +
                                "'LogisticCode':'"+ logisticode + "'," +
                                "'PayType':1," +
                                "'ExpType':1," +
                                "'CustomerName':''," +
                                "'CustomerPwd':''," +
                                "'MonthCode':''," +
                                "'IsNotice':0," +
                                "'Cost':1.0," +
                                "'OtherCost':1.0," +
                                "'Sender':" +
                                "{" +
                                "'Company':'LV','Name':'"+ sename + "','Mobile':'"+ sephone + "','ProvinceName':'"+ secpro + "','CityName':'"+ secity + "','ExpAreaName':'"+ secexp + "','Address':'"+ secaddr + "'}," +
                                "'Receiver':" +
                                "{" +
                                "'Company':'GCCUI','Name':'"+ recname + "','Mobile':'"+ recphone + "','ProvinceName':'"+ recpro + "','CityName':'"+ recity + "','ExpAreaName':'"+ recexp + "','Address':'"+ recaddr + "'}," +
                                "'Commodity':" +
                                "[{" +
                                "'GoodsName':'','Goodsquantity':1,'GoodsWeight':1.0}]," +
                                "'Weight':1.0," +
                                "'Quantity':1," +
                                "'Volume':0.0," +
                                "'Remark':''}";

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("RequestData", HttpUtility.UrlEncode(requestData, Encoding.UTF8));
            param.Add("EBusinessID", EBusinessID);
            param.Add("RequestType", "1008");
            string dataSign = encrypt(requestData, AppKey, "UTF-8");
            param.Add("DataSign", HttpUtility.UrlEncode(dataSign, Encoding.UTF8));
            param.Add("DataType", "2");

            string result = sendPost("http://api.kdniao.com/api/dist", param);

            //根据公司业务处理返回的信息......

            return result;
        }

        /// <summary>
        /// Post方式提交数据，返回网页的源代码
        /// </summary>
        /// <param name="url">发送请求的 URL</param>
        /// <param name="param">请求的参数集合</param>
        /// <returns>远程资源的响应结果</returns>
        private static string sendPost(string url, Dictionary<string, string> param)
        {
            string result = "";
            StringBuilder postData = new StringBuilder();
            if (param != null && param.Count > 0)
            {
                foreach (var p in param)
                {
                    if (postData.Length > 0)
                    {
                        postData.Append("&");
                    }
                    postData.Append(p.Key);
                    postData.Append("=");
                    postData.Append(p.Value);
                }
            }
            byte[] byteData = Encoding.GetEncoding("UTF-8").GetBytes(postData.ToString());
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Referer = url;
                request.Accept = "*/*";
                request.Timeout = 30 * 1000;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
                request.Method = "POST";
                request.ContentLength = byteData.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(byteData, 0, byteData.Length);
                stream.Flush();
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream backStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(backStream, Encoding.GetEncoding("UTF-8"));
                result = sr.ReadToEnd();
                sr.Close();
                backStream.Close();
                response.Close();
                request.Abort();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        ///<summary>
        ///电商Sign签名
        ///</summary>
        ///<param name="content">内容</param>
        ///<param name="keyValue">Appkey</param>
        ///<param name="charset">URL编码 </param>
        ///<returns>DataSign签名</returns>
        private string encrypt(String content, String keyValue, String charset)
        {
            if (keyValue != null)
            {
                return base64(MD5(content + keyValue, charset), charset);
            }
            return base64(MD5(content, charset), charset);
        }

        ///<summary>
        /// 字符串MD5加密
        ///</summary>
        ///<param name="str">要加密的字符串</param>
        ///<param name="charset">编码方式</param>
        ///<returns>密文</returns>
        private string MD5(string str, string charset)
        {
            byte[] buffer = System.Text.Encoding.GetEncoding(charset).GetBytes(str);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider check;
                check = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] somme = check.ComputeHash(buffer);
                string ret = "";
                foreach (byte a in somme)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("X");
                    else
                        ret += a.ToString("X");
                }
                return ret.ToLower();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// base64编码
        /// </summary>
        /// <param name="str">内容</param>
        /// <param name="charset">编码方式</param>
        /// <returns></returns>
        private string base64(String str, String charset)
        {
            return Convert.ToBase64String(Encoding.GetEncoding(charset).GetBytes(str));
        }

    }
}
