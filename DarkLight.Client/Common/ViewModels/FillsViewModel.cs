using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Data;
using Caliburn.Micro;
using DarkLight.Client.Customizations;
using DarkLight.Framework.Data.Common;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.Repository;
using DarkLight.Framework.Utilities;

namespace DarkLight.Client.Common.ViewModels
{
    public class FillsViewModel : DarkLightTradeScreen
    {
        #region Properties

        string _sortColumn = "Time";
        ListSortDirection _sortDirection = ListSortDirection.Descending;

        private BindableCollection<DarkLightFill> _fills;
        public BindableCollection<DarkLightFill> Fills
        {
            get { return _fills; }
            set
            {
                _fills = value;
                NotifyOfPropertyChange(() => Fills);
            }
        }

        public ICollectionView FillView { get; set; }
    
        #endregion

        #region Constructor

        public FillsViewModel()
        {
            Fills = new BindableCollection<DarkLightFill>();
        }

        #endregion

        #region Public Methods

        public void AddFill(DarkLightFill fill)
        {
            Fills.Add(MockUtilities.GenerateFills("backtestidToImplement", 1)[0]);
        }

        public void Sort(string column)
        {
            if (_sortColumn == column)
                _sortDirection = _sortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            else //default
                _sortDirection = ListSortDirection.Ascending;

            _sortColumn = column;
            FillView.SortDescriptions.Clear();
            FillView.SortDescriptions.Add(new SortDescription(_sortColumn, _sortDirection));
        }

        #endregion

        #region Base Class Overrides

        public override void Initialize(LinkedNavigationEvent linkedNavigationEvent)
        {
            var fills = IoC.Get<IBacktestRepository>().GetBacktestFills(linkedNavigationEvent.Key);
            Fills.Clear();
            foreach (var fill in fills)
            {
                Fills.Add(fill);
            }

            FillView = CollectionViewSource.GetDefaultView(Fills);
            base.Initialize(linkedNavigationEvent);
        }

        protected override void AddTrade(TradeEvent tradeEvent)
        {
            Fills.Add(tradeEvent.Fill);
        }

        #endregion

    }
}
