using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Zack.ComObjectHelpers;

namespace PowerPoint_Remote
{
    public class Server : IDisposable
    {
        public string IpAddress = "";
        public const int Port = 8080;
        public IWebHost Host;
        public dynamic Presentation;
        private COMReferenceTracker comReference = new();

        public Server()
        {
            Host = new WebHostBuilder().UseKestrel().Configure(ConfigureServer).UseUrls("http://*:" + Port).Build();
            Host.RunAsync();
            ReadIPV4();
        }

        public void OpenPresentation(string url)
        {
            dynamic app = T(PowerPointHelper.CreatePowerPointApplication());
            app.Visible = true;
            dynamic presentations = T(app.Presentations);
            Presentation = T(presentations.Open(url));
            T(Presentation.SlideShowSettings).Run();
        }

        private void ConfigureServer(IApplicationBuilder app)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.Run(async ctx =>
            {
                var req = ctx.Request;
                var res = ctx.Response;
                var path = req.Path.Value;
                res.ContentType = "application/json; charset=UTF-8";
                if (path == "/next")
                {
                    if (Presentation == null) return;
                    try
                    {
                        T(Presentation.SlideShowWindow).Activate();
                        T(T(Presentation.SlideShowWindow).View).Next();
                    }
                    catch (COMException) { }
                }
                else if (path == "/previous")
                {
                    if (Presentation == null) return;
                    try
                    {
                        T(Presentation.SlideShowWindow).Activate();
                        T(T(Presentation.SlideShowWindow).View).Previous();
                    }
                    catch (COMException) { }
                }
                else if (path == "/preview")
                {
                    if (Presentation == null) return;
                    {
                        res.ContentType = "image/jpeg";
                        await res.Body.WriteAsync(ScreenCaptureUtil.CaptureSlideShow().ToArray());
                    }
                }
            });
        }

        public void ClearComRefs()
        {
            try
            {
                if (Presentation != null)
                {
                    T(Presentation.Application).Quit();
                    Presentation = null;
                }
            }
            catch (COMException ex)
            {
                Debug.WriteLine(ex);
            }
            comReference.Dispose();
            comReference = new COMReferenceTracker();
        }

        private dynamic T(dynamic comObj) => comReference.T(comObj);

        private void ReadIPV4() => IpAddress = GetLocalIPv4(NetworkInterfaceType.Wireless80211);

        private static string GetLocalIPv4(NetworkInterfaceType type)
        {
            foreach (var ip in from item in NetworkInterface.GetAllNetworkInterfaces()
                               where item.NetworkInterfaceType == type && item.OperationalStatus == OperationalStatus.Up
                               from ip in item.GetIPProperties().UnicastAddresses
                               where ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                               select ip)
            {
                return ip.Address.ToString();
            }

            var addrL = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var ip in addrL)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) return ip.ToString();
            }
            return "";
        }

        public void Dispose()
        {
            ClearComRefs();
            Host.StopAsync();
            Host.WaitForShutdown();
            GC.SuppressFinalize(this);
        }
    }
}
