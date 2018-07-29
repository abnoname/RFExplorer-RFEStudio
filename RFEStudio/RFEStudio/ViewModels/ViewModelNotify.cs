using System;
using System.ComponentModel;
using System.Windows.Input;
using RFEStudio.Views;
using Xamarin.Forms;

namespace RFEStudio.ViewModels
{
    public partial class ViewModel
    {
		private String m_notify_text = "";

		public ICommand cmdNotifyOK { protected set; get; }

        public void InitializeCommands_Notify ()
        {
			cmdNotifyOK = new Command<string> (async (o) =>
                {
					await m_Navigation.PopAsync (); 
                });
        }

		public void Notify_Show(String msg)
		{
			Notify_Text = msg;

			Device.BeginInvokeOnMainThread (() => { 
				m_Navigation.PushAsync (new Notify ()); 
			});
		}

		public void Notify_Show_Alt(String msg)
		{
			Notify_Text = msg;

			Device.BeginInvokeOnMainThread (() =>
			{
				Page p = new Page ();
				p.DisplayAlert ("Notify", msg, "OK");
			});
		}

		public String Notify_Text
		{
			set
			{
				if (m_notify_text != value)
				{
					m_notify_text = value;
                    OnPropertyChanged();
				}
			}
			get
			{
				return m_notify_text;
			}            
		}
    }
}