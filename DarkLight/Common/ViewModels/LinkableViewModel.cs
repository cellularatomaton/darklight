using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Caliburn.Micro;
using DarkLight.Events;
using System.Collections;
using DarkLight.Interfaces;
using DarkLight.Services;

namespace DarkLight.Common.ViewModels
{
    public class LinkableViewModel : Conductor<Screen>.Collection.OneActive, IHandle<LinkedNavigationEvent>
    {
        private IColorService _colorService;

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

        public BindableCollection<LinkGroup> EventGroups
        {
            get
            {
                var _eventGroupValues = Enum.GetValues(typeof (LinkGroup));
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

        public LinkableViewModel(IColorService colorService)
        {
            _colorService = colorService;
            IoC.Get<IEventAggregator>().Subscribe(this);
        }

        #region Implementation of IHandle<LinkedEvent>

        public void Handle(LinkedNavigationEvent linkedNavigationEvent)
        {
            var _filter = IoC.Get<IFilterService>().GetLinkedNavigationFilter(_selectedLinkGroup, _selectedColorGroup);
            if(_filter.IsPassedBy(linkedNavigationEvent))
            {
                LoadView(linkedNavigationEvent);
            }
        }

        #endregion

        private void LoadView(LinkedNavigationEvent linkedNavigationEvent)
        {
            DarkLightScreen _viewModel;
            switch (linkedNavigationEvent.Destination)
            {
                case NavigationDestination.Statistics:
                {
                    _viewModel = IoC.Get<StatisticsViewModel>();
                    
                    break;
                }
                case NavigationDestination.Results:
                {
                    _viewModel = IoC.Get<ResultsViewModel>();
                    break;
                }
                case NavigationDestination.Fills:
                {
                    _viewModel = IoC.Get<FillsViewModel>();
                    break;
                }
                case NavigationDestination.Indicators:
                {
                    _viewModel = IoC.Get<IndicatorsViewModel>();
                    break;
                }
                case NavigationDestination.Messages:
                {
                    _viewModel = IoC.Get<MessagesViewModel>();
                    break;
                }
                case NavigationDestination.TickData:
                {
                    _viewModel = IoC.Get<TickDataViewModel>();
                    break;
                }
                case NavigationDestination.Orders:
                {
                    _viewModel = IoC.Get<OrdersViewModel>();
                    break;
                }
                case NavigationDestination.Positions:
                {
                    _viewModel = IoC.Get<PositionsViewModel>();
                    break;
                }
                case NavigationDestination.TimeSeries:
                {
                    _viewModel = IoC.Get<TimeseriesViewModel>();
                    break;
                }
                default:
                {
                    _viewModel = IoC.Get<DefaultViewModel>();
                    break;
                }
            }
            _viewModel.Configure(linkedNavigationEvent.Key);
            ActivateItem(_viewModel);
        }
    }
}
