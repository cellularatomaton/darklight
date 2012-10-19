using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace DarkLight.Optimization.ViewModels
{
    public class OptimizationModuleViewModel : Screen
    {
        public OptimizationModuleViewModel()
        {
            this.DisplayName = this.GetType().Name;
        }
    }
}
