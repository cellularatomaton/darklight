using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Caliburn.Micro;
using DarkLight.Client.Customizations;
using System.Collections;
using DarkLight.Framework.Data.Common;
using DarkLight.Framework.Enums;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.Common;
using DarkLight.Framework.Interfaces.Services;
using DarkLight.Framework.Utilities;

namespace DarkLight.Client.Backtest.ViewModels
{
    public class BacktestLauncherViewModel : DarkLightScreen
    {

        #region Properties

        IViewModelService _viewModelService;
        IBacktestService _backtestService;

        string _currentScreenName;
        public string CurrentScreenName
        {
            get { return _currentScreenName; }
            set
            {
                _currentScreenName = value;
                NotifyOfPropertyChange(() => CurrentScreenName);
            }
        }

        int _currentScreenIndex;
        int CurrentScreenIndex
        {
            get { return _currentScreenIndex; }
            set { 
                _currentScreenIndex = value;
                string tempName = "";
                string separator = " > ";
                for (int i = 0; i < _currentScreenIndex + 1; i++)
                {
                    var formattedName = Items[i].ToString().Replace("DarkLight.Backtest.ViewModels.", "");
                    formattedName = formattedName.Replace("ViewModel", "");
                    tempName += formattedName + (i < _currentScreenIndex ? separator : "");
                }
                CurrentScreenName = tempName;
            }         
        }

        #endregion

        #region Constructor

        public BacktestLauncherViewModel(IViewModelService viewModelService, IBacktestService backtestService)
        {
            _viewModelService = viewModelService;
            _backtestService = backtestService;

            ActivateItem(_viewModelService.GetScreenForNavigationEvent(new LinkedNavigationEvent { Destination = NavigationDestination.ResponseSelection }));
            ActivateItem(_viewModelService.GetScreenForNavigationEvent(new LinkedNavigationEvent { Destination = NavigationDestination.ParametricRange }));
            ActivateItem(_viewModelService.GetScreenForNavigationEvent(new LinkedNavigationEvent { Destination = NavigationDestination.TemporalRange }));

            CurrentScreenIndex = 0;
            ActivateItem(Items[_currentScreenIndex]);
        }

        #endregion

        #region Public Methods

        public void NavigateBack()
        {
            if (_currentScreenIndex > 0)
            {
                CurrentScreenIndex--;
                ActivateItem(Items[CurrentScreenIndex]);
            }           
        }

        public void NavigateQuit()
        {
            TryClose();
        }

        public void NavigateNext()
        {
            if (_currentScreenIndex < Items.Count - 1)
            {
                CurrentScreenIndex++;
                ActivateItem(Items[CurrentScreenIndex]);
            }              
        }

        public void LaunchBacktest()
        {
            var viewModel = IoC.Get<BacktestStatusViewModel>();
            var backtestGroup = MockUtilities.GenerateBacktestGroupRecords(1).First();
            viewModel.Initialize(backtestGroup, 4);

            IoC.Get<IWindowManager>().ShowWindow(viewModel);            

            var requestEvent = new BacktestRequestEvent();
            requestEvent.ActionType = ServiceAction.Run;        
            requestEvent.Response = new DarkLightResponse();           
             requestEvent.Response.Key = backtestGroup.GUID;
            requestEvent.HistDataService = IoC.Get<IHistDataService>();
            IoC.Get<IMediator>().Broadcast(requestEvent);            
        }

        #endregion

        #region Base Class Overrides

        #endregion
   
    }
}
