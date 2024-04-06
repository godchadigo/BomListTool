using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BomListTool.Manager
{
    public static class ImageHelper
    {
        public static string ConvertImageToBase64(string imagePath)
        {
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
        public static string ConvertImageToBase64(Image img)
        {
            // 创建图像的深层副本
            using (Bitmap bitmapCopy = new Bitmap(img))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // 保存副本到内存流，防止因为使用原始图像对象而导致的GDI+错误
                    bitmapCopy.Save(ms, img.RawFormat);

                    // 从内存流创建Base64字符串
                    string base64String = Convert.ToBase64String(ms.GetBuffer());

                    return base64String;
                }
            }
        }

        public static void DisplayBase64InPictureBox(string base64String, PictureBox pictureBox)
        {
            if (base64String == null) return;
            if (base64String.Length == 0) return;
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                Image image = Image.FromStream(ms);
                pictureBox.Image = image;
            }
        }
    }
}
