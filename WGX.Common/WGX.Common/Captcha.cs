using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using WGX.Common.Helper;

namespace WGX.Common
{
    /// <summary>
    /// 生成验证码
    /// </summary>
    public class Captcha : IDisposable
    {
        /// <summary>
        /// 宽度
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// 允许的字符
        /// </summary>
        public string AllowedLetters { get; private set; }

        /// <summary>
        /// 每个字符的长度
        /// </summary>
        public int CodeLength
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Code
        {
            get;
            private set;
        }

        private int PerWidth;

        private Font CodeFont;

        private float FontSize;

        private Random _rnd;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codeLength"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="allowedLetters"></param>
        /// <exception cref="ArgumentException"></exception>
        public Captcha(int codeLength = 4, int width = 160, int height = 40, string allowedLetters = "abcdefghjkmnpqrtuvwxyz12346789")
        {
            if (width < 1 || height < 1 || allowedLetters.Length < 1 || codeLength < 1)
                throw new ArgumentException();

            Width = width;
            Height = height;
            AllowedLetters = allowedLetters;
            FontSize = PerWidth = width / codeLength;
            if (FontSize > height)
                FontSize = height;

            CodeLength = codeLength;

            CodeFont = new Font("Courier New", FontSize, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] Render()
        {
            _rnd = new Random();
            using (var bm = new Bitmap(Width, Height))
            using (var g = Graphics.FromImage(bm))
            using (var msm = new MemoryStream())
            {
                Code = GetCode();
                SetBackground(g, bm.Width, bm.Height);
                PrintCode(g, Code.ToUpper());
                bm.Save(msm, ImageFormat.Jpeg);
                var datas = msm.GetBytes();
                return datas;
            }
        }

        private void PrintCode(Graphics g, string code)
        {

            for (var i = 0; i < code.Length; i++)
            {
                var c = code[i];

                var s = i * PerWidth; // -this.PerWidth / 4;
                var e = (i + 1) * PerWidth - PerWidth / 2;//+this.PerWidth / 2;

                var x = _rnd.Next(s, e);

                if (x < 0)
                    x = 0;

                if (x > Width - PerWidth)
                    x = Width - PerWidth;

                var y = _rnd.Next(0, (int)(Height - FontSize));

                PrintChar(g, c, x, y);
            }
        }

        private void PrintChar(Graphics g, char c, int x, int y)
        {
            var container = g.BeginContainer();
            var rect = new Rectangle(x, y, (int)FontSize, (int)FontSize);
            //var points = new Point[] { 
            //    new Point(x,y),
            //    new Point(x + (int)FontSize, y),
            //    new Point(x, (int)FontSize + y)
            //};

            var centerPoint = new Point(x + (int)FontSize / 2, y + (int)FontSize / 2);
            var angle = _rnd.Next(-45, 45);

            //var v = 4f;
            //PointF[] pfs = {
            //        new PointF(this.Rnd.Next(rect.Width) / v, this.Rnd.Next(rect.Height) / v),
            //        new PointF(rect.Width - this.Rnd.Next(rect.Width) / v, this.Rnd.Next(rect.Height) / v),
            //        new PointF(this.Rnd.Next(rect.Width) / v, rect.Height - this.Rnd.Next(rect.Height) / v),
            //        new PointF(rect.Width - this.Rnd.Next(rect.Width) / v, rect.Height - this.Rnd.Next(rect.Height) / v)
            //    };

            var c1 = Color.FromArgb(_rnd.Next(180, 255), _rnd.Next(0, 200), _rnd.Next(0, 255), _rnd.Next(0, 255));
            var c2 = Color.FromArgb(_rnd.Next(180, 255), _rnd.Next(0, 200), _rnd.Next(0, 255), _rnd.Next(0, 255));

            using (var brString = new HatchBrush((HatchStyle)_rnd.Next(0, 52), Color.White, Color.Transparent))
            using (var brGString = new LinearGradientBrush(rect, c1, c2, (LinearGradientMode)_rnd.Next(0, 3)))
            using (var sf = new StringFormat(StringFormatFlags.NoWrap)
            {
                Alignment = (StringAlignment)_rnd.Next(0, 3)
            })
            using (var gp = new GraphicsPath((FillMode)_rnd.Next(0, 1)))
            using (var matrix = new Matrix())
            {
                matrix.RotateAt(angle, centerPoint);
                gp.AddString("" + c, FontFamily.GenericSansSerif, (int)CodeFont.Style, CodeFont.Size, rect, sf);
                gp.Transform(matrix);

                g.FillPath(brString, gp);
                g.FillPath(brGString, gp);

                matrix.RotateAt(angle, centerPoint);
            }

            g.EndContainer(container);
        }

        private void SetBackground(Graphics g, int width, int height)
        {
            using (var brush = new HatchBrush((HatchStyle)_rnd.Next(0, 52), Color.DarkGray, Color.White))
            {
                g.FillRectangle(brush, new Rectangle(0, 0, width, height));
            }
        }

        private string GetCode()
        {
            var code = new char[CodeLength];
            for (var i = 0; i < CodeLength; i++)
            {
                var idx = _rnd.Next(0, AllowedLetters.Length - 1);
                code[i] = AllowedLetters[idx];
            }
            return string.Join("", code);
        }

        #region dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Captcha()
        {
            Dispose(false);
        }

        private bool _isDisposed;

        private void Dispose(bool flag)
        {
            if (!_isDisposed)
            {
                if (flag)
                {

                    if (CodeFont != null)
                        CodeFont.Dispose();
                    //if (this.CodeBrush != null)
                    //    this.CodeBrush.Dispose();
                }
                _isDisposed = true;
            }
        }
        #endregion
    }
}
