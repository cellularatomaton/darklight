using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using DarkLight.Common.ViewModels;
using DarkLight.Events;

namespace DarkLight.Services
{
    public class DefaultViewModelService : IViewModelService
    {
        #region Implementation of IViewModelService

        public DarkLightScreen GetScreenForNavigationEvent(LinkedNavigationEvent linkedNavigationEvent)
        {
            DarkLightScreen _viewModel;
            switch (linkedNavigationEvent.Destination)
            {
                case NavigationDestination.Statistics:
                    {
                        _viewModel = IoC.Get<StatisticsViewModel>();
                        break;
                    }
                case NavigationDestination.Results:
                    {
                        _viewModel = IoC.Get<ResultsViewModel>();
                        break;
                    }
                case NavigationDestination.Fills:
                    {
                        _viewModel = IoC.Get<FillsViewModel>();
                        break;
                    }
                case NavigationDestination.Indicators:
                    {
                        _viewModel = IoC.Get<IndicatorsViewModel>();
                        break;
                    }
                case NavigationDestination.Messages:
                    {
                        _viewModel = IoC.Get<MessagesViewModel>();
                        break;
                    }
                case NavigationDestination.TickData:
                    {
                        _viewModel = IoC.Get<TickDataViewModel>();
                        break;
                    }
                case NavigationDestination.Orders:
                    {
                        _viewModel = IoC.Get<OrdersViewModel>();
                        break;
                    }
                case NavigationDestination.Positions:
                    {
                        _viewModel = IoC.Get<PositionsViewModel>();
                        break;
                    }
                case NavigationDestination.TimeSeries:
                    {
                        _viewModel = IoC.Get<TimeseriesViewModel>();
                        break;
                    }
                default:
                    {
                        _viewModel = IoC.Get<DefaultViewModel>();
                        break;
                    }
            }
            return _viewModel;
        }

        #endregion
    }
}
