using System;
using System.Windows.Media;
using DarkLight.Common.ViewModels;
using DarkLight.Customizations;
using DarkLight.Events;
using Caliburn.Micro;
using DarkLight.Infrastructure;
using DarkLight.Services;
using System.Linq;

namespace DarkLight.Backtest.ViewModels
{
    public class BacktestModuleViewModel : DarkLightScreen
    {
        public BacktestModuleViewModel()
        {
            this.DisplayName = this.GetType().Name;
        }

        public void CreateNewBacktest()
        {
            IoC.Get<IMediator>().Broadcast(new LinkedNavigationEvent
            {
                NavigationAction = NavigationAction.NewLinkedWindow,
                Destination = NavigationDestination.BacktestLauncher,
            });
        }

        public void LoadExistingBacktest()
        {
            IoC.Get<IMediator>().Broadcast(new LinkedNavigationEvent
            {
                NavigationAction = NavigationAction.NewWindow,
                Destination = NavigationDestination.BacktestBrowser,
            });
        }
    }
}
