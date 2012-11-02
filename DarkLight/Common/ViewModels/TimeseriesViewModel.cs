using Caliburn.Micro;
using DarkLight.Customizations;
using DarkLight.Events;
using DarkLight.Repositories;

namespace DarkLight.Common.ViewModels
{
    public class TimeseriesViewModel : DarkLightScreen
    {
        public TimeseriesViewModel()
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

        private BindableCollection<string> _timeseries;

        public BindableCollection<string> Timeseries
        {
            get { return _timeseries; }
            set
            {
                _timeseries = value;
                NotifyOfPropertyChange(() => Timeseries);
            }
        }

        #endregion

        #region Base Class Overrides

        public override void Initialize(LinkedNavigationEvent linkedNavigationEvent)
        {
            var timeseries = IoC.Get<IBacktestRepository>().GetBacktestTimeseries(linkedNavigationEvent.Key);
            TestField = linkedNavigationEvent.Key;
        }

        #endregion
    }
}
