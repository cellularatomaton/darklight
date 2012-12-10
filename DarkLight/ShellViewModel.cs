﻿using Caliburn.Micro;
using System.ComponentModel.Composition;
using DarkLight.Backtest.ViewModels;
using DarkLight.Common;
using DarkLight.Common.ViewModels;
using DarkLight.Customizations;
using DarkLight.Events;
using DarkLight.Infrastructure;
using DarkLight.Infrastructure.WPFClient;
using DarkLight.Interfaces;
using DarkLight.LiveTrading.ViewModels;
using DarkLight.Optimization.ViewModels;
using DarkLight.Services;
using com.espertech.esper.client;

namespace DarkLight 
{
    //[Export(typeof(IShell))]
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive, IShell, IHandle<LinkedNavigationEvent>
    {
        bool _isBacktestActive;
        public bool IsBacktestActive
        {
            get { return _isBacktestActive; }
            set
            {
                _isBacktestActive = value;
                NotifyOfPropertyChange(() => IsBacktestActive);
            }
        }

        bool _isOptimizationActive;
        public bool IsOptimizationActive
        {
            get { return _isOptimizationActive; }
            set
            {
                _isOptimizationActive = value;
                NotifyOfPropertyChange(() => IsOptimizationActive);
            }
        }

        bool _isLiveActive;
        public bool IsLiveActive
        {
            get { return _isLiveActive; }
            set
            {
                _isLiveActive = value;
                NotifyOfPropertyChange(() => IsLiveActive);
            }
        }

        public ShellViewModel()
        {
            IsBacktestActive = true;

            var mediator = IoC.Get<IMediator>();
            if (mediator.GetType() == typeof(Mediator))            
                IoC.Get<IEventAggregator>().Subscribe(this);
            else if (mediator.GetType() == typeof(MediatorCEP))
                mediator.Subscribe(Events.EventType.LinkedNavigation, UpdateFromCEP);
            
            NavigateToBacktestModule();
        }

        public void NavigateToBacktestModule()
        {
            IsOptimizationActive = false;
            IsLiveActive = false;

            IoC.Get<IMediator>().Broadcast(new LinkedNavigationEvent
            {                
                NavigationAction = NavigationAction.Basic,
                Destination = NavigationDestination.BacktestBrowser
            });
        }

        public void NavigateToOptimizationModule()
        {
            IsBacktestActive = false;
            IsLiveActive = false;

            IoC.Get<IMediator>().Broadcast(new LinkedNavigationEvent
            {
                NavigationAction = NavigationAction.Basic,
                Destination = NavigationDestination.OptimizationScheduler
            });
        }

        public void NavigateToLiveTradingModule()
        {
            IsBacktestActive = false;
            IsOptimizationActive = false;

            IoC.Get<IMediator>().Broadcast(new LinkedNavigationEvent
            {
                NavigationAction = NavigationAction.Basic,
                Destination = NavigationDestination.LiveTradingPorfolios,
            });
        }

        public void ShowEventPublisher()
        {
            IoC.Get<IMediator>().Broadcast(new LinkedNavigationEvent
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
                IoC.Get<IMediator>().Broadcast(linkedNavigationEvent);                
            }
            else if(linkedNavigationEvent.NavigationAction == NavigationAction.NewWindow)
            {
                _viewModel.Initialize(linkedNavigationEvent);
                IoC.Get<IWindowManager>().ShowWindow(_viewModel);
            }
        }

        #endregion

        public void UpdateFromCEP(object sender, UpdateEventArgs e)
        {
            var linkedNavigationEvent = (LinkedNavigationEvent)e.NewEvents[0].Underlying;
            Handle(linkedNavigationEvent);
        }
        
    }
}
