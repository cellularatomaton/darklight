using System;
using System.Windows.Media;
using DarkLight.Events;
using Caliburn.Micro;
using DarkLight.Services;
using System.Linq;

namespace DarkLight.Backtest.ViewModels
{
    public class BacktestModuleViewModel : Screen
    {
        public BacktestModuleViewModel()
        {
            
            this.DisplayName = this.GetType().Name;
        }
    }
}
