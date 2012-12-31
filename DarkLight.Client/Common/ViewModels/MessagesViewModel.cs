using Caliburn.Micro;
using DarkLight.Client.Customizations;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.Repository;

namespace DarkLight.Client.Common.ViewModels
{
    public class MessagesViewModel : DarkLightScreen
    {
        public MessagesViewModel()
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

        private BindableCollection<string> _messages;
        public BindableCollection<string> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                NotifyOfPropertyChange(() => Messages);
            }
        }

        #endregion

        #region Base Class Overrides

        public override void Initialize(LinkedNavigationEvent linkedNavigationEvent)
        {
            var messages = IoC.Get<IBacktestRepository>().GetBacktestMessages(linkedNavigationEvent.Key);
            TestField = linkedNavigationEvent.Key;
        }

        #endregion
  
    }
}
