using Caliburn.Micro;

namespace DarkLight.Common.ViewModels
{
    public class DarkLightScreen : Screen
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