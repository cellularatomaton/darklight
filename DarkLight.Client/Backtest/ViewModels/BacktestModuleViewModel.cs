using System;
using System.Windows.Media;
using DarkLight.Client.Customizations;
using Caliburn.Micro;
using DarkLight.Framework.Enums;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.CEP;
using DarkLight.Framework.Interfaces.Common;
using System.Linq;

namespace DarkLight.Client.Backtest.ViewModels
{
    public class BacktestModuleViewModel : DarkLightScreen
    {
        public BacktestModuleViewModel()
        {
            this.DisplayName = this.GetType().Name;
        }

        public void CreateNewBacktest()
        {
            IoC.Get<IEventBroker>().Publish(new LinkedNavigationEvent
            {
                NavigationAction = NavigationAction.NewLinkedWindow,
                Destination = NavigationDestination.BacktestLauncher,
            });
        }

        public void LoadExistingBacktest()
        {
            IoC.Get<IEventBroker>().Publish(new LinkedNavigationEvent
            {
                NavigationAction = NavigationAction.NewWindow,
                Destination = NavigationDestination.BacktestBrowser,
            });
        }
    }
}
