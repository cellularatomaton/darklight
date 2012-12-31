using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Caliburn.Micro;
using DarkLight.Client.Customizations;
using DarkLight.Framework.Events;

namespace DarkLight.Client.Common.ViewModels
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
