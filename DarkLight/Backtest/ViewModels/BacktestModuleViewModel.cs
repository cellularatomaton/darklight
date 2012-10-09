using System;
using System.Windows.Media;
using DarkLight.Events;
using Caliburn.Micro;
using DarkLight.Services;
using System.Linq;

namespace DarkLight.Backtest.ViewModels
{
    public class BacktestModuleViewModel : Screen
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

        public BindableCollection<LinkGroup> LinkGroups
        {
            get
            {
                var _eventGroupValues = Enum.GetValues(typeof(LinkGroup));
                return new BindableCollection<LinkGroup>(_eventGroupValues.Cast<LinkGroup>().AsEnumerable());
            }
        }

        private LinkGroup _selectedLinkGroup;
        public LinkGroup SelectedLinkGroup
        {
            get { return _selectedLinkGroup; }
            set
            {
                _selectedLinkGroup = value;
                NotifyOfPropertyChange(() => SelectedLinkGroup);
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

        public BacktestModuleViewModel(IColorService colorService)
        {
            _colorService = colorService;
            this.DisplayName = this.GetType().Name;
        }

        public void ShowNewFillsWindow()
        {
            IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent
            {
                LinkGroup = SelectedLinkGroup,
                ColorGroup = _colorService.GetColorGroups().First(),
                Destination = NavigationDestination.Fills,
                Key = TestKey,
            });
        }

        public void ShowNewIndicatorsWindow()
        {
            IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent
            {
                LinkGroup = SelectedLinkGroup,
                ColorGroup = _colorService.GetColorGroups().First(),
                Destination = NavigationDestination.Indicators,
                Key = TestKey,
            });
        }

        public void ShowNewMessagesWindow()
        {
            IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent
            {
                LinkGroup = SelectedLinkGroup,
                ColorGroup = _colorService.GetColorGroups().First(),
                Destination = NavigationDestination.Messages,
                Key = TestKey,
            });
        }

        public void ShowNewPositionsWindow()
        {
            IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent
            {
                LinkGroup = SelectedLinkGroup,
                ColorGroup = _colorService.GetColorGroups().First(),
                Destination = NavigationDestination.Positions,
                Key = TestKey,
            });
        }

        public void ShowNewResponsesWindow()
        {
            IoC.Get<IEventAggregator>().Publish(new LinkedNavigationEvent
            {
                LinkGroup = SelectedLinkGroup,
                ColorGroup = _colorService.GetColorGroups().First(),
                Destination = NavigationDestination.Response,
                Key = TestKey,
            });
        }
    }
}
