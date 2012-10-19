using Caliburn.Micro;

namespace DarkLight.Common.ViewModels
{
    public class TestViewModel : DarkLightScreen
    {
        public TestViewModel()
        {
        }

        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyOfPropertyChange(() => Name);
                NotifyOfPropertyChange(() => CanSayHello);
            }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set 
            { 
                message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        public bool CanSayHello
        {
            get { return !string.IsNullOrWhiteSpace(Name); }
        }

        public void SayHello()
        {
            Message = "Hello " + Name + "!";
        }
    }
}
