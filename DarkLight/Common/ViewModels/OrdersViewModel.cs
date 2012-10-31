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
    public class OrdersViewModel : DarkLightScreen
    {
        #region Properties

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
        }

        #endregion
    }
}
