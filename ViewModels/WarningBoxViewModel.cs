using Microsoft.WindowsAPICodePack.Dialogs;
using Stylet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaHackLoader.ViewModels
{
    public class WarningBoxViewModel : Screen
    {
        public string Message
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Dota2 не была обнаружена в системе. Пожалуйста, требуется указать путь до папки \"dota 2 beta\".";
                return "Dota2 is not found in current system. Please, select path to \"dota 2 beta\" folder.";
            }
        }

        public string WarningTitleContent
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Важно";
                return "Important";
            }
        }

        public WarningBoxViewModel()
        {
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
        }

        public async void OkWarning()
        {
            var conductor = this.Parent as MainViewModel;
            var selectPathResult = RequestDotaFolder();
            if (selectPathResult == 1) return;
            if (selectPathResult == 0)
            {
                OkWarning();
                return;
            }
            CancelWarning();
            int instalationCode = -1;
            await Task.Run(() => instalationCode = (conductor.Items[1] as ProfileViewModel).HackInstallation());
            switch (instalationCode)
            {
                case -1:
                    break;
                case 0:
                    if (Properties.Settings.Default.language == "ru-RU")
                        conductor.ShowError("Steam не был обнаружен в системе. Пожалуйста, убедитесь, что Steam установлен");
                    else conductor.ShowError("Steam is not found in current system. Please, make sure you have installed Steam");
                    break;
                case 1:
                    conductor.ShowWarning();
                    break;
            }
        }

        public void CancelWarning()
        {
            (this.Parent as MainViewModel).CloseWarning();
        }

        private int RequestDotaFolder()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select path to dota 2 beta",
                InitialDirectory = "C:\\",
                EnsurePathExists = true
            };
            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                if (!dialog.FileName.Contains("dota 2 beta"))
                {
                    if (Properties.Settings.Default.language == "ru-RU")
                        System.Windows.MessageBox.Show("Выбранный путь не содержит папку dota 2 beta");
                    else System.Windows.MessageBox.Show("Selected path doesn't contain dota 2 beta folder");
                    return 0;
                }
                var count = dialog.FileName.IndexOf("dota 2 beta") + "dota 2 beta".Length;
                if (Directory.Exists($"{dialog.FileName.Substring(0, count)}\\game") && Directory.Exists($"{dialog.FileName.Substring(0, count)}\\game\\dota") && File.Exists($"{dialog.FileName.Substring(0, count)}\\game\\dota\\gameinfo.gi"))
                {
                    Properties.Settings.Default.dotapath = $"{dialog.FileName.Substring(0, count)}\\game";
                    Properties.Settings.Default.Save();                    
                    return 2;
                }
                else
                {
                    if (Properties.Settings.Default.language == "ru-RU")
                        System.Windows.MessageBox.Show("Выбранный путь не является директорией игры Dota2");
                    else System.Windows.MessageBox.Show("Selected path doesn't contain game files");
                    return 0;
                }
            }
            return 1;
        }
    }
}
