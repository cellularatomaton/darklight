using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using DarkLight.Client.Customizations;

namespace DarkLight.Client.LiveTrading.ViewModels
{
    public class LiveTradingPortfoliosViewModel : DarkLightScreen
    {
        public LiveTradingPortfoliosViewModel()
        {
            this.DisplayName = this.GetType().Name;
        }
    }
}