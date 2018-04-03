using DotaHackLoader.Models;
using DotaHackLoader.Services;
using Microsoft.Win32;
using Newtonsoft.Json;
using Stylet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DotaHackLoader.ViewModels
{
    public class ProfileViewModel : Screen, ITabItem, IDisposable
    {
        public UserData UserData { get; set; }

        public IList<string> Languages { get; set; }
        public int SelectedLanguage { get; set; }

        public string UserNameTitle
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Имя пользователя:";
                return "Username:";
            }
        }

        public string GenderTitle
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Пол:";
                return "Gender:";
            }
        }

        public string MailTitle
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Эл.адрес:";
                return "E-mail:";
            }
        }

        public string RegisterDateTitle
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Дата регистрации:";
                return "Resitration date:";
            }
        }

        public string SubscriptionDateTitle
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Окончании подписки:";
                return "Subscription expires at:";
            }
        }

        public string LanguageTitle
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Язык:";
                return "Language:";
            }
        }

        public string LogoutContent
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "ВЫХОД";
                return "LOGOUT";
            }
        }

        public string ProcessingText
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Установка DotaMeat.\nЭто может занять несколько минут...";
                return "DotaMeat injecting.\nIt may take few minutes...";
            }
        }

        public bool IsEnabled { get; set; }
        public bool IsLoading { get; set; }
        public string StartGameContent
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "ЗАПУСТИТЬ ИГРУ";
                return "START GAME";
            }
        }

        public ProfileViewModel()
        {
            if (Properties.Settings.Default.language == "ru-RU")
                DisplayName = "Профиль";
            else DisplayName = "Profile";
            Languages = new List<string> { "English", "Русский" };
            SelectedLanguage = Properties.Settings.Default.language == "ru-RU" ? 1 : 0;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
        }

        public void OnLogoutClick()
        {
            (this.Parent as MainViewModel).ShowLogin();
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            var selectedItem = (sender as ComboBox).SelectedValue.ToString();
            if (selectedItem == "English" && Properties.Settings.Default.language != "en-EN")
            {
                Properties.Settings.Default.language = "en-EN";
                Properties.Settings.Default.Save();
                (this.Parent as MainViewModel).ShowError("The language has been changed. Changes only take effect after a program restart", "Important");
                return;
            }
            if (selectedItem == "Русский" && Properties.Settings.Default.language != "ru-RU")
            {
                Properties.Settings.Default.language = "ru-RU";
                Properties.Settings.Default.Save();
                (this.Parent as MainViewModel).ShowError("Язык изменен. Изменения вступят в силу после перезапуска программы", "Важно");
            }
        }

        public async void StartGameCommand()
        {
            IsLoading = true;
            var response = await HttpService.GetUserDataAsync();
            if (response.Item1)
            {
                IsLoading = false;
                (this.Parent as MainViewModel).ShowError(response.Item2);
                return;
            }

            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (process.ProcessName == "dota2")
                {
                    if (Properties.Settings.Default.language == "ru-RU")
                        (this.Parent as MainViewModel).ShowError("Dota2 уже запущена. Остановите процесс игры и запустите игру из DotaMeat лаунчера.");
                    else (this.Parent as MainViewModel).ShowError("Dota2 is already running. Exit game and start it from the DotaMeat launcher");
                    IsLoading = false;
                    return;
                }
            }

            var userData = JsonConvert.DeserializeObject<UserData>(response.Item2);
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
            this.UserData = userData;
            if (daysLeftValue == 0 || daysLeftTime < DateTime.Now || userData.package == "default")
            {
                IsLoading = false;
                string message = "";
                if (Properties.Settings.Default.language == "ru-RU")
                    message = "Подписка не приобретена или ее время истекло.";
                else message = "Subscription is not purchased or it has been expired.";
                (this.Parent as MainViewModel).ShowError(message);
                return;
            }
            int instalationCode = -1;
            await Task.Run(() => instalationCode = HackInstallation());
            IsLoading = false;
            switch (instalationCode)
            {
                case -1:
                    break;
                case 0:
                    if (Properties.Settings.Default.language == "ru-RU")
                        (this.Parent as MainViewModel).ShowError("Steam не был обнаружен в системе. Пожалуйста, убедитесь, что Steam установлен");
                    else (this.Parent as MainViewModel).ShowError("Steam is not found in current system. Please, make sure you have installed Steam");
                    return;
                case 1:
                    (this.Parent as MainViewModel).ShowWarning();
                    return;
            }
            Process.Start("steam://run/570");
            CancellationTokenSource cts = new CancellationTokenSource();
            var token = cts.Token;

            Task t = Task.Run(() => { CheckForDotaLaunched(token); }, token);
            try
            {
                t.Wait();
            }
            catch (AggregateException)
            {
                cts.Cancel();
            }
            catch (Exception)
            {
                cts.Cancel();
            }
        }

        private async void CheckForDotaLaunched(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process process in processes)
                {
                    if (process.ProcessName == "dota2")
                    {
                        if (!string.IsNullOrEmpty(Services.HttpService.dotaPath) && File.Exists($"{Services.HttpService.dotaPath}\\gameinfo.gi"))
                        {
                            var gameinfoDefaultString = File.ReadAllText($"{Services.HttpService.dotaPath}\\gameinfo.gi");
                            if (gameinfoDefaultString.Contains("\t\t\tGame\t\t\t\tdotameat\r\n"))
                            {

                                var position = gameinfoDefaultString.IndexOf("\t\t\tGame\t\t\t\tdotameat\r\n");
                                var defaultGameInfoString = gameinfoDefaultString.Remove(position, 21);
                                File.WriteAllText($"{Services.HttpService.dotaPath}\\gameinfo.gi", defaultGameInfoString);
                            }
                        }
                        token.ThrowIfCancellationRequested();
                        return;
                    }
                }
                await Task.Delay(2000);
            }
        }

        public DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public void Dispose()
        {

        }

        public string TraverseTree(string root, string targetName)
        {
            Stack<string> dirs = new Stack<string>();

            if (!System.IO.Directory.Exists(root))
            {
                return null;
            }
            dirs.Push(root);

            while (dirs.Count > 0)
            {
                string currentDir = dirs.Pop();
                string[] subDirs;
                try
                {
                    subDirs = System.IO.Directory.GetDirectories(currentDir);

                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                catch (System.IO.DirectoryNotFoundException)
                {
                    continue;
                }

                foreach (string str in subDirs)
                {
                    if (str.Contains(targetName))
                        return str;
                    dirs.Push(str);
                }

            }
            return null;
        }

        private string dotaPath;
        public int HackInstallation()
        {
            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            dotaPath = !string.IsNullOrEmpty(Properties.Settings.Default.dotapath) ? Properties.Settings.Default.dotapath : null;
            if (dotaPath == null || !Directory.Exists(dotaPath))
                using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
                {
                    var appNames = key.GetSubKeyNames();
                    if (appNames.Contains("Steam"))
                    {
                        var steamKey = key.OpenSubKey("Steam");
                        var steamPath = steamKey.GetValue("UninstallString").ToString();
                        var dirs = Directory.GetDirectories($"{Directory.GetParent(steamPath).FullName}\\steamapps");

                        if (dirs.Contains($"{Directory.GetParent(steamPath).FullName}\\steamapps\\common"))
                            dotaPath = $"{Directory.GetParent(steamPath).FullName}\\steamapps\\common\\dota 2 beta\\game";
                        else
                        {
                            foreach (var drive in DriveInfo.GetDrives())
                            {
                                var dotaDir = TraverseTree(drive.Name, "dota 2 beta");
                                if (dotaDir != null && !dotaDir.Contains("Sandbox"))
                                {
                                    dotaPath = $"{dotaDir}\\game";
                                    break;
                                }
                            }
                        }
                        Properties.Settings.Default.dotapath = dotaPath;
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        return 0;
                    }
                }
            if (dotaPath == null)
            {
                Properties.Settings.Default.dotapath = string.Empty;
                Properties.Settings.Default.Save();
                return 1;
            }

            var gameinfoGIPath = $"{dotaPath}\\dota";
            if (!Directory.Exists(gameinfoGIPath))
            {
                Properties.Settings.Default.dotapath = string.Empty;
                Properties.Settings.Default.Save();
                return 1;
            }
            HttpService.dotaPath = gameinfoGIPath;
            if (!Directory.Exists($"{dotaPath}\\dotameat"))
                Directory.CreateDirectory($"{dotaPath}\\dotameat");
            dotaPath = $"{dotaPath}\\dotameat";

            using (WebClient wc = new WebClient())
            {
                if (UserData.package == "classic")
                    wc.DownloadFileAsync(new System.Uri("http://dotameat.com/classic/pak01_dir.vpk"), $"{dotaPath}\\pak01_dir.vpk");
                if (UserData.package == "bloodbath" || UserData.package == "superb")
                    wc.DownloadFileAsync(new System.Uri("http://dotameat.com/superb/pak01_dir.vpk"), $"{dotaPath}\\pak01_dir.vpk");
            };
            var gameinfoDefaultString = File.ReadAllText($"{gameinfoGIPath}\\gameinfo.gi");
            if (string.IsNullOrEmpty(gameinfoDefaultString))
                return 1;
            if (gameinfoDefaultString.Contains("\t\t\tGame\t\t\t\tdotameat\r\n"))
                return -1;
            if (!gameinfoDefaultString.Contains("\n\t\t\tGame\t\t\t\tdota\r\n\t\t\tGame\t\t\t\tcore"))
                return 1;
            var position = gameinfoDefaultString.IndexOf("\n\t\t\tGame\t\t\t\tdota\r\n\t\t\tGame\t\t\t\tcore");
            var gameinfoModifiedString = gameinfoDefaultString.Insert(position, "\t\t\tGame\t\t\t\tdotameat\r\n");
            File.WriteAllText($"{gameinfoGIPath}\\gameinfo.gi", gameinfoModifiedString);
            return -1;
        }
    }
}
