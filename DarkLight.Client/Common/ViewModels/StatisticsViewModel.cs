using Caliburn.Micro;
using DarkLight.Client.Customizations;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.Repository;

namespace DarkLight.Client.Common.ViewModels
{
    public class StatisticsViewModel : DarkLightScreen
    {
        public StatisticsViewModel()
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

        private BindableCollection<string> _statistics;
        public BindableCollection<string> Statistics
        {
            get { return _statistics; }
            set
            {
                _statistics = value;
                NotifyOfPropertyChange(() => Statistics);
            }
        }

        #endregion

        #region Base Class Overrides

        public override void Initialize(LinkedNavigationEvent linkedNavigationEvent)
        {
            var results = IoC.Get<IBacktestRepository>().GetBacktestStatistics(linkedNavigationEvent.Key);
            TestField = linkedNavigationEvent.Key;
        }

        #endregion

    }
}
