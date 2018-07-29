using Xamarin.Forms;
using RFEStudio.ViewModels;

namespace RFEStudio.Views
{
    public partial class CustomNavigationPage : NavigationPage
    {
        private ViewModel m_viewmodel;

        public CustomNavigationPage(Page root, IRFExplorer_Device rf_dev) : base(root)
        {
            InitializeComponent();

            m_viewmodel = new ViewModel(Navigation, rf_dev);

            BindingContext = m_viewmodel;
        }
    }
}