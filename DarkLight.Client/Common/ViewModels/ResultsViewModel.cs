using Caliburn.Micro;
using DarkLight.Client.Customizations;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.Repository;

namespace DarkLight.Client.Common.ViewModels
{
    public class ResultsViewModel : DarkLightScreen
    {
        public ResultsViewModel()
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

        private BindableCollection<string> _results;
        public BindableCollection<string> Results
        {
            get { return _results; }
            set
            {
                _results = value;
                NotifyOfPropertyChange(() => Results);
            }
        }

        #endregion

        #region Base Class Overrides

        public override void Initialize(LinkedNavigationEvent linkedNavigationEvent)
        {
            var results = IoC.Get<IBacktestRepository>().GetBacktestResults(linkedNavigationEvent.Key);
            TestField = linkedNavigationEvent.Key;
        }

        #endregion
    }
}
