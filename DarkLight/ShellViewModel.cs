using Caliburn.Micro;
using System.ComponentModel.Composition;
using DarkLight.Common;
using DarkLight.Common.ViewModels;

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
