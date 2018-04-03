using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stylet;
using System.Windows;
using DotaHackLoader.Services;
using System.Globalization;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Net;
using Microsoft.Win32;
using System.IO;
using Newtonsoft.Json;
using DotaHackLoader.Models;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace DotaHackLoader.ViewModels
{
    public class MainViewModel : Conductor<ITabItem>.Collection.OneActive
    {
        public bool IsLoading { get; set; }

        private Storyboard tabAnim;

        private IScreen _errorBox;
        public IScreen ErrorBox
        {
            get { return this._errorBox; }
            private set { this.SetAndNotify(ref this._errorBox, value); }
        }

        private IScreen _warningBox;
        public IScreen WarningBox
        {
            get { return this._warningBox; }
            private set { this.SetAndNotify(ref this._warningBox, value); }
        }

        public string ProcessingText { get; set; }

        public MainViewModel()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.language))
            {
                if (CultureInfo.CurrentUICulture.Name == "ru-RU")
                    Properties.Settings.Default.language = "ru-RU";
                else Properties.Settings.Default.language = "en-EN";
                Properties.Settings.Default.Save();
            }

            var loginView = new LoginViewModel();
            var storeView = new StoreViewModel();
            var profileView = new ProfileViewModel();
            var settingsView = new SettingsViewModel();

            DisplayName = "Multihack 2017";
            Items.Add(storeView);
            Items.Add(profileView);
            Items.Add(settingsView);
            Items.Add(loginView);
        }

        

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            if (string.IsNullOrEmpty(Properties.Settings.Default.userToken))
                ShowLogin();
            else
            {
                HttpService.Token = Properties.Settings.Default.userToken;
                Initiation();
            }
        }

        public void ShowLogin()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.userToken))
            {
                Properties.Settings.Default.userToken = "";
                Properties.Settings.Default.Save();
            }
            (Items[0] as StoreViewModel).IsEnabled = false;
            (Items[1] as ProfileViewModel).IsEnabled = false;
            (Items[2] as SettingsViewModel).IsEnabled = false;
            (Items[3] as LoginViewModel).IsEnabled = true;
            this.ActiveItem = Items[3];
            var tabAnim = (this.View as Window).FindResource("TabAnimation") as Storyboard;
            tabAnim.Begin();
        }

        public async void Initiation()
        {
            try
            {
                (Items.ElementAt(3) as LoginViewModel).IsEnabled = false;
                IsLoading = true;
                ProcessingText = "Loading user's data...";
                var response = await HttpService.GetUserDataAsync();
                if (response.Item1)
                {
                    IsLoading = false;
                    ShowError(response.Item2);
                    ShowLogin();
                    return;
                }
                if (response.Item2.Contains("token_failed"))
                {
                    IsLoading = false;
                    ShowError("Authorization token is expired. Please sign in again.");
                    ShowLogin();
                    return;
                }
                var userData = JsonConvert.DeserializeObject<UserData>(response.Item2);
                var buyLinks = JsonConvert.DeserializeObject<BuyLinks>(userData.buy.ToString());
                ProcessingText = "Processing user's data...";
                var hwid = await Task.Run(() => UniqueProvider.Value());
                if (userData.hwid == "0")
                {
                    response = await HttpService.GetHwidAsync(hwid);
                    if (response.Item1)
                    {
                        IsLoading = false;
                        ShowError(response.Item2);
                        ShowLogin();
                        return;
                    }
                    userData.hwid = hwid;
                }

                if (userData.hwid != hwid)
                {
                    IsLoading = false;
                    if (Properties.Settings.Default.language == "ru-RU")
                        ShowError("Ваш HWID изменился и не совпадает с привязанным, воспользуйтесь услугой сброса на сайте в профиле");
                    else ShowError("Your HWID is changed and doesn't match with previous one, use hwid reset service via the profile page on the website");
                    ShowLogin();
                    return;
                }
                ProcessingText = "Getting current prices...";
                response = await HttpService.GetPricesAsync();
                if (response.Item1)
                {
                    IsLoading = false;
                    ShowError(response.Item2);
                    ShowLogin();
                    return;
                }
                var prices = JsonConvert.DeserializeObject<PriceData[]>(response.Item2);
                userData.username = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(userData.username);
                userData.gender[1] = Properties.Settings.Default.language == "ru-RU" ? userData.gender[0] == "0" ? "Мужчина" : "Женщина" : userData.gender[0] == "0" ? "Male" : "Female";
                userData.avatar = $"http://{userData.avatar}";
                var registerData = UnixTimeStampToDateTime(Convert.ToDouble(userData.register_date));
                userData.register_date = $"{registerData.ToShortDateString()} {registerData.ToShortTimeString()}";
                var daysLeftValue = Convert.ToDouble(userData.days_left);
                var daysLeftTime = UnixTimeStampToDateTime(daysLeftValue);
                if (daysLeftValue == 0 || daysLeftTime < DateTime.Now)
                    userData.days_left = "подписка не куплена";
                else userData.days_left = $"{daysLeftTime.ToShortDateString()} {daysLeftTime.ToShortTimeString()}";
                (Items[0] as StoreViewModel).IsEnabled = true;
                (Items[0] as StoreViewModel).Prices = prices;
                (Items[0] as StoreViewModel).BuyLinks = buyLinks;
                (Items[1] as ProfileViewModel).IsEnabled = true;
                (Items[1] as ProfileViewModel).UserData = userData;
                (Items[2] as SettingsViewModel).IsEnabled = true;
                (Items[2] as SettingsViewModel).UserData = userData;
                this.ActiveItem = Items[0];

                IsLoading = false;
                tabAnim = (this.View as Window).FindResource("TabAnimation") as Storyboard;
                tabAnim.Begin();
            }
            catch (Exception ex)
            {
                IsLoading = false;
                ShowError(ex.Message);
                ShowLogin();
                return;
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ((this.View as Window).FindName("DragableGrid") as Grid).MouseLeftButtonDown += View_MouseLeftButtonDown;
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            ((this.View as Window).FindName("DragableGrid") as Grid).MouseLeftButtonDown -= View_MouseLeftButtonDown;
        }

        private void View_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (this.View as Window).DragMove();
        }

        public void ShowError(string message, string customTitle = null)
        {
            CloseError();
            this.ErrorBox = new ErrorBoxViewModel(message)
            {
                Parent = this,
                CustomTitleContent = customTitle
            };
        }

        public void ShowWarning()
        {
            CloseWarning();
            this.WarningBox = new WarningBoxViewModel()
            {
                Parent = this,
            };
        }

        public void CloseError()
        {
            if (ErrorBox != null)
                ErrorBox = null;
        }

        public void CloseWarning()
        {
            if (WarningBox != null)
                WarningBox = null;
        }

        public void MinimizeWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        public void CloseWindow()
        {
            Application.Current.Shutdown();
        }

        public DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
