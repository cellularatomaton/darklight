using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Caliburn.Micro;
using DarkLight.Customizations;
using DarkLight.Events;
using System.Collections;
using DarkLight.Interfaces;
using DarkLight.Services;

namespace DarkLight.Backtest.ViewModels
{
    public class BacktestLauncherViewModel : DarkLightScreen
    {

        #region Properties

        IViewModelService _viewModelService;

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

        public BacktestLauncherViewModel(IViewModelService viewModelService)
        {
            _viewModelService = viewModelService;

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
            //TODO: close out
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
            IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent
            {
                NavigationAction = NavigationAction.NewWindow,
                Destination = NavigationDestination.BacktestStatus
            });
        }

        #endregion

        #region Base Class Overrides

        #endregion
   
    }
}
