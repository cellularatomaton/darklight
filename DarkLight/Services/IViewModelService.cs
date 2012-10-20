using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkLight.Common.ViewModels;
using DarkLight.Customizations;
using DarkLight.Events;

namespace DarkLight.Services
{
    public interface IViewModelService
    {
        DarkLightScreen GetScreenForNavigationEvent(LinkedNavigationEvent linkedNavigationEvent);
    }
}
