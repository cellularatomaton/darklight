using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Caliburn.Micro;

namespace DarkLight.Services
{
    public class DefaultColorService : IColorService
    {
        #region Implementation of IColorService

        public BindableCollection<Color> GetColorGroups()
        {
            return new BindableCollection<Color>(new Color[] { Colors.Red, Colors.Orange, Colors.Yellow, Colors.Green, Colors.Blue, Colors.Purple, Colors.Pink });
        }

        public Color GetDefaultColorGroup()
        {
            return GetColorGroups().First();
        }

        #endregion
    }
}
