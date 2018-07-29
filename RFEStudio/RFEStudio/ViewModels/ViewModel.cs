using RFEStudio.ViewModels.Base;
using Xamarin.Forms;

namespace RFEStudio.ViewModels
{
    public partial class ViewModel : ViewModelBase
	{
        private INavigation m_Navigation;

        private bool m_IsBusy;
        private IRFExplorer_Device m_rf_dev;


        public ViewModel (INavigation MainPageNav, IRFExplorer_Device rf_dev)
		{
            m_IsBusy = false;

            m_rf_dev = rf_dev;
            m_Navigation = MainPageNav;

            InitializeCommands_Main();
            InitializeCommands_Notify();

            Setup_RFDevice();
            Setup_Oxyplot();
            Setup_Transceive_Task();
		}

        public bool is_busy
        {
            set
            {
                if (m_IsBusy != value)
                {
                    m_IsBusy = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return m_IsBusy;
            }
        }
	}
}