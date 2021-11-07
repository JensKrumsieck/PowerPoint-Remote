using Microsoft.Win32;
using SkiaSharp.QrCode.Image;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using ThemeCommons.Controls;

namespace PowerPoint_Remote
{
    public partial class MainWindow : DefaultWindow
    {
        private Server server;
        public MainWindow()
        {
            InitializeComponent();
            server = new Server();
            Closing += (s, e) => server.Dispose();
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
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) =>
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
    }
}
