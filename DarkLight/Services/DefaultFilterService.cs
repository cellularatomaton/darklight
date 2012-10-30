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

        public IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationAction navigationAction, NavigationDestination navigationDestination)
        {
            var _colorGroup = _colorService.GetDefaultColorGroup();
            return GetLinkedNavigationFilter(navigationAction, navigationDestination, _colorGroup);
        }

        public IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationAction navigationAction, NavigationDestination navigationDestination, Color colorGroup)
        {
            return new LinkedEventFilter(navigationAction, navigationDestination, colorGroup);
        }

        public IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationAction navigationAction, NavigationGroup navigationGroup, Color colorGroup)
        {
            return new LinkedEventFilter(navigationAction, navigationGroup, colorGroup);
        }

        #endregion
   
    }
}
