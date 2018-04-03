using DotaHackLoader.Models;
using DotaHackLoader.Services;
using Microsoft.Win32;
using Newtonsoft.Json;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DotaHackLoader.ViewModels
{
    public class LoginViewModel : Screen, ITabItem
    {
        #region Properties
        [DisplayName("Login")]
        public string Login { get; set; }

        [DisplayName("Password")]
        public SecureString Password { get; set; }

        public string Title
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Играй По Своим Правилам";
                return "Play Your Own Rules";
            }
        }        

        public string LoginButtonText
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "АВТОРИЗОВАТЬСЯ";
                return "SIGN IN";
            }
        }

        public string RememberText
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Запомнить";
                return "Remember me";
            }
        }

        public bool IsEnabled { get; set; } = true;
        public bool IsNotProcessing { get; set; } = true;

        public bool IsAuthSaved { get; set; }
        #endregion

        #region Variables        
        private MainViewModel Conductor;
        #endregion

        #region Base Methods
        public LoginViewModel()
        {
            this.DisplayName = "";
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Conductor = (this.Parent as MainViewModel);
        }

        public void KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && CheckAuthForm())
                LoginCommand();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
        }
        #endregion

        protected override void OnValidationStateChanged(IEnumerable<string> changedProperties)
        {
            base.OnValidationStateChanged(changedProperties);
            // Manual rasing notifier for properties
            this.NotifyOfPropertyChange(() => this.CanLoginCommand);
        }

        public bool CanLoginCommand
        {
            get { return CheckAuthForm() && IsNotProcessing; }
        }

        private bool CheckAuthForm()
        {
            if (string.IsNullOrEmpty(Login))
                return false;
            if (Password == null || Password.Length == 0)
                return false;
            return true;
        }

        public async void LoginCommand()
        {
            try
            {
                IsNotProcessing = false;
                var response = await HttpService.GetAuthAsync(Login, Password);
                IsNotProcessing = true;
                if (response.Item1)
                {
                    Conductor.ShowError(response.Item2);
                    return;
                }
                var authResult = JsonConvert.DeserializeObject<AuthResponse>(response.Item2);
                switch (authResult.error)
                {
                    case 4:
                        Conductor.ShowError("Пользователя не существует");
                        return;
                    case 5:
                        Conductor.ShowError("Неверный пароль");
                        return;
                    case 7:
                        Conductor.ShowError("Сессия истекла, авторизуйтесь снова");
                        return;
                    case 0:
                        //Conductor.ShowError("Токен получен");
                        break;
                    default:
                        Conductor.ShowError("Нет соединения с сервером");
                        return;
                }

                if (IsAuthSaved)
                {
                    Properties.Settings.Default.userToken = authResult.token;
                    Properties.Settings.Default.Save();
                }
                else if (!string.IsNullOrEmpty(Properties.Settings.Default.userToken))
                {
                    Properties.Settings.Default.userToken = "";
                    Properties.Settings.Default.Save();
                }
                HttpService.Token = authResult.token;
                Conductor.Initiation();
            }
            catch (Exception ex)
            {
                Conductor.ShowError(ex.Message);
            }
        }
    }
}
