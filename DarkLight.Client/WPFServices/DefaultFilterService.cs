using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using DarkLight.Framework.Enums;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.Common;
using DarkLight.Framework.Interfaces.Services;
using DarkLight.Infrastructure.Filter;

namespace DarkLight.Client.WPFServices
{
    public class DefaultFilterService : IFilterService
    {
        private IColorService _colorService;

        public DefaultFilterService(IColorService colorService)
        {
            _colorService = colorService;
        }

        #region Implementation of IFilterService

        //CURRENTLY UNUSED
        public IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationAction navigationAction, NavigationDestination navigationDestination)
        {
            var _colorGroup = _colorService.GetDefaultColorGroup();
            return GetLinkedNavigationFilter(navigationAction, navigationDestination, _colorGroup);
        }

        //CURRENTLY UNUSED
        public IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationAction navigationAction, NavigationDestination navigationDestination, Color colorGroup)
        {
            return new LinkedEventFilter(navigationAction, navigationDestination, colorGroup);
        }

        public IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationAction navigationAction, NavigationGroup navigationGroup, Color colorGroup)
        {
            return new LinkedEventFilter(navigationAction, navigationGroup, colorGroup);
        }

        public IFilter<TradeEvent> GetTradeFilter(string key)
        {
            return new TradeEventFilter(key);
        }

        #endregion
   
    }
}
