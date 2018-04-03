using DotaHackLoader.Models;
using Stylet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaHackLoader.ViewModels
{
    public class StoreViewModel : Screen, ITabItem
    {
        public PriceData[] Prices { get; set; }
        public BuyLinks BuyLinks { get; set; }

        public string AntivacSource
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "http://ru.dotameat.com/assets/images/AntiVAC_ru.png";
                return "http://dotameat.com/assets/images/AntiVAC_en.png";
            }
        }

        public void ClassicLink()
        {
            if (Properties.Settings.Default.language == "ru-RU")
                Process.Start(BuyLinks.ru_classic);
            else Process.Start(BuyLinks.en_classic);
        }

        public void SuperbLink()
        {
            if (Properties.Settings.Default.language == "ru-RU")
                Process.Start(BuyLinks.ru_superb);
            else Process.Start(BuyLinks.en_superb);
        }

        public void BloodbathLink()
        {
            if (Properties.Settings.Default.language == "ru-RU")
                Process.Start(BuyLinks.ru_bloodbath);
            else Process.Start(BuyLinks.en_bloodbath);
        }

        public string ClassicPrice
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return $"{Prices[0].price_rur} р.";
                return $"${Prices[0].price_usd}";
            }
        }

        public string BloodBathPrice
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return $"{Prices[2].price_rur} р.";
                return $"${Prices[2].price_usd}";
            }
        }

        public string SuperbPrice
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return $"{Prices[1].price_rur} р.";
                return $"${Prices[1].price_usd}";
            }
        }

        public string BuyButtonText
        {
            get
            {
                if (Properties.Settings.Default.language == "ru-RU")
                    return "КУПИТЬ";
                return "BUY";
            }
        }

        public bool IsEnabled { get; set; }

        public StoreViewModel()
        {
            if (Properties.Settings.Default.language == "ru-RU")
                DisplayName = "Магазин";
            else DisplayName = "Store";
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
        }
    }
}
