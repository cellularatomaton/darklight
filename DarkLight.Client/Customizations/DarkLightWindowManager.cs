using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Caliburn.Micro;

namespace DarkLight.Client.Customizations
{
    public class DarkLightWindowManager : WindowManager
    {
        public override void ShowWindow(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            if (rootModel is Screen)
            {
                var _screen = rootModel as Screen;
                if(!_screen.IsActive)
                {
                    base.ShowWindow(rootModel, context, settings);
                }
            }
            else
            {
                base.ShowWindow(rootModel, context, settings);
            }
        }
    }
}
