using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY_Rearview_Mirror.Functions
{
    public class ImageTool
    {
        #region 单例
        /// <summary>
        /// 单例
        /// </summary>
        private static ImageTool instance = null;
        private static readonly object syncRoot = new object();
        public static ImageTool GetInstance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new ImageTool();
                }
            }
            return instance;
        }
        #endregion

        public void Bitmap2HObjectBpp24(Bitmap bmp, out HObject image)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData srcBmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                HOperatorSet.GenImageInterleaved(out image, srcBmpData.Scan0, "bgr", bmp.Width, bmp.Height, 0, "byte", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmpData);
            }
            catch (Exception) { image = null; }
        }
    }
}
