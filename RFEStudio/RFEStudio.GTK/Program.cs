using System;
using RFEStudio.GTK.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

namespace RFEStudio.GTK
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Gtk.Application.Init();
            Forms.Init();
            PlotViewRenderer.Init();

            RFExplorer_Device rfdev = new RFExplorer_Device();

            var app = new App(rfdev);
            var window = new FormsWindow();
            window.LoadApplication(app);
            window.SetApplicationTitle("RFEStudio");
            window.Resize(800, 480);            
            window.Show();

            Gtk.Application.Run();

            rfdev.Close();
        }
    }
}