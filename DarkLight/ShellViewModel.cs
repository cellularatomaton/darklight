using Caliburn.Micro;
using System.ComponentModel.Composition;
using DarkLight.Backtest.ViewModels;
using DarkLight.Common;
using DarkLight.Common.ViewModels;
using DarkLight.Events;
using DarkLight.Interfaces;
using DarkLight.LiveTrading.ViewModels;
using DarkLight.Optimization.ViewModels;
using DarkLight.Services;

namespace DarkLight 
{
    //[Export(typeof(IShell))]
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive, IShell, IHandle<LinkedNavigationEvent>
    {
        public ShellViewModel()
        {
            IoC.Get<IEventAggregator>().Subscribe(this);
            NavigateToBacktestModule();
        }

        public void NavigateToBacktestModule()
        {
            IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent { Destination = NavigationDestination.Backtest });
        }

        public void NavigateToOptimizationModule()
        {
            IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent { Destination = NavigationDestination.Optimization });
        }

        public void NavigateToLiveTradingModule()
        {
            IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent { Destination = NavigationDestination.LiveTrading });
        }

        #region Implementation of IHandle<NavigationEvent>

        public void Handle(LinkedNavigationEvent linkedNavigationEvent)
        {
            var _filter = IoC.Get<IFilterService>().GetLinkedNavigationFilter();
            if (_filter.IsPassedBy(linkedNavigationEvent))
            {
                Screen _viewModel;
                switch (linkedNavigationEvent.Destination)
                {
                    case NavigationDestination.Backtest:
                    {
                        _viewModel = IoC.Get<BacktestModuleViewModel>();
                        ActivateItem(_viewModel);
                        break;
                    }
                    case NavigationDestination.Optimization:
                    {
                        _viewModel = IoC.Get<OptimizationModuleViewModel>();
                        ActivateItem(_viewModel);
                        break;
                    }
                    case NavigationDestination.LiveTrading:
                    {
                        _viewModel = IoC.Get<LiveTradingModuleViewModel>();
                        ActivateItem(_viewModel);
                        break;
                    }
                    default:
                    {
                        linkedNavigationEvent.LinkGroup = IoC.Get<IFilterService>().GetDefaultLinkGroup(linkedNavigationEvent.Destination);
                        var _linkableViewModel = IoC.Get<LinkableViewModel>();
                        _linkableViewModel.SelectedColorGroup = linkedNavigationEvent.ColorGroup;
                        _linkableViewModel.SelectedLinkGroup = linkedNavigationEvent.LinkGroup;
                        _linkableViewModel.Handle(linkedNavigationEvent);
                        IoC.Get<IWindowManager>().ShowWindow(_linkableViewModel);
                        break;
                    }
                }
            }
        }

        #endregion
    }
}
