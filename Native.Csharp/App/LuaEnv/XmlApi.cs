using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Native.Csharp.App.LuaEnv
{
    class XmlApi
    {
        public const string rootNode = "Categories";
        public const string keyNode = "msginfo";
        public const string keyName = "msg";
        public const string valueName = "ans";

        public static string path = Common.AppDirectory + "xml/";
        
        public static void set(string pth ,string group, string msg, string str)
        {
            del(pth, group, msg);
            insert(pth, group, msg, str);
        }

        /// <summary>
        /// 随机获取一条结果
        /// </summary>
        /// <param name="group"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string replay_get(string pth, string group, string msg)
        {
            string tpath = dircheck(pth, group);
            XElement root = XElement.Load(tpath + group + ".xml");
            Random ran = new Random(System.DateTime.Now.Millisecond);
            int RandKey;
            string ansall = "";
            var element = from ee in root.Elements()
                          where msg.IndexOf(ee.Element(keyName).Value) != -1
                          select ee;
            XElement[] result = element.ToArray();
            if (result.Count() > 0)
            {
                RandKey = ran.Next(0, result.Count());
                ansall = result[RandKey].Element(valueName).Value;
            }
            return ansall;
        }

        public static string xml_get(string pth, string group, string msg)
        {
            string tpath = dircheck(pth, group);
            XElement root = XElement.Load(tpath + group + ".xml");
            string ansall = "";
            var element = from ee in root.Elements()
                          where ee.Element(keyName).Value == msg
                          select ee;
            if (element.Count() > 0)
                ansall = element.First().Element(valueName).Value;
            return ansall;
        }

        public static string xml_row(string pth, string group, string msg)
        {
            string tpath = dircheck(pth, group);
            XElement root = XElement.Load(tpath + group + ".xml");
            string ansall = "";
            var element = from ee in root.Elements()
                          where ee.Element(valueName).Value == msg
                          select ee;
            if (element.Count() > 0)
                ansall = element.First().Element(keyName).Value;
            return ansall;
        }

        public static string alist_get(string pth, string group, string msg)
        {
            string tpath = dircheck(pth, group); 
            XElement root = XElement.Load(tpath + group + ".xml");
            string ansall = "";
            var element = from ee in root.Elements()
                          where ee.Element(keyName).Value == msg
                          select ee;
            XElement[] result = element.ToArray();
            foreach (XElement mm in result)
                ansall = ansall + mm.Element(valueName).Value + "\r\n";
            ansall = ansall + "一共有" + element.Count() + "条回复";
            return ansall;
        }

        public static void del(string pth, string group, string msg)
        {
            string tpath = dircheck(pth, group);
            string gg = group.ToString();
            XElement root = XElement.Load(tpath + group + ".xml");

            var element = from ee in root.Elements()
                          where (string)ee.Element(keyName) == msg
                          select ee;
            if (element.Count() > 0)
                element.Remove();
            root.Save(tpath + group + ".xml");
        }

        public static void remove(string pth, string group, string msg, string ans)
        {
            string tpath = dircheck(pth, group);
            string gg = group.ToString();
            XElement root = XElement.Load(tpath + group + ".xml");

            var element = from ee in root.Elements()
                          where (string)ee.Element(keyName) == msg && (string)ee.Element(valueName) == ans
                          select ee;
            if (element.Count() > 0)
                element.First().Remove();
            root.Save(tpath + group + ".xml");
        }


        public static void insert(string pth, string group, string msg, string ans)
        {
            if (msg.IndexOf("\r\n") < 0 & msg != "")
            {
                string tpath = dircheck(pth, group);
                XElement root = XElement.Load(tpath + group + ".xml");

                XElement read = root.Element(keyNode);

                if (read == null)
                {
                    root.Add(new XElement(keyNode,
                      new XElement(keyName, msg),
                      new XElement(valueName, ans)
                      ));
                }
                else
                {
                    read.AddBeforeSelf(new XElement(keyNode,
                      new XElement(keyName, msg),
                      new XElement(valueName, ans)
                      ));
                }
                root.Save(tpath + group + ".xml");
            }
        }

        public static ArrayList nlist_get(string pth, string group)
        {
            string tpath = dircheck(pth, group);
            XElement root = XElement.Load(tpath + group + ".xml");
            var element = from ee in root.Elements() select ee.Element(keyName);
            XElement[] result = element.ToArray();
            ArrayList list = new ArrayList();
            ArrayList arry = new ArrayList();
            list.Add(result.Length - 2 );
            foreach (XElement mm in result)
                arry.Add(mm.Value);
            list.Add(arry);
            return list;
        }

        public static string dircheck(string pth, string group)
        {
            string tpath;
            if (pth != "" & pth != null)
            {
                tpath = path + pth + "/";
            }
            else
            {
                tpath = path;
            }
            if (!Directory.Exists(tpath))
            {
                Directory.CreateDirectory(tpath);
            }
            if (!File.Exists(tpath + group + ".xml"))
            {
                XElement root = new XElement(rootNode,
                    new XElement(keyNode,
                        new XElement(keyName, "初始问题"),
                        new XElement(valueName, "初始回答")
                        )
                   );
                root.Save(tpath + group + ".xml");
            }
            return tpath;
        }
    }
}
