using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace PowerPoint_Remote
{
    public static class ScreenCaptureUtil
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowA(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr handle, ref Rectangle rect);

        public static MemoryStream CaptureSlideShow()
        {
            //powerpoints slideshow window
            var hwnd = FindWindowA("screenClass", null);
            if (hwnd != IntPtr.Zero) return CaptureWindow(hwnd);
            return new MemoryStream();
        }

        private static MemoryStream CaptureWindow(IntPtr handle)
        {
            var rect = new Rectangle();
            GetWindowRect(handle, ref rect);

            rect.Width = rect.Width - rect.X;
            rect.Height = rect.Height - rect.Y;
            var size = new Size(rect.Width, rect.Height);

            //original and resized img
            using var srcImage = new Bitmap(size.Width, size.Height);
            using var srcGraphics = Graphics.FromImage(srcImage);
            using var dstImage = new Bitmap(size.Width / 2, size.Height / 2);
            using var dstGraphics = Graphics.FromImage(dstImage);

            var src = new Rectangle(0, 0, size.Width, size.Height);
            var dst = new Rectangle(0, 0, size.Width / 2, size.Height / 2);

            srcGraphics.CopyFromScreen(0, 0, 0, 0, size);
            dstGraphics.DrawImage(srcImage, dst, src, GraphicsUnit.Pixel);

            var memoryStream = new MemoryStream();
            dstImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

            return memoryStream;
        }
    }
}
