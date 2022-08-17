using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.IO;
using System.Net;

namespace IronLeo_Auth
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        CryptoFunctions crypto = new CryptoFunctions();
        public MainPage()
        {
            InitializeComponent();
            crypto.retrieveKeys();
            interpretDBResponse();
        }

        public Task interpretDBResponse()
        {
            if (!string.IsNullOrEmpty(crypto.cryptoIV) && !string.IsNullOrEmpty(crypto.serverKey) && !string.IsNullOrEmpty(crypto.clientKey))
            {
                datagrid.Children.Clear();
                string dbResponse = ApiIntegration.apiDB("readdb");
                string[] rows = { };

                if (dbResponse != "")
                {
                    rows = crypto.decryptToken(dbResponse).Split('}');
                }

                foreach (string item in rows)
                {
                    if (item != "")
                    {
                        string[] values = rows[Array.IndexOf(rows, item)].Split(';');
                        DateTime endDate = DateTime.ParseExact(values[3], "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                        DateTime currentDate = DateTime.UtcNow;
                        TimeSpan timeDifference = endDate - currentDate;
                        string timeLeft = timeDifference.Days + "d, " + timeDifference.Hours + "h, " + timeDifference.Minutes + "m";
                        buildList(values, timeLeft, endDate.ToLocalTime().ToString("dd.MM.yy HH:mm"), Array.IndexOf(rows, item));
                    }
                }
            }
            return Task.CompletedTask;
        }

        public void buildList(string[] data, string timeLeft, string endTime, int position)
        {
            datagrid.RowDefinitions.Add(new RowDefinition { Height = 100 });
            
            Grid grid = new Grid
            {
                ColumnDefinitions = {
                    new ColumnDefinition { Width = 160 },
                    new ColumnDefinition { Width = 160 }
                },
                RowDefinitions = {
                    new RowDefinition { Height = 25 },
                    new RowDefinition { Height = 25 }
                }
            };

            grid.Children.Add(
                new Label
                {
                    Text = data[1],
                    Padding = new Thickness(10, 0),
                    FontSize = 20,
                    VerticalOptions = LayoutOptions.Center
                }, 0, 0);

            Button CopyToken = new Button
            {
                Scale = 1.5,
                Text = data[2],
                Padding = new Thickness(10, 0),
                FontSize = 15,
                VerticalOptions = LayoutOptions.Center,
            };
            CopyToken.Clicked += (sender, e) => OnCopyTokenClicked(sender, e, data[2], position);
            grid.Children.Add(CopyToken, 0, 1);

            grid.Children.Add(
                new Label
                {
                    Text = endTime,
                    Padding = new Thickness(10, 0),
                    FontSize = 20,
                    VerticalOptions = LayoutOptions.Center
                }, 1, 0);

            grid.Children.Add(
                new Label
                {
                    Text = timeLeft,
                    Padding = new Thickness(10, 0),
                    FontSize = 20,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.End
                }, 1, 1);

                ImageButton DeleteBtn = new ImageButton
                {
                    BackgroundColor = Color.Transparent,
                    Source = "delete.png",
                    Padding = new Thickness(10, 0),
                    VerticalOptions = LayoutOptions.Start
                };
            DeleteBtn.Clicked += (sender, e) => OnDeleteBtnClicked(sender, e, position);

            datagrid.Children.Add(DeleteBtn, 1, position);
            datagrid.Children.Add(grid, 0, position);
        }

        private void OnDeleteBtnClicked(object sender, EventArgs e, int position)
        {
            string[] rows = crypto.decryptToken(ApiIntegration.apiDB("readdb")).Split('}');
            string[] values = rows[position].Split(';');
            string ticketId = values[0].Replace("{", "");
            ApiIntegration.apiDB("writedb", "{id;" + ticketId + "}");
            App.Current.MainPage = new MainPage();
        }

        private async void OnCopyTokenClicked(object sender, EventArgs e, string token, int position)
        {
            if (token.ToUpper() == "INACTIVE")
            {
                OnDeleteBtnClicked(sender, e, position);
                AddTicket addTicket = new AddTicket();
                DateTime expireDateTime = DateTime.UtcNow.AddMinutes(3);
                string expireTime = expireDateTime.ToString("dd-MM-yyyy HH:mm");
                ApiIntegration.apiDB("writedb", "{app;Admin}{token;" + addTicket.tokenGen() + "}{expire;" + expireTime + "}");
                App.Current.MainPage = new MainPage();
            }
            else
            {
                await Clipboard.SetTextAsync(token.ToUpper());
                DependencyService.Get<IMessage>().ShortAlert("Copied");
            }
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new AddTicket();
        }

        private async void refreshView_Refreshing(object sender, EventArgs e) {
            await interpretDBResponse();
            refreshView.IsRefreshing = false;
        }

        private void settingsbtn_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new SettingsPage();
        }
    }

    public interface IMessage
    {
        void ShortAlert(string message);
        void LongAlert(string message);
    }
}
