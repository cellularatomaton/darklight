using System.ComponentModel;
using System.Windows.Data;
using Caliburn.Micro;
using DarkLight.Common.Models;
using DarkLight.Customizations;
using DarkLight.Events;
using DarkLight.Repositories;
using DarkLight.Utilities;

namespace DarkLight.Common.ViewModels
{
    public class OrdersViewModel : DarkLightTradeScreen
    {
        #region Properties

        string _sortColumn = "Time";
        ListSortDirection _sortDirection = ListSortDirection.Descending;

        private BindableCollection<DarkLightOrder> _orders;
        public BindableCollection<DarkLightOrder> Orders
        {
            get { return _orders; }
            set
            {
                _orders = value;
                NotifyOfPropertyChange(() => Orders);
            }
        }

        public ICollectionView OrderView { get; set; }
    
        #endregion

        #region Constructor

        public OrdersViewModel()
        {
            Orders = new BindableCollection<DarkLightOrder>();
        }

        #endregion

        #region Public Methods

        public void AddOrder()
        {
            Orders.Add(MockUtilities.GenerateOrders("backtestidToImplement",1)[0]);
        }

        public void Sort(string column)
        {
            if (_sortColumn == column)
                _sortDirection = _sortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            else //default
                _sortDirection = ListSortDirection.Ascending;

            _sortColumn = column;
            OrderView.SortDescriptions.Clear();
            OrderView.SortDescriptions.Add(new SortDescription(_sortColumn, _sortDirection));
        }

        #endregion

        #region Base Class Overrides

        public override void Initialize(LinkedNavigationEvent linkedNavigationEvent)
        {
            var orders = IoC.Get<IBacktestRepository>().GetBacktestOrders(linkedNavigationEvent.Key);
            Orders.Clear();
            foreach (var order in orders)
            {
                Orders.Add(order);
            }

            OrderView = CollectionViewSource.GetDefaultView(Orders);
            base.Initialize(linkedNavigationEvent);
        }

        protected override void AddTrade(TradeEvent tradeEvent)
        {
            Orders.Add(tradeEvent.Order);
        }

        #endregion
    }
}
