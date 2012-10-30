using System;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using DarkLight.Backtest.Models;
using DarkLight.Common.ViewModels;
using DarkLight.Customizations;
using DarkLight.Events;
using DarkLight.Repositories;
using DarkLight.Services;

namespace DarkLight.Backtest.ViewModels
{

    public class BacktestBrowserViewModel : LinkableViewModel
    {
        #region Properties

        public BindableCollection<BacktestGroupRecord> BacktestGroups { get; set; }
        public BindableCollection<BacktestRecord> Backtests { get; set; }

        BacktestGroupRecord _selectedBacktestGroup;
        public BacktestGroupRecord SelectedBacktestGroup
        {
            get { return _selectedBacktestGroup; }
            set { _selectedBacktestGroup = value; }
        }

        BacktestRecord _selectedBacktest;
        public BacktestRecord SelectedBacktest
        {
            get { return _selectedBacktest; }
            set
            {
                _selectedBacktest = value;
                IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent
                {
                    NavigationAction = NavigationAction.UpdateLinkedWindows,
                    Group = NavigationGroup.Backtest,
                    ColorGroup = SelectedColorGroup,
                    Key = SelectedBacktest.Description
                });
            }
        }

        #endregion

        #region Constructor

        public BacktestBrowserViewModel(IColorService colorService, IViewModelService viewModelService)
            : base(colorService, viewModelService)
        {
            BacktestGroups = new BindableCollection<BacktestGroupRecord>();
            Backtests = new BindableCollection<BacktestRecord>();
        }

        #endregion

        #region Public Methods

        #region Queries

        public bool CanFindBacktestGroups(string find)
        {
            return !string.IsNullOrWhiteSpace(find);
        }

        public void FindBacktestGroups(string find)
        {
            var backtestGroups = IoC.Get<IBacktestRepository>().GetBacktestGroupRecords(find);
            BacktestGroups.Clear();
            foreach (var backtestGroupRecord in backtestGroups)
            {
                BacktestGroups.Add(backtestGroupRecord);
            }

            SelectedBacktestGroup = null;
        }

        public bool CanQueryBacktestGroup(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || SelectedBacktestGroup == null)
                return false;
            else
                return true;
        }

        public void QueryBacktestGroup(string query)
        {
            if (SelectedBacktestGroup != null)
            {
                var backtests = IoC.Get<IBacktestRepository>().GetBacktestRecords(SelectedBacktestGroup.Description, query);
                Backtests.Clear();
                foreach (var backtest in backtests)
                {
                    Backtests.Add(backtest);
                }
            }
        }

        #endregion

        #region Single Test Views

        public void OpenSingleTestWindow(NavigationDestination destination)
        {
            if (SelectedBacktest != null)
            {

                IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent
                {
                    NavigationAction = NavigationAction.NewLinkedWindow,
                    Destination = destination,
                    Group = NavigationGroup.Backtest,
                    ColorGroup = SelectedColorGroup,
                    Key = SelectedBacktest.Description
                });
            }
        }

        #endregion

        #region Multi Test Views

        public void OpenTemporalStatisticsWindow()
        {
            //TODO
        }

        public void OpenParametricStatisticsWindow()
        {
            //TODO
        }

        #endregion

        #region Context Menu

        public void ShowBacktestStatus()
        {
            //TODO
        }

        public void ShowTemporalStatistics()
        {
            //TODO
        }

        public void ShowParametricStatistics()
        {
            //TODO
        }

        public void Show1DPlot()
        {
            //TODO
        }

        public void Show2DPlot()
        {
            //TODO
        }

        #endregion

        #endregion

        #region Base Class Overrides

        public override void Handle(LinkedNavigationEvent linkedNavigationEvent)
        {
            //ignore events for now
        }

        #endregion
    }



}
