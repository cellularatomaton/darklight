using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Client.Customizations;
using DarkLight.Framework.Events;

namespace DarkLight.Client.Customizations
{
    public interface IViewModelService
    {
        DarkLightScreen GetScreenForNavigationEvent(LinkedNavigationEvent linkedNavigationEvent);
    }
}
