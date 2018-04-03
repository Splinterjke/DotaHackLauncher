using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaHackLoader.ViewModels
{
    public class ErrorBoxViewModel : Screen
    {
        public string Message { get; private set; }
        public string CustomTitleContent;

        public string ErrorTitleContent
        {
            get
            {
                if (!string.IsNullOrEmpty(CustomTitleContent))
                    return CustomTitleContent;
                if (Properties.Settings.Default.language == "ru-RU")
                    return "Ошибка";
                return "Error";
            }
        }

        public ErrorBoxViewModel(string message)
        {
            this.Message = message;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
        }

        public void CloseWithSuccess()
        {
            (this.Parent as MainViewModel).CloseError();
        }
    }
}
