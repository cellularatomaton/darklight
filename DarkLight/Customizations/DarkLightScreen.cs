using Caliburn.Micro;

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
    }

}