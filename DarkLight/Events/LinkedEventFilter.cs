using System;
using System.Windows.Media;
using DarkLight.Interfaces;

namespace DarkLight.Events
{
    public class LinkedEventFilter : IFilter<LinkedNavigationEvent>
    {
        private readonly NavigationAction _navigationAction;
        private readonly NavigationDestination _navigationDestination;
        private readonly NavigationGroup _navigationGroup;
        private readonly Color _colorGroup;

        public LinkedEventFilter(NavigationAction navigationAction, NavigationDestination navigationDestination, Color colorGroup)
        {
            _navigationAction = navigationAction;
            _navigationDestination = navigationDestination;
            _colorGroup = colorGroup;
        }

        public LinkedEventFilter(NavigationAction navigationAction, NavigationGroup navigationGroup, Color colorGroup)
        {
            _navigationAction = navigationAction;
            _navigationGroup = navigationGroup;
            _colorGroup = colorGroup;
        }

        #region Implementation of IFilter<LinkedEvent>

        public bool IsPassedBy(LinkedNavigationEvent message)
        {
            //TODO refine filter model
            if (message.Group != NavigationGroup.Default)
            {
                if (_navigationAction == message.NavigationAction &&
                    _navigationGroup == message.Group &&
                    _colorGroup == message.ColorGroup)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (_navigationAction == message.NavigationAction &&
                    _navigationDestination == message.Destination &&
                    _navigationGroup == message.Group &&
                    _colorGroup == message.ColorGroup)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion
    
    }
}