using DotaHackLoader.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stylet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DotaHackLoader.ViewModels
{
    public class SettingsViewModel : Screen, ITabItem, IDisposable
    {
        public UserData UserData { get; set; }
        public bool IsEnabled { get; set; }        

        public bool IsMapHackEnabled
        {
            get
            {
                return ReadOptionValue("MapHackOption");
            }
            set
            {
                SaveOptionValue("MapHackOption", value);
            }
        }

        public bool IsMapHackAvailable
        {
            get
            {
                if (UserData.package == "default")
                    return false;
                if (UserData.package == "bloodbath")
                    return true;
                return false;
            }
        }

        public string MapHackDescription
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Показывает героев противника в тумане войны";
                return "Shows enemy heroes in fog of war";
            }
        }

        public bool IsVisibleByEnemyEnabled
        {
            get
            {
                return ReadOptionValue("VisibleByEnemyOption");
            }
            set
            {
                SaveOptionValue("VisibleByEnemyOption", value);
            }
        }

        public bool IsVisibleByEnemyAvailable
        {
            get
            {
                if (UserData.package == "default")
                    return false;
                if (UserData.package == "bloodbath")
                    return true;
                return false;
            }
        }

        public string VisibleByEnemyDescription
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Создает индикацию о том, что вас видно противникам";
                return "Shows indicator when you are visible for enemy";
            }
        }

        public bool IsAutoDisableEnabled
        {
            get
            {
                return ReadOptionValue("AutoDisableOption");
            }
            set
            {
                SaveOptionValue("AutoDisableOption", value);
            }
        }

        public bool IsAutoDisableAvailable
        {
            get
            {
                if (UserData.package == "default")
                    return false;
                if (UserData.package == "bloodbath")
                    return true;
                return false;
            }
        }

        public string AutoDisableDescription
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Автоматически использует обезвреживающие предметы/способности на инициаторов";
                return "Automatically disables initiators by auto using items/abilities";
            }
        }

        public bool IsShowCooldownsEnabled
        {
            get
            {
                return ReadOptionValue("ShowCooldownsOption");
            }
            set
            {
                SaveOptionValue("ShowCooldownsOption", value);
            }
        }

        public bool IsShowCooldownsAvailable
        {
            get
            {
                if (UserData.package != "default")
                    return true;
                return false;
            }
        }

        public string ShowCooldownsDescription
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Показывает кулдауны способностей вражеских героев";
                return "Displays the cooldowns of abilities";
            }
        }

        public bool IsAutoDodgeEnabled
        {
            get
            {
                return ReadOptionValue("AutoDodgeOption");
            }
            set
            {
                SaveOptionValue("AutoDodgeOption", value);
            }
        }

        public bool IsAutoDodgeAvailable
        {
            get
            {
                if (UserData.package == "default")
                    return false;
                if (UserData.package != "classic")
                    return true;
                return false;
            }
        }

        public string AutoDodgeDescription
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Уворачивается от способностей героев противника";
                return "Dodges the abilities of enemy heroes";
            }
        }

        public bool IsShowItemsEnabled
        {
            get
            {
                return ReadOptionValue("ShowItemsOption");
            }
            set
            {
                SaveOptionValue("ShowItemsOption", value);
            }
        }

        public string ShowItemsDescription
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Панель предметов героев противника";
                return "Shows items of enemy heroes";
            }
        }

        public bool IsShowItemsAvailable
        {
            get
            {
                if (UserData.package != "default")
                    return true;
                return false;
            }
        }

        public bool IsAutoStealerEnabled
        {
            get
            {
                return ReadOptionValue("AutoStealingOption");
            }
            set
            {
                SaveOptionValue("AutoStealingOption", value);
            }
        }

        public bool IsAutoStealerAvailable
        {
            get
            {
                if (UserData.package == "default")
                    return false;
                if (UserData.package != "classic")
                    return true;
                return false;
            }
        }

        public string AutoStealerDescription
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Автоматически использует способности/предметы для добивания противника";
                return "Automatically uses abilities/items to killsteal";
            }
        }

        public bool IsAntiCourFeedEnabled
        {
            get
            {
                return ReadOptionValue("AntiCourFeedOption");
            }
            set
            {
                SaveOptionValue("AntiCourFeedOption", value);
            }
        }

        public bool IsAntiCourFeedAvailable
        {
            get
            {
                if (UserData.package == "default")
                    return false;
                if (UserData.package != "classic")
                    return true;
                return false;
            }
        }

        public string AntiCourFeedDescription
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Берет контроль над курьером, блокируя его на базе";
                return "Takes the courier control and blocks it on a base";
            }
        }

        public string AutoComboDescription
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Комбинации на абсолютно всех героев нажатием одной кнопки";
                return "Auto combos on all heroes by one button";
            }
        }

        public bool IsAutoComboAvailable
        {
            get
            {
                if (UserData.package == "default")
                    return false;
                if (UserData.package != "classic")
                    return true;
                return false;
            }
        }

        public string CameraHackDescription
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Позволяет менять дистанцию камеры";
                return "Allows you to change the camera distance";
            }
        }

        public bool IsCameraHackAvailable
        {
            get
            {
                if (UserData.package == "default")
                    return false;
                return true;
            }
        }

        public SettingsViewModel()
        {
            if (Properties.Settings.Default.language == "ru-RU")
                DisplayName = "Настройки";
            else DisplayName = "Settings";           
        }

        private bool ReadOptionValue(string optionKey)
        {
            try
            {
                if (File.Exists("config.json"))
                {
                    var json = "";
                    using (var fs = File.OpenRead("config.json"))
                    using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                        json = sr.ReadToEnd();

                    var cfgjson = JsonConvert.DeserializeObject<Dictionary<string, bool>>(json);
                    return cfgjson[optionKey];
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            //File.WriteAllText("config.json", data);            
        }

        private void SaveOptionValue(string optionKey, bool value)
        {
            if (File.Exists("config.json"))
            {
                var json = "";
                using (var fs = File.OpenRead("config.json"))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = sr.ReadToEnd();
                var obj = JObject.Parse(json);
                obj[optionKey] = value;
                File.WriteAllText("config.json", obj.ToString(Formatting.Indented));
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (!File.Exists("config.json") || UserData.package == "default")
            {

                var defaultData = new Dictionary<string, bool> { { "MapHackOption", false }, { "VisibleByEnemyOption", false }, { "AutoDisableOption", false }, { "ShowCooldownsOption", false }, { "AutoDodgeOption", false }, { "ShowItemsOption", false }, { "AutoStealingOption", false }, { "AntiCourFeedOption", false } };
                string json = JsonConvert.SerializeObject(defaultData, Formatting.Indented);
                File.WriteAllText("config.json", json);
            }
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
        }

        public void RepeatPreview(object sender, System.Windows.RoutedEventArgs e)
        {
            (sender as MediaElement).Position = new TimeSpan(0, 0, 1);
        }

        public void Dispose()
        {

        }
    }
}
