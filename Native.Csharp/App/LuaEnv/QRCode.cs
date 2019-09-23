using MessagingToolkit.QRCode.Codec;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace Native.Csharp.App.LuaEnv
{
    class QRCode
    {
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="data">信息</param>
        /// <returns>位图</returns>
        public static int QREncode(string data="0nly.cn")
        {
            string dpath = Common.AppDirectory;
            dpath = dpath.Substring(0, dpath.LastIndexOf("\\"));
            dpath = dpath.Substring(0, dpath.LastIndexOf("\\"));
            dpath = dpath.Substring(0, dpath.LastIndexOf("\\") + 1);
            try
            {
                QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                qrCodeEncoder.QRCodeScale = 5;
                qrCodeEncoder.QRCodeVersion = 0;

                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
                var pbImg = qrCodeEncoder.Encode(data, System.Text.Encoding.UTF8);
                var width = pbImg.Width / 10;
                var dwidth = width * 2;
                Bitmap bmp = new Bitmap(pbImg.Width + dwidth, pbImg.Height + dwidth);
                Graphics g = Graphics.FromImage(bmp);
                var c = System.Drawing.Color.White;
                g.FillRectangle(new SolidBrush(c), 0, 0, pbImg.Width + dwidth, pbImg.Height + dwidth);
                g.DrawImage(pbImg, width, width);
                g.Dispose();
                bmp.Save(dpath + "image\\qr.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "二维码生成错误", e.ToString());
                return -1;
            }
            return 1;
        }
        public static void QRDecode()
        {

        }



        /// <summary>  
        /// 调用此函数后使此两种图片合并，类似相册，有个  
        /// 背景图，中间贴自己的目标图片  
        /// </summary>  
        /// <param name="sourceImg">粘贴的源图片</param>  
        /// <param name="destImg">粘贴的目标图片</param>  
        public static void CombinImage(string imgBackname, string destImgname)
        {
            string dpath = Common.AppDirectory;
            dpath = dpath.Substring(0, dpath.LastIndexOf("\\"));
            dpath = dpath.Substring(0, dpath.LastIndexOf("\\"));
            dpath = dpath.Substring(0, dpath.LastIndexOf("\\") + 1);
            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(dpath + "image\\" + destImgname);        //照片图片    
            if (img.Height != 50 || img.Width != 50)
            {
                img = KiResizeImage(img, 30, 30, 0);
            }

                FileStream fs = new FileStream(dpath + "image\\"+imgBackname, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行保存
                BinaryReader br = new BinaryReader(fs);
                byte[] imgBytesIn = br.ReadBytes((int)fs.Length);
                using (MemoryStream ms = new MemoryStream(imgBytesIn))
                {
                    Image imgBack = Image.FromStream(ms);
                
                    Graphics g = Graphics.FromImage(imgBack);
            
                    g.DrawImage(imgBack, 0, 0, imgBack.Width, imgBack.Height);      //g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);   

                    //g.FillRectangle(System.Drawing.Brushes.White, imgBack.Width / 2 - img.Width / 2 - 1, imgBack.Width / 2 - img.Width / 2 - 1,1,1);//相片四周刷一层黑色边框  

                    //g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);  

                    g.DrawImage(img, imgBack.Width / 2 - img.Width / 2, imgBack.Width / 2 - img.Width / 2, img.Width, img.Height);
                    GC.Collect();
                    imgBack.Save(dpath + "image\\add.jpg");
                }
            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "创建二维码logo", e.ToString());
            }
        }

        /// <summary>  
        /// Resize图片  
        /// </summary>  
        /// <param name="bmp">原始Bitmap</param>  
        /// <param name="newW">新的宽度</param>  
        /// <param name="newH">新的高度</param>  
        /// <param name="Mode">保留着，暂时未用</param>  
        /// <returns>处理以后的图片</returns>  
        public static System.Drawing.Image KiResizeImage(System.Drawing.Image bmp, int newW, int newH, int Mode)
        {
            try
            {
                System.Drawing.Image b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);

                // 插值算法的质量  
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();

                return b;
            }
            catch
            {
                return null;
            }
        }
    }
}
