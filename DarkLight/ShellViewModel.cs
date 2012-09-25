using Caliburn.Micro;
using DarkLight.ViewModels;
using System.ComponentModel.Composition;

namespace DarkLight 
{
    [Export(typeof(IShell))]
    public class ShellViewModel : PropertyChangedBase, IShell
    {
        private TestViewModel testViewModel;
        public TestViewModel TestViewModel
        {
            get { return testViewModel; }
            set 
            { 
                testViewModel = value;
                NotifyOfPropertyChange(() => TestViewModel);
            }
        }

        public ShellViewModel()
        {
            TestViewModel = new TestViewModel();
        }
    }
}
