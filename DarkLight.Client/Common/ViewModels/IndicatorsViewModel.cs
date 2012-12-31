using Caliburn.Micro;
using DarkLight.Client.Customizations;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.Repository;

namespace DarkLight.Client.Common.ViewModels
{
    public class IndicatorsViewModel : DarkLightScreen
    {
        public IndicatorsViewModel()
        {
        }

        #region Properties

        private string _testField;
        public string TestField
        {
            get { return _testField; }
            set
            {
                _testField = value;
                NotifyOfPropertyChange(() => TestField);
            }
        }

        private BindableCollection<string> _orders;
        public BindableCollection<string> Orders
        {
            get { return _orders; }
            set
            {
                _orders = value;
                NotifyOfPropertyChange(() => Orders);
            }
        }

        #endregion

        #region Base Class Overrides

        public override void Initialize(LinkedNavigationEvent linkedNavigationEvent)
        {
            var orders = IoC.Get<IBacktestRepository>().GetBacktestOrders(linkedNavigationEvent.Key);
            TestField = linkedNavigationEvent.Key;
        }

        #endregion
   
    }
}
