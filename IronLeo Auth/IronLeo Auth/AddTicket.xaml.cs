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
    public partial class AddTicket : ContentPage
    {
        CryptoFunctions crypto = new CryptoFunctions();
        public AddTicket()
        {
            InitializeComponent();
            InitPickers();
            crypto.retrieveKeys();
        }

        public void InitPickers()
        {
            pickerTime.Time = DateTime.Now.AddHours(2).TimeOfDay;
            if (pickerTime.Time.Hours < 2)
            { 
                pickerDate.Date = pickerDate.Date.AddDays(1);
            }
        }

        private void Abort_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new MainPage();
        }
        public void Submit_Clicked(object sender, EventArgs e)
        {
            DateTime dateUTC = pickerDate.Date.Add(pickerTime.Time).ToUniversalTime();
            TimeSpan timeUTC = dateUTC.TimeOfDay;
            string pickerHours = Convert.ToString(timeUTC.Hours);
            string pickerMinutes = Convert.ToString(timeUTC.Minutes);
            if (pickerHours.Length == 1)
            {
                pickerHours = "0" + pickerHours;
            }
            if (pickerMinutes.Length == 1)
            {
                pickerMinutes = "0" + pickerMinutes;
            }
            string expireTime = dateUTC.Date.ToString("dd-MM-yyyy") + " " + pickerHours + ":" + pickerMinutes;
            ApiIntegration.apiDB("writedb", "{app;"+appPicker.SelectedItem+"}{token;"+tokenGen()+"}{expire;"+expireTime+"}");
            App.Current.MainPage = new MainPage();
        }

        public string tokenGen()
        {
            Random rand = new Random();
            int length = rand.Next(9, 12);
            string chars = "abcdefghjklmnpqrstuvwxyz23456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[rand.Next(s.Length)]).ToArray());
        }
    }
}