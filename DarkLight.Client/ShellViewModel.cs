﻿using Caliburn.Micro;
using System.ComponentModel.Composition;
using DarkLight.Client.Common.ViewModels;
using DarkLight.Client.Customizations;
using DarkLight.Framework.Enums;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.CEP;
using DarkLight.Framework.Interfaces.Common;
using DarkLight.Framework.Interfaces.Services;
using DarkLight.Infrastructure;
using DarkLight.Infrastructure.Mediator;
using DarkLight.Client.LiveTrading.ViewModels;
using DarkLight.Client.Optimization.ViewModels;
using com.espertech.esper.client;
using EventType = DarkLight.Framework.Enums.EventType;


namespace DarkLight.Client
{
    //[Export(typeof(IShell))]
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive, IShell, DarkLight.Framework.Interfaces.CEP.IHandle<LinkedNavigationEvent>
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
            IoC.Get<IEventBroker>().Subscribe(this);            
            NavigateToBacktestModule();

            //TODO: better place for this than here?
            IoC.Get<IBacktestService>().Initialize();                        
        }

        public void NavigateToBacktestModule()
        {
            IsOptimizationActive = false;
            IsLiveActive = false;

            IoC.Get<IEventBroker>().Publish(new LinkedNavigationEvent
            {                
                NavigationAction = NavigationAction.Basic,
                Destination = NavigationDestination.BacktestBrowser
            });
        }

        public void NavigateToOptimizationModule()
        {
            IsBacktestActive = false;
            IsLiveActive = false;

            IoC.Get<IEventBroker>().Publish(new LinkedNavigationEvent
            {
                NavigationAction = NavigationAction.Basic,
                Destination = NavigationDestination.OptimizationScheduler
            });
        }

        public void NavigateToLiveTradingModule()
        {
            IsBacktestActive = false;
            IsOptimizationActive = false;

            IoC.Get<IEventBroker>().Publish(new LinkedNavigationEvent
            {
                NavigationAction = NavigationAction.Basic,
                Destination = NavigationDestination.LiveTradingPorfolios
            });
        }

        public void ShowEventPublisher()
        {
            IoC.Get<IEventBroker>().Publish(new LinkedNavigationEvent
            {
                NavigationAction = NavigationAction.NewWindow,
                Destination = NavigationDestination.EventPublisher
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
                IoC.Get<IEventBroker>().Publish(linkedNavigationEvent);                
            }
            else if(linkedNavigationEvent.NavigationAction == NavigationAction.NewWindow)
            {
                _viewModel.Initialize(linkedNavigationEvent);
                IoC.Get<IWindowManager>().ShowWindow(_viewModel);
            }
        }

        #endregion
        
    }
}
