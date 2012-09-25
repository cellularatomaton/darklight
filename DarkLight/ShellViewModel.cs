using Caliburn.Micro;
using DarkLight.ViewModels;
using System.ComponentModel.Composition;

namespace DarkLight 
{
    //[Export(typeof(IShell))]
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive, IShell
    {
        public ShellViewModel()
        {
            ActivateItem(new TestViewModel());
        }
    }
}
