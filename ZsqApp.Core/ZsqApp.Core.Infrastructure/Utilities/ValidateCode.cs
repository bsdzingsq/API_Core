using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ZsqApp.Core.Infrastructure.SysEnum;
using Qiniu.IO.Model;
using Qiniu.Http;
using Qiniu.IO;
using Qiniu.Util;

namespace ZsqApp.Core.Infrastructure.Utilities
{
    //**************操作记录******************
    //创建时间：2017.07.27
    //作者：陶林辉
    //内容描述：图形验证码
    //***************************************
    public class ValidateCode
    {
        private string code;
        private const int ImageHeigth = 22;             //验证码图片的高度
        private const double ImageLengthBase = 13;    //验证码图片中每个字符的宽度
        private const int ImageLineNumber = 10;         //噪音线的数量
        private const int ImagePointNumber = 50;       //噪点的数量
        private int length;
        public string key;      //此处用来保存验证码到Session的Key的
        private string url = "/imagecode/";//"/Users/taolinhui/Desktop/";//"D:/Banner/";
        // private string url = "/Users/linhuitao/Desktop/c统计后台模版/hAdmin的/";


        //设置验证码的长度和内容
        public ValidateCode()
        {
            this.length = 4;
            this.code = string.Empty;
        }

        ///<summary>
        ///产生随机的验证码并加入缓存
        ///</summary>
        /// <param name = "length" > 验证码长度 </ param >
        ///< returns ></ returns >
        public string CreateCode(int length)
        {
            if (length <= 0)
            {
                return string.Empty;
            }
            Random random = new Random();
            StringBuilder builder = new StringBuilder();
            //产生随机的验证码并拼接起来
            for (int i = 0; i < length; i++)
            {
                builder.Append(random.Next(0, 10));
            }
            this.code = builder.ToString();
            // MCachedHelper.Set<string>(key, code, DateTime.Now.AddMinutes(2));
            RedisHelper.StringSet(key, code, 2, RedisFolderEnum.code, RedisEnum.Four);
            return this.code;
        }

        private static char[] constant =
       {
        '0','1','2','3','4','5','6','7','8','9'
      };

        /// <summary>
        /// Creates the code.
        /// </summary>
        public string CreateCode()
        {
            Random random = new Random();
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < 4; i++) //i<6 生成的就是六位的
            {
                builder.Append(constant[random.Next(0, 10)]);
            }
            //if (random.Next(0, 10) > 5)
            //{
            //    builder.Append(random.Next(0, 10));
            //    builder.Append("+");
            //    builder.Append(random.Next(0, 10));
            //}
            //else
            //{
            //    builder.Append(random.Next(0, 10));
            //    builder.Append("x");
            //    builder.Append(random.Next(0, 10));
            //}
            // builder.Append("=?");
            this.code = builder.ToString();
            //MCachedHelper.Set<string>(key, code, DateTime.Now.AddMinutes(2));
            RedisHelper.StringSet(key, code, 2, RedisFolderEnum.code, RedisEnum.Four);
            return builder.ToString();

        }

        /// <summary>
        /// 根据长度产生验证码 , 并将验证码画成图片
        /// </summary>
        /// <param name="length">Length.</param>
        public void CreateValidateImage(int length)
        {
            this.CreateCode();
            this.CreateValidateImage(this.code);
        }

        /// <summary>
        /// 将验证码存储在七牛云上
        /// </summary>
        /// <param name="localFile"></param>
        /// <param name="saveKey"></param>
        public static void UploadFile(string localFile, string saveKey)
        {
            #region
            Mac mac = new Mac("YxS5pPQ3GYdwsnUgkqnCOeJA2CXtP-vtGeSEOP6D", "dK69Cs4SMo3bnZA3KIb1uBAYBe-uGmuRv_HtPJcm");
            // 上传策略，参见 
            // https://developer.qiniu.com/kodo/manual/put-policy
            PutPolicy putPolicy = new PutPolicy();
            // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
            // putPolicy.Scope = bucket + ":" + saveKey;  
            putPolicy.Scope = "img-s-code";
            // 上传策略有效期(对应于生成的凭证的有效期)          
            putPolicy.SetExpires(3600);
            // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除
            putPolicy.DeleteAfterDays = 1;
            string jstr = putPolicy.ToJsonString();
            //获取上传凭证
            var uploadToken = Auth.CreateUploadToken(mac, jstr);
            Qiniu.Common.Config.AutoZone("YxS5pPQ3GYdwsnUgkqnCOeJA2CXtP-vtGeSEOP6D", "img-s-code", false);
            FormUploader upLoader = new FormUploader(false);
            HttpResult result = upLoader.UploadFile(localFile, saveKey, uploadToken);
            #endregion

            if (result.Code != 200)
            {
                throw new Exception(result.RefText);
            }
            return;
        }

        /// <summary>
        ///  根据产生的验证码生成图片
        /// </summary>
        /// <param name="code">Code.</param>
        public void CreateValidateImage(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                //this.Session[key] = code;
                //初始化位图Bitmap对象，指定图片对象的大小(宽,高)
                Bitmap image = new Bitmap((int)Math.Ceiling((double)(code.Length * ImageLengthBase)), ImageHeigth);
                //初始化一块画布
                Graphics graphics = Graphics.FromImage(image);
                Random random = new Random();
                try
                {
                    int num5;
                    graphics.Clear(Color.White);
                    //绘制噪音线
                    for (num5 = 0; num5 < ImageLineNumber; num5++)
                    {
                        int num = random.Next(image.Width);
                        int num3 = random.Next(image.Height);
                        int num2 = random.Next(image.Width);
                        int num4 = random.Next(image.Height);
                        graphics.DrawLine(new Pen(Color.Silver), num, num3, num2, num4);
                    }
                    //验证码字体样式
                    Font font = new Font("Tahoma", 10, FontStyle.Italic | FontStyle.Bold);
                    LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                    //绘制验证码到画布
                    graphics.DrawString(code, font, brush, (float)3, (float)2);
                    //绘制噪点
                    for (num5 = 0; num5 < ImagePointNumber; num5++)
                    {
                        int x = random.Next(image.Width);
                        int y = random.Next(image.Height);
                        image.SetPixel(x, y, Color.FromArgb(random.Next()));
                    }
                    graphics.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                    //保存图片
                    if (!Directory.Exists(url))
                    {
                        Directory.CreateDirectory(url);//如果文件夹不存在就创建它
                    }
                    image.Save(url + key + ".gif", ImageFormat.Gif);

                    UploadFile(url + key + ".gif", key + ".gif");
                }
                finally
                {
                    graphics.Dispose();
                    image.Dispose();
                }

            }

        }

        //Properties
        public string Code { get; set; }

        public int Length
        {
            get
            {
                return this.length;
            }
            set
            {
                this.length = value;
            }
        }
    }
}
