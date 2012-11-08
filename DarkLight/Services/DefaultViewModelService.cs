using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using DarkLight.Backtest.ViewModels;
using DarkLight.Common.ViewModels;
using DarkLight.Customizations;
using DarkLight.Events;
using DarkLight.LiveTrading.ViewModels;
using DarkLight.Optimization.ViewModels;

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
                case NavigationDestination.EventPublisher:
                {
                    _viewModel = IoC.Get<EventPublisherViewModel>();
                    break;
                }
                case NavigationDestination.BacktestModule:
                {
                    _viewModel = IoC.Get<BacktestModuleViewModel>();
                    break;
                }
                case NavigationDestination.BacktestLauncher:
                {
                    _viewModel = IoC.Get<BacktestLauncherViewModel>();
                    break;
                }
                case NavigationDestination.BacktestBrowser:
                {
                    _viewModel = IoC.Get<BacktestBrowserViewModel>();
                    break;
                }
                case NavigationDestination.OptimizationModule:
                {
                    _viewModel = IoC.Get<OptimizationModuleViewModel>();
                    break;
                }
                case NavigationDestination.LiveTradingModule:
                {
                    _viewModel = IoC.Get<LiveTradingModuleViewModel>();
                    break;
                }
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
                case NavigationDestination.ResponseSelection:
                {
                    _viewModel = IoC.Get<ResponseSelectionViewModel>();
                    break;
                }
                case NavigationDestination.ResponseConfiguration:
                {
                    _viewModel = IoC.Get<ResponseConfigurationViewModel>();
                    break;
                }
                case NavigationDestination.ParametricRange:
                {
                    _viewModel = IoC.Get<ParametricRangeViewModel>();
                    break;
                }
                case NavigationDestination.TemporalRange:
                {
                    _viewModel = IoC.Get<TemporalRangeViewModel>();
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
