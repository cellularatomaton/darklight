using Caliburn.Micro;
using DarkLight.Customizations;
using DarkLight.Events;
using DarkLight.Repositories;

namespace DarkLight.Common.ViewModels
{
    public class PositionsViewModel : DarkLightScreen
    {
        public PositionsViewModel()
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

        private BindableCollection<string> _positions;
        public BindableCollection<string> Positions
        {
            get { return _positions; }
            set
            {
                _positions = value;
                NotifyOfPropertyChange(() => Positions);
            }
        }

        #endregion

        #region Base Class Overrides

        public override void Initialize(LinkedNavigationEvent linkedNavigationEvent)
        {
            var positions = IoC.Get<IBacktestRepository>().GetBacktestPositions(linkedNavigationEvent.Key);
            TestField = linkedNavigationEvent.Key;
        }

        #endregion
    }
}
