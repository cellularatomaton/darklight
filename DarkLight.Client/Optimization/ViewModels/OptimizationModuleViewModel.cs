using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using DarkLight.Client.Customizations;

namespace DarkLight.Client.Optimization.ViewModels
{
    public class OptimizationModuleViewModel : DarkLightScreen
    {
        public OptimizationModuleViewModel()
        {
            this.DisplayName = this.GetType().Name;
        }
    }
}
