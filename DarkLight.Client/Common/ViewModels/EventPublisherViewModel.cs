using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Caliburn.Micro;
using DarkLight.Client.Customizations;
using DarkLight.Framework.Enums;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.Common;
using DarkLight.Framework.Interfaces.Services;
using DarkLight.Framework.Utilities;
using DarkLight.Infrastructure;

namespace DarkLight.Client.Common.ViewModels
{
    public class EventPublisherViewModel : DarkLightScreen
    {
        
        #region Properties

        public BindableCollection<EventType> EventTypes
        {
            get
            {
                var _eventTypes = Enum.GetValues(typeof(EventType));
                return new BindableCollection<EventType>(_eventTypes.Cast<EventType>().AsEnumerable());
            }
        }

        public BindableCollection<Color> ColorGroups
        {
            get { return _colorService.GetColorGroups(); }
        }

        public BindableCollection<NavigationAction> NavigationActions
        {
            get
            {
                var _navigationActionValues = Enum.GetValues(typeof(NavigationAction));
                return new BindableCollection<NavigationAction>(_navigationActionValues.Cast<NavigationAction>().AsEnumerable());
            }
        }

        public BindableCollection<NavigationGroup> NavigationGroups
        {
            get
            {
                var _groupValues = Enum.GetValues(typeof(NavigationGroup));
                return new BindableCollection<NavigationGroup>(_groupValues.Cast<NavigationGroup>().AsEnumerable());
            }
        }

        public BindableCollection<NavigationDestination> NavigationDestinations
        {
            get
            {
                var _destinationValues = Enum.GetValues(typeof(NavigationDestination));
                return new BindableCollection<NavigationDestination>(_destinationValues.Cast<NavigationDestination>().AsEnumerable());
            }
        }

        private EventType _selectedEventType;
        public EventType SelectedEventType
        {
            get { return _selectedEventType; }
            set
            {
                _selectedEventType = value;
                NotifyOfPropertyChange(() => SelectedEventType);
            }
        }

        private string _testKey = "TestKey";
        public string TestKey
        {
            get { return _testKey; }
            set
            {
                _testKey = value;
                NotifyOfPropertyChange(() => TestKey);
            }
        }

        private Color _selectedColorGroup;
        public Color SelectedColorGroup
        {
            get { return _selectedColorGroup; }
            set
            {
                _selectedColorGroup = value;
                NotifyOfPropertyChange(() => SelectedColorGroup);
            }
        }

        private NavigationAction _selectedNavigationAction;
        public NavigationAction SelectedNavigationAction
        {
            get { return _selectedNavigationAction; }
            set
            {
                _selectedNavigationAction = value;
                NotifyOfPropertyChange(() => SelectedNavigationAction);
            }
        }

        private NavigationGroup _selectedGroup;
        public NavigationGroup SelectedNavigationGroup
        {
            get { return _selectedGroup; }
            set
            {
                _selectedGroup = value;
                NotifyOfPropertyChange(() => SelectedNavigationGroup);
            }
        }

        private NavigationDestination _selectedDestination;
        public NavigationDestination SelectedNavigationDestination
        {
            get { return _selectedDestination; }
            set
            {
                _selectedDestination = value;
                NotifyOfPropertyChange(() => SelectedNavigationDestination);
            }
        }

        private IColorService _colorService;

        #endregion

        #region Constructor

        public EventPublisherViewModel(IColorService colorService)
        {
            _colorService = colorService;
            SelectedColorGroup = _colorService.GetDefaultColorGroup();
        }

        #endregion

        #region Public Methods

        public void SendMessage()
        {
            if (SelectedEventType == EventType.LinkedNavigation)
            {
                IoC.Get<IMediator>().Broadcast(new LinkedNavigationEvent
                                                   {
                                                       NavigationAction = SelectedNavigationAction,
                                                       ColorGroup = SelectedColorGroup,
                                                       Destination = SelectedNavigationDestination,
                                                       Key = TestKey,
                                                   });
            }
            else if (SelectedEventType == EventType.Trade)
            {
                IoC.Get<IMediator>().Broadcast(new TradeEvent
                {
                    Key = TestKey,
                    Fill = MockUtilities.GenerateFills(TestKey, 1)[0],
                    Order = MockUtilities.GenerateOrders(TestKey, 1)[0],
                    Position = MockUtilities.GeneratePositions(TestKey, 1)[0],
                    Tick = MockUtilities.GenerateTicks(TestKey, 1)[0],
                });
            }
        }

        #endregion

    }
}
