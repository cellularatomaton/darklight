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
            //var _filter = IoC.Get<IFilterService>().GetLinkedNavigationFilter();
            if (linkedNavigationEvent.NavigationAction == NavigationAction.Basic)
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
                        break;
                    }
                }
            }
            else if(linkedNavigationEvent.NavigationAction == NavigationAction.NewLinkedWindow)
            {
                var _linkableViewModel = IoC.Get<LinkableViewModel>();
                _linkableViewModel.Destination = linkedNavigationEvent.Destination;
                _linkableViewModel.SelectedColorGroup = linkedNavigationEvent.ColorGroup;
                IoC.Get<IWindowManager>().ShowWindow(_linkableViewModel);
                IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent
                {
                    NavigationAction = NavigationAction.UpdateLinkedWindows,
                    Destination = linkedNavigationEvent.Destination,
                    ColorGroup = linkedNavigationEvent.ColorGroup,
                    Key = linkedNavigationEvent.Key,
                });
                
            }
            else if(linkedNavigationEvent.NavigationAction == NavigationAction.NewWindow)
            {
                var _viewModel = IoC.Get<EventPublisherViewModel>();
                IoC.Get<IWindowManager>().ShowWindow(_viewModel);
            }
        }

        #endregion

        public void ShowEventPublisher()
        {
            IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent
            {
                NavigationAction = NavigationAction.NewWindow,
                Destination = NavigationDestination.EventPublisher,
            });
        }
    }
}
