using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using DarkLight.Common.ViewModels;
using DarkLight.Customizations;

namespace DarkLight.LiveTrading.ViewModels
{
    public class LiveTradingModuleViewModel : DarkLightScreen
    {
        public LiveTradingModuleViewModel()
        {
            this.DisplayName = this.GetType().Name;
        }
    }
}
