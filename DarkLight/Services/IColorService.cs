﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Caliburn.Micro;

namespace DarkLight.Services
{
    public interface IColorService
    {
        BindableCollection<Color> GetColorGroups();
        Color GetDefaultColorGroup();

    }
}
