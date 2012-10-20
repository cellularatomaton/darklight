using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Caliburn.Micro;
using DarkLight.Customizations;
using DarkLight.Events;
using System.Collections;
using DarkLight.Interfaces;
using DarkLight.Services;

namespace DarkLight.Common.ViewModels
{
    public class LinkableViewModel : DarkLightScreen, IHandle<LinkedNavigationEvent>
    {
        private IColorService _colorService;
        private IViewModelService _viewModelService;

        private NavigationDestination _destination;
        public NavigationDestination Destination
        {
            get { return _destination; }
            set 
            { 
                _destination = value;
                NotifyOfPropertyChange(() => Destination);
            }
        }

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

        public LinkableViewModel(IColorService colorService, IViewModelService viewModelService)
        {
            _colorService = colorService;
            _viewModelService = viewModelService;
            SelectedColorGroup = _colorService.GetDefaultColorGroup();
            IoC.Get<IEventAggregator>().Subscribe(this);
        }

        #region Implementation of IHandle<LinkedEvent>

        public void Handle(LinkedNavigationEvent linkedNavigationEvent)
        {
            var _filter = IoC.Get<IFilterService>().GetLinkedNavigationFilter(NavigationAction.UpdateLinkedWindows, Destination, _selectedColorGroup);
            if(_filter.IsPassedBy(linkedNavigationEvent))
            {
                LoadView(linkedNavigationEvent);
            }
        }

        #endregion

        private void LoadView(LinkedNavigationEvent linkedNavigationEvent)
        {
            var _viewModel = _viewModelService.GetScreenForNavigationEvent(linkedNavigationEvent);
            SelectedColorGroup = linkedNavigationEvent.ColorGroup;
            _viewModel.Configure(linkedNavigationEvent.Key);
            ActivateItem(_viewModel);
        }
    }
}
