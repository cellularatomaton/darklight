using Caliburn.Micro;
using DarkLight.Events;

namespace DarkLight.Customizations
{
    public class DarkLightScreen : Conductor<Screen>.Collection.OneActive
    {
        public DarkLightScreen()
        {
            this.DisplayName = this.GetType().Name;
        }

        public virtual void Configure(string key)
        {
            this.DisplayName = key;
        }

        public virtual void Initialize(LinkedNavigationEvent key)
        {
           
        }
    }

}