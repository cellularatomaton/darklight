using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using DarkLight.Common.ViewModels;
using DarkLight.Customizations;

namespace DarkLight.Optimization.ViewModels
{
    public class OptimizationSchedulerViewModel : DarkLightScreen
    {
        public OptimizationSchedulerViewModel()
        {
            this.DisplayName = this.GetType().Name;
        }
    }
}