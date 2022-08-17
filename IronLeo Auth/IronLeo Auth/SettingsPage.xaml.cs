using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IronLeo_Auth
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            populateEntryText();
        }

        private void populateEntryText()
        {
            if (Application.Current.Properties.ContainsKey("cryptoIV"))
            {
                cryptoIV.Text = Application.Current.Properties["cryptoIV"] as string;
            }

            if (Application.Current.Properties.ContainsKey("clientKey"))
            {
                clientKey.Text = Application.Current.Properties["clientKey"] as string;
            }

            if (Application.Current.Properties.ContainsKey("serverKey"))
            {
                serverKey.Text = Application.Current.Properties["serverKey"] as string;
            }

            if (Application.Current.Properties.ContainsKey("apiUrl"))
            {
                apiUrl.Text = Application.Current.Properties["apiUrl"] as string;
            }
        }

        private void submitButton_Pressed(object sender, EventArgs e)
        {
            Application.Current.Properties["serverKey"] = serverKey.Text;
            Application.Current.Properties["clientKey"] = clientKey.Text;
            Application.Current.Properties["cryptoIV"] = cryptoIV.Text;
            Application.Current.Properties["apiUrl"] = apiUrl.Text;
            Application.Current.SavePropertiesAsync();
            DependencyService.Get<IMessage>().ShortAlert("Done");
        }

        private void backbtn_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new MainPage();
        }
    }
}