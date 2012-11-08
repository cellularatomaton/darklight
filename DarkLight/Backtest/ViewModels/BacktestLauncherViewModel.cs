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
        DarkLightScreen[] _viewModels = new DarkLightScreen[3];

        int _currentScreenIndex;
        const int _iResponseSelection = 0;
        const int _iParametricRange = 1;
        const int _iTemporalRange = 2;

        public string CurrentScreenName { get; set; }

        #endregion

        #region Constructor

        public BacktestLauncherViewModel(IViewModelService viewModelService)
        {
            _viewModelService = viewModelService;

            _viewModels[_iResponseSelection] = _viewModelService.GetScreenForNavigationEvent(new LinkedNavigationEvent { Destination = NavigationDestination.ResponseSelection });
            _viewModels[_iParametricRange] = _viewModelService.GetScreenForNavigationEvent(new LinkedNavigationEvent { Destination = NavigationDestination.ParametricRange });
            _viewModels[_iTemporalRange] = _viewModelService.GetScreenForNavigationEvent(new LinkedNavigationEvent { Destination = NavigationDestination.TemporalRange });
            
            CurrentScreenName = "Response Selection";
            _currentScreenIndex = 0;
            ActivateItem(_viewModels[_iResponseSelection]);
        }

        #endregion

        #region Public Methods

        public void NavigateBack()
        {
            if (_currentScreenIndex > 0)
            {
                _currentScreenIndex--;
                CurrentScreenName = _viewModels[_currentScreenIndex].ToString();
                ActivateItem(_viewModels[_currentScreenIndex]);
            }           
        }

        public void NavigateQuit()
        {
            //TODO: close out
        }

        public void NavigateNext()
        {
            if (_currentScreenIndex < 2)
            {
                _currentScreenIndex++;
                CurrentScreenName = _viewModels[_currentScreenIndex].ToString();
                ActivateItem(_viewModels[_currentScreenIndex]);
            }  
        }

        public void LaunchBacktest()
        {

        }

        #endregion

        #region Base Class Overrides

        #endregion
   
    }
}
