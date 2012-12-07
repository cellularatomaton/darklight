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
using DarkLight.Infrastructure;
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
        public BindableCollection<ResponseSessionRecord> Backtests { get; set; }
        public ICollectionView BacktestGroupView { get; set; }
        public ICollectionView BacktestView { get; set; }

        string _findBacktestGroupsText;
        public string FindBacktestGroupsText
        {
            get { return _findBacktestGroupsText; }
            set
            {
                _findBacktestGroupsText = value;
                NotifyOfPropertyChange(() => FindBacktestGroupsText);
            }
        }

        string _queryBacktestGroupText;
        public string QueryBacktestGroupText
        {
            get { return _queryBacktestGroupText; }
            set
            {
                _queryBacktestGroupText = value;
                NotifyOfPropertyChange(() => QueryBacktestGroupText);
            }
        }

        BacktestGroupRecord _selectedBacktestGroupView;
        public BacktestGroupRecord SelectedBacktestGroupView
        {
            get { return _selectedBacktestGroupView; }
            set { _selectedBacktestGroupView = value; }
        }

        ResponseSessionRecord _selectedBacktestView;
        public ResponseSessionRecord SelectedBacktestView
        {
            get { return _selectedBacktestView; }
            set
            {
                _selectedBacktestView = value;
                IoC.Get<IMediator>().Broadcast(new LinkedNavigationEvent
                {
                    NavigationAction = NavigationAction.UpdateLinkedWindows,
                    Group = NavigationGroup.Backtest,
                    ColorGroup = SelectedColorGroup,
                    Key = SelectedBacktestView.GUID
                });
            }
        }

        #endregion

        #region Constructor

        public BacktestBrowserViewModel(IColorService colorService, IViewModelService viewModelService)
            : base(colorService, viewModelService)
        {
            BacktestGroups = new BindableCollection<BacktestGroupRecord>();
            Backtests = new BindableCollection<ResponseSessionRecord>();
            BacktestGroupView = CollectionViewSource.GetDefaultView(BacktestGroups);
            BacktestView = CollectionViewSource.GetDefaultView(Backtests);
        }

        #endregion

        #region Public Methods

        #region Header Controls

        public void NewBacktest()
        {
            IoC.Get<IMediator>().Broadcast(new LinkedNavigationEvent
            {
                NavigationAction = NavigationAction.NewWindow,
                Destination = NavigationDestination.BacktestLauncher,
            });
        }

        #endregion

        #region Queries
        /*
        public bool CanFindBacktestGroups(string find)
        {
            return !string.IsNullOrWhiteSpace(find);
        }
        */
        public void FindBacktestGroups()
        {
            string find = FindBacktestGroupsText;
            if (string.IsNullOrWhiteSpace(find))
            {
                OpenErrorWindow("Please enter response type");
            }
            else
            {
                var backtestGroups = IoC.Get<IBacktestRepository>().GetBacktestGroupRecords(find);
                BacktestGroups.Clear();
                foreach (var backtestGroupRecord in backtestGroups)
                {
                    BacktestGroups.Add(backtestGroupRecord);
                }

                SelectedBacktestGroupView = null;
            }
        }

        public void QueryBacktestGroup()
        {
            string query = QueryBacktestGroupText;
            if (SelectedBacktestGroupView == null)
            {
                OpenErrorWindow("Please select backtest group");
            }        
            else if (string.IsNullOrWhiteSpace(query))
            {
                OpenErrorWindow("Please enter query");                
            }
            else
            {
                var backtests = IoC.Get<IBacktestRepository>().GetBacktestRecords(SelectedBacktestGroupView, query);
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
                IoC.Get<IMediator>().Broadcast(new LinkedNavigationEvent
                {
                    NavigationAction = NavigationAction.NewLinkedWindow,
                    Destination = destination,
                    Group = NavigationGroup.Backtest,
                    ColorGroup = SelectedColorGroup,
                    Key = SelectedBacktestView.GUID
                });
            }
            else
            {
                IoC.Get<IMediator>().Broadcast(new LinkedNavigationEvent
                {
                    NavigationAction = NavigationAction.NewWindow,
                    Destination = NavigationDestination.Error,
                    Message = "Please select backtest"
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

        #region Private Methods

        void OpenErrorWindow(string errorMessage)
        {
            IoC.Get<IMediator>().Broadcast(new LinkedNavigationEvent
            {
                NavigationAction = NavigationAction.NewWindow,
                Destination = NavigationDestination.Error,
                Message = errorMessage
            });            
        }

        #endregion

        #region Base Class Overrides

        public override void Handle(LinkedNavigationEvent linkedNavigationEvent)
        {
            //ignore events for now
        }

        #endregion
    }



}
