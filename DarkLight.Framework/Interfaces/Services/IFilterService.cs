using System.Windows.Media;
using DarkLight.Framework.Enums;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.Common;

namespace DarkLight.Framework.Interfaces.Services
{
    public interface IFilterService
    {
        IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationAction navigationAction, NavigationDestination navigationDestination, Color colorGroup);
        IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationAction navigationAction, NavigationGroup navigationGroup, Color colorGroup);
        IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationAction navigationAction, NavigationDestination navigationDestination);
        IFilter<TradeEvent> GetTradeFilter(string key);                
    }
}
