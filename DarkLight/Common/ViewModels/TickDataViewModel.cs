using Caliburn.Micro;
using DarkLight.Customizations;
using DarkLight.Events;
using DarkLight.Repositories;

namespace DarkLight.Common.ViewModels
{
    public class TickDataViewModel : DarkLightScreen
    {
        public TickDataViewModel()
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

        private BindableCollection<string> _ticks;
        public BindableCollection<string> Ticks
        {
            get { return _ticks; }
            set
            {
                _ticks = value;
                NotifyOfPropertyChange(() => Ticks);
            }
        }

        #endregion

        #region Base Class Overrides

        public override void Initialize(LinkedNavigationEvent linkedNavigationEvent)
        {
            var results = IoC.Get<IBacktestRepository>().GetBacktestTicks(linkedNavigationEvent.Key);
            TestField = linkedNavigationEvent.Key;
        }

        #endregion
    }
}
