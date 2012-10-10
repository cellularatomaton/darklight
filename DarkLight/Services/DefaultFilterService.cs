using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using DarkLight.Events;
using DarkLight.Interfaces;

namespace DarkLight.Services
{
    public class DefaultFilterService : IFilterService
    {
        private IColorService _colorService;

        public DefaultFilterService(IColorService colorService)
        {
            _colorService = colorService;
        }

        #region Implementation of IFilterService

        public IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter()
        {
            return new ShellLinkedEventFilter();
        }

        public IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationAction navigationAction)
        {
            var _colorGroup = _colorService.GetDefaultColorGroup();
            return GetLinkedNavigationFilter(navigationAction, _colorGroup);
        }

        public IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationAction navigationAction, Color colorGroup)
        {
            return new LinkedEventFilter(navigationAction, colorGroup);
        }

        #endregion
    }
}
