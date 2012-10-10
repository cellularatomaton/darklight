using System.Windows.Media;
using DarkLight.Events;
using DarkLight.Interfaces;

namespace DarkLight.Services
{
    public interface IFilterService
    {
        IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter();
        IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationAction navigationAction);
        IFilter<LinkedNavigationEvent> GetLinkedNavigationFilter(NavigationAction navigationAction, Color colorGroup);
    }
}
