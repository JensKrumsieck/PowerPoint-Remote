using Microsoft.Win32;
using SkiaSharp.QrCode.Image;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Media.Imaging;
using ThemeCommons.Controls;

namespace PowerPoint_Remote
{
    public partial class MainWindow : DefaultWindow
    {
        private Server server;
        private Timer timer;
        public MainWindow()
        {
            InitializeComponent();
            server = new Server();
            Closing += (s, e) =>
            {
                timer.Stop();
                server.Dispose();
            };
        }

        private void BuildQrCode()
        {
            var stream = new MemoryStream();
            var qrcode = new QrCode(server.IpAddress + ":" + Server.Port, new Vector2Slim(512, 512), SkiaSharp.SKEncodedImageFormat.Png);
            qrcode.GenerateImage(stream);
            QrImg.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "PowerPoint Presentation (.ppt(x)) |*.ppt;*.pptx"
            };
            if (ofd.ShowDialog(this) != true) return;
            PPTPath.Text = ofd.FileName;
            server.OpenPresentation(PPTPath.Text);
            BuildQrCode();
            Activate();
            StartSlideShowTimer();
        }

        private void StartSlideShowTimer()
        {
            timer = new Timer(500)
            {
                AutoReset = true,
                Enabled = true
            };
            timer.Elapsed += ScreenCaptureUtil.CaptureSlideShow;
        }
    }
}
