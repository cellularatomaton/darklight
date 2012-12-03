using System.Windows.Media;
using DarkLight.Events;
using DarkLight.Interfaces;

namespace DarkLight.Services
{
    public interface IFilterService
    {
        IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationAction navigationAction, NavigationDestination navigationDestination, Color colorGroup);
        IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationAction navigationAction, NavigationGroup navigationGroup, Color colorGroup);
        IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationAction navigationAction, NavigationDestination navigationDestination);
        IFilter<TradeEvent> GetTradeFilter(string key);                
    }
}
