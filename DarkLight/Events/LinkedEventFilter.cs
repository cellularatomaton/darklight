using System.Windows.Media;
using DarkLight.Interfaces;

namespace DarkLight.Events
{
    public class LinkedEventFilter : IFilter<LinkedNavigationEvent>
    {
        private readonly NavigationAction _navigationAction;
        private readonly Color _colorGroup;

        public LinkedEventFilter(NavigationAction navigationAction, Color colorGroup)
        {
            _navigationAction = navigationAction;
            _colorGroup = colorGroup;
        }

        #region Implementation of IFilter<LinkedEvent>

        public bool IsPassedBy(LinkedNavigationEvent message)
        {
            if(_navigationAction == message.NavigationAction &&
               _colorGroup == message.ColorGroup)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }

    public class ShellLinkedEventFilter : IFilter<LinkedNavigationEvent>
    {
        #region Implementation of IFilter<LinkedEventFilter>

        public bool IsPassedBy(LinkedNavigationEvent message)
        {
            if(message.NavigationAction == NavigationAction.Basic)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}