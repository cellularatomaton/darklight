using Caliburn.Micro;
using System.ComponentModel.Composition;
using DarkLight.Backtest.ViewModels;
using DarkLight.Common;
using DarkLight.Common.ViewModels;
using DarkLight.Customizations;
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
            IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent 
            {
                NavigationAction = NavigationAction.Basic,
                Destination = NavigationDestination.BacktestModule 
            });
        }

        public void NavigateToOptimizationModule()
        {
            IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent
            {
                NavigationAction = NavigationAction.Basic,
                Destination = NavigationDestination.OptimizationModule
            });
        }

        public void NavigateToLiveTradingModule()
        {
            IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent
            {
                NavigationAction = NavigationAction.Basic,
                Destination = NavigationDestination.LiveTradingModule,
            });
        }

        public void ShowEventPublisher()
        {
            IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent
            {
                NavigationAction = NavigationAction.NewWindow,
                Destination = NavigationDestination.EventPublisher,
            });
        }

        #region Implementation of IHandle<NavigationEvent>

        public void Handle(LinkedNavigationEvent linkedNavigationEvent)
        {
            DarkLightScreen _viewModel = IoC.Get<IViewModelService>().GetScreenForNavigationEvent(linkedNavigationEvent);
            if (linkedNavigationEvent.NavigationAction == NavigationAction.Basic)
            {
                ActivateItem(_viewModel);
            }
            else if(linkedNavigationEvent.NavigationAction == NavigationAction.NewLinkedWindow)
            {
                var _linkableViewModel = IoC.Get<LinkableViewModel>();
                _linkableViewModel.Initialize(linkedNavigationEvent);
                IoC.Get<IWindowManager>().ShowWindow(_linkableViewModel);
                linkedNavigationEvent.NavigationAction = NavigationAction.UpdateLinkedWindows;
                IoC.Get<IEventAggregator>().Publish(linkedNavigationEvent);                
            }
            else if(linkedNavigationEvent.NavigationAction == NavigationAction.NewWindow)
            {
                IoC.Get<IWindowManager>().ShowWindow(_viewModel);
            }
        }

        #endregion

        
    }
}
