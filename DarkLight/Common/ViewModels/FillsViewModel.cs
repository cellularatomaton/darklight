using Caliburn.Micro;
using DarkLight.Customizations;
using DarkLight.Events;
using DarkLight.Repositories;

namespace DarkLight.Common.ViewModels
{
    public class FillsViewModel : DarkLightScreen
    {
        public FillsViewModel()
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

        private BindableCollection<string> _fills;
        public BindableCollection<string> Fills
        {
            get { return _fills; }
            set
            {
                _fills = value;
                NotifyOfPropertyChange(() => Fills);
            }
        }

        #endregion

        #region Base Class Overrides

        public override void Initialize(LinkedNavigationEvent linkedNavigationEvent)
        {
            var fills = IoC.Get<IBacktestRepository>().GetBacktestFills(linkedNavigationEvent.Key);
            TestField = linkedNavigationEvent.Key;
        }

        #endregion


    }
}
