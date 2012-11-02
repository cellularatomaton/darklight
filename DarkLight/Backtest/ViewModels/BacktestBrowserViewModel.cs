using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
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

        string _backtestGroupSortColumn = "CreateDate";
        string _backtestSortColumn = "CreateDate";

        ListSortDirection backtestGroupDirection = ListSortDirection.Descending;
        ListSortDirection backtestDirection = ListSortDirection.Descending;

        public BindableCollection<BacktestGroupRecord> BacktestGroups { get; set; }
        public BindableCollection<BacktestRecord> Backtests { get; set; }
        public ICollectionView BacktestGroupView { get; set; }
        public ICollectionView BacktestView { get; set; }

        BacktestGroupRecord _selectedBacktestGroupView;
        public BacktestGroupRecord SelectedBacktestGroupView
        {
            get { return _selectedBacktestGroupView; }
            set { _selectedBacktestGroupView = value; }
        }

        BacktestRecord _selectedBacktestView;
        public BacktestRecord SelectedBacktestView
        {
            get { return _selectedBacktestView; }
            set
            {
                _selectedBacktestView = value;
                IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent
                {
                    NavigationAction = NavigationAction.UpdateLinkedWindows,
                    Group = NavigationGroup.Backtest,
                    ColorGroup = SelectedColorGroup,
                    Key = SelectedBacktestView.Description
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
            BacktestGroupView = CollectionViewSource.GetDefaultView(BacktestGroups);
            BacktestView = CollectionViewSource.GetDefaultView(Backtests);
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

            SelectedBacktestGroupView = null;
        }

        public bool CanQueryBacktestGroup(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || SelectedBacktestGroupView == null)
                return false;
            else
                return true;
        }

        public void QueryBacktestGroup(string query)
        {
            if (SelectedBacktestGroupView != null)
            {
                var backtests = IoC.Get<IBacktestRepository>().GetBacktestRecords(SelectedBacktestGroupView.Description, query);
                Backtests.Clear();
                foreach (var backtest in backtests)
                {
                    Backtests.Add(backtest);
                }
            }
        }

        public void SortBacktestGroup(string column)
        {
            if (_backtestGroupSortColumn == column)
                backtestGroupDirection = backtestGroupDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            else //default
                backtestGroupDirection = ListSortDirection.Ascending;

            _backtestGroupSortColumn = column;
            BacktestGroupView.SortDescriptions.Clear();
            BacktestGroupView.SortDescriptions.Add(new SortDescription(_backtestGroupSortColumn, backtestGroupDirection));
        }

        public void SortBacktest(string column)
        {
            if (_backtestSortColumn == column)
                backtestDirection = backtestDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            else 
                backtestDirection = ListSortDirection.Ascending;

            _backtestSortColumn = column;
            BacktestView.SortDescriptions.Clear();
            BacktestView.SortDescriptions.Add(new SortDescription(_backtestSortColumn, backtestDirection));
        }

        #endregion

        #region Single Test Views

        public void OpenSingleTestWindow(NavigationDestination destination)
        {
            if (SelectedBacktestView != null)
            {

                IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent
                {
                    NavigationAction = NavigationAction.NewLinkedWindow,
                    Destination = destination,
                    Group = NavigationGroup.Backtest,
                    ColorGroup = SelectedColorGroup,
                    Key = SelectedBacktestView.Description
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
