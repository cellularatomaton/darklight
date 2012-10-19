using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Caliburn.Micro;
using DarkLight.Customizations;
using DarkLight.Events;
using DarkLight.Services;

namespace DarkLight.Common.ViewModels
{
    public class EventPublisherViewModel : DarkLightScreen
    {
        public BindableCollection<Color> ColorGroups
        {
            get { return _colorService.GetColorGroups(); }
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

        public BindableCollection<NavigationAction> NavigationActions
        {
            get
            {
                var _navigationActionValues = Enum.GetValues(typeof(NavigationAction));
                return new BindableCollection<NavigationAction>(_navigationActionValues.Cast<NavigationAction>().AsEnumerable());
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

        public BindableCollection<NavigationDestination> Destinations
        {
            get
            {
                var _destinationValues = Enum.GetValues(typeof(NavigationDestination));
                return new BindableCollection<NavigationDestination>(_destinationValues.Cast<NavigationDestination>().AsEnumerable());
            }
        }

        private NavigationDestination _selectedDestination;
        public NavigationDestination SelectedDestination
        {
            get { return _selectedDestination; }
            set
            {
                _selectedDestination = value;
                NotifyOfPropertyChange(() => SelectedDestination);
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

        private IColorService _colorService;

        public EventPublisherViewModel(IColorService colorService)
        {
            _colorService = colorService;
            SelectedColorGroup = _colorService.GetDefaultColorGroup();
        }

        public void SendMessage()
        {
            IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent
            {
                NavigationAction = SelectedNavigationAction,
                ColorGroup = SelectedColorGroup,
                Destination = SelectedDestination,
                Key = TestKey,
            });
        }
    }
}
