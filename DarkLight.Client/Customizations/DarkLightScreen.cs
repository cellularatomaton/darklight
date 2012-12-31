using Caliburn.Micro;
using DarkLight.Framework.Events;

namespace DarkLight.Client.Customizations
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