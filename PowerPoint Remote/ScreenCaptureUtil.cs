using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Timers;

namespace PowerPoint_Remote
{
    public static class ScreenCaptureUtil
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowA(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr handle, ref Rectangle rect);

        public static void CaptureSlideShow(object sender, ElapsedEventArgs e)
        {
            //powerpoints slideshow window
            var hwnd = FindWindowA("screenClass", null);
            if (hwnd != IntPtr.Zero) CaptureWindow(hwnd);
        }

        private static void CaptureWindow(IntPtr handle)
        {
            // Get the size of the window to capture
            Rectangle rect = new();
            GetWindowRect(handle, ref rect);

            // GetWindowRect returns Top/Left and Bottom/Right, so fix it
            rect.Width = rect.Width - rect.X;
            rect.Height = rect.Height - rect.Y;

            // Create a bitmap to draw the capture into
            using var bitmap = new Bitmap(rect.Width, rect.Height);
            // Use PrintWindow to draw the window into our bitmap
            using (var g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(0, 0, 0, 0, new Size(rect.Width, rect.Height));
            }
            bitmap.Save($"{AppDomain.CurrentDomain.BaseDirectory}/wwwroot/preview.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}
