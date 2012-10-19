using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace DarkLight.LiveTrading.ViewModels
{
    public class LiveTradingModuleViewModel : Screen
    {
        public LiveTradingModuleViewModel()
        {
            this.DisplayName = this.GetType().Name;
        }
    }
}
