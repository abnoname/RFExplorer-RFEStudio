using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using RFEStudio.Views;

namespace RFEStudio
{
    public partial class App : Application
    {
        public App(IRFExplorer_Device rf_dev)
        {
            InitializeComponent();

            MainPage = new CustomNavigationPage(new Main(), rf_dev);
        }
    }
}
