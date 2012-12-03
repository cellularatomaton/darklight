using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Caliburn.Micro;
using DarkLight.Customizations;
using DarkLight.Events;
using DarkLight.Repositories;

namespace DarkLight.Common.ViewModels
{
    class ErrorViewModel : DarkLightScreen
    {
        public string ErrorMessage { get; set; }

        public ErrorViewModel()
        {
        }

        public override void Initialize(LinkedNavigationEvent linkedNavigationEvent)
        {
            ErrorMessage = linkedNavigationEvent.Message;
        }

        public void OK()
        {
            TryClose();            
        }

    }
}
