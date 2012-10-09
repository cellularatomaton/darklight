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

        public LinkGroup GetDefaultLinkGroup(NavigationDestination destination)
        {
            switch (destination)
            {
                case NavigationDestination.LiveTrading:
                case NavigationDestination.Optimization:
                case NavigationDestination.Backtest:
                {
                    return LinkGroup.Shell;
                    break;
                }
                case NavigationDestination.Fills:
                case NavigationDestination.Indicators:
                case NavigationDestination.Orders:
                case NavigationDestination.Results:
                case NavigationDestination.Positions:
                case NavigationDestination.Statistics:
                {
                    return LinkGroup.Backtest;
                    break;
                }
                default:
                {
                    return LinkGroup.Symbol;
                    break;
                }
            }
        }

        public IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter()
        {
            return new ShellLinkedEventFilter();
        }

        public IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(LinkGroup linkGroup)
        {
            var _colorGroup = _colorService.GetDefaultColorGroup();
            return GetLinkedNavigationFilter(linkGroup, _colorGroup);
        }

        public IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(LinkGroup linkGroup, Color colorGroup)
        {
            return new LinkedEventFilter(linkGroup, colorGroup);
        }

        public IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationDestination destination)
        {
            var linkGroup = GetDefaultLinkGroup(destination);
            return GetLinkedNavigationFilter(linkGroup);
        }

        public IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationDestination destination, Color colorGroup)
        {
            var _linkGroup = GetDefaultLinkGroup(destination);
            return GetLinkedNavigationFilter(_linkGroup, colorGroup);
        }

        #endregion
    }
}
