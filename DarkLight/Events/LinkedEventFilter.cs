using System.Windows.Media;
using DarkLight.Interfaces;

namespace DarkLight.Events
{
    public class LinkedEventFilter : IFilter<LinkedNavigationEvent>
    {
        private readonly LinkGroup _linkGroup;
        private readonly Color _colorGroup;

        public LinkedEventFilter(LinkGroup linkGroup, Color colorGroup)
        {
            _linkGroup = linkGroup;
            _colorGroup = colorGroup;
        }

        #region Implementation of IFilter<LinkedEvent>

        public bool IsPassedBy(LinkedNavigationEvent message)
        {
            if(_linkGroup == message.LinkGroup &&
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
            if(message.LinkGroup == LinkGroup.Shell)
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