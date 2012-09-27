using Caliburn.Micro;
using System.ComponentModel.Composition;
using DarkLight.Backtest.ViewModels;
using DarkLight.Common;
using DarkLight.Common.ViewModels;
using DarkLight.Events;
using DarkLight.LiveTrading.ViewModels;
using DarkLight.Optimization.ViewModels;

namespace DarkLight 
{
    //[Export(typeof(IShell))]
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive, IShell, IHandle<NavigationEvent>
    {
        public ShellViewModel()
        {
            IoC.Get<IEventAggregator>().Subscribe(this);
            NavigateToBacktestModule();
        }

        public void NavigateToBacktestModule()
        {
            IoC.Get<IEventAggregator>().Publish(new NavigationEvent{NavigateTo = Destination.Backtest });
        }

        public void NavigateToOptimizationModule()
        {
            IoC.Get<IEventAggregator>().Publish(new NavigationEvent { NavigateTo = Destination.Optimization });
        }

        public void NavigateToLiveTradingModule()
        {
            IoC.Get<IEventAggregator>().Publish(new NavigationEvent { NavigateTo = Destination.LiveTrading });
        }

        #region Implementation of IHandle<NavigationEvent>

        public void Handle(NavigationEvent message)
        {
            switch (message.NavigateTo)
            {
                case Destination.Backtest:
                    ActivateItem(new BacktestModuleViewModel());
                    break;
                case Destination.Optimization:
                    ActivateItem(new OptimizationModuleViewModel());
                    break;
                case Destination.LiveTrading:
                    ActivateItem(new LiveTradingModuleViewModel());
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}
