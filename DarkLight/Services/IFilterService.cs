using System.Windows.Media;
using DarkLight.Events;
using DarkLight.Interfaces;

namespace DarkLight.Services
{
    public interface IFilterService
    {
        LinkGroup GetDefaultLinkGroup(NavigationDestination destination);

        IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter();
        IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(LinkGroup linkGroup);
        IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(LinkGroup linkGroup, Color colorGroup);
        IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationDestination destination);
        IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationDestination destination, Color colorGroup);
    }
}
