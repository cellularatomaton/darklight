using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Caliburn.Micro;
using DarkLight.Client.Customizations;
using System.Collections;
using DarkLight.Framework.Enums;
using DarkLight.Framework.Events;
using DarkLight.Framework.Interfaces.CEP;
using DarkLight.Framework.Interfaces.Common;
using DarkLight.Framework.Interfaces.Services;
using DarkLight.Infrastructure.Mediator;
using com.espertech.esper.client;
using EventType = DarkLight.Framework.Enums.EventType;

namespace DarkLight.Client.Common.ViewModels
{
    public class LinkableViewModel : DarkLightScreen, DarkLight.Framework.Interfaces.CEP.IHandle<LinkedNavigationEvent>
    {

        #region Properties

        protected IColorService _colorService;
        protected IViewModelService _viewModelService;

        protected NavigationDestination _destination;
        public NavigationDestination Destination
        {
            get { return _destination; }
            set
            {
                _destination = value;
                NotifyOfPropertyChange(() => Destination);
            }
        }

        protected NavigationGroup _group;
        public NavigationGroup Group
        {
            get { return _group; }
            set
            {
                _group = value;
                NotifyOfPropertyChange(() => Group);
            }
        }

        public BindableCollection<Color> ColorGroups
        {
            get { return _colorService.GetColorGroups(); }
        }

        protected Color _selectedColorGroup;
        public Color SelectedColorGroup
        {
            get { return _selectedColorGroup; }
            set
            {
                _selectedColorGroup = value;
                NotifyOfPropertyChange(() => SelectedColorGroup);
            }
        }

        #endregion

        #region Constructor

        public LinkableViewModel(IColorService colorService, IViewModelService viewModelService)
        {
            _colorService = colorService;
            _viewModelService = viewModelService;
            SelectedColorGroup = _colorService.GetDefaultColorGroup();

            IoC.Get<IEventBroker>().Subscribe(this);
        }

        #endregion

        #region Private Methods

        void LoadView(LinkedNavigationEvent linkedNavigationEvent)
        {
            linkedNavigationEvent.Destination = Destination;
            var _viewModel = _viewModelService.GetScreenForNavigationEvent(linkedNavigationEvent);
            _viewModel.Initialize(linkedNavigationEvent);

            SelectedColorGroup = linkedNavigationEvent.ColorGroup;
            _viewModel.Configure(linkedNavigationEvent.Key);
            ActivateItem(_viewModel);
        }

        #endregion

        #region Implementation of IHandle<LinkedEvent>

        public virtual void Handle(LinkedNavigationEvent linkedNavigationEvent)
        {
            var _filter = IoC.Get<IFilterService>().GetLinkedNavigationFilter(NavigationAction.UpdateLinkedWindows, Group, _selectedColorGroup);
            if (_filter.IsPassedBy(linkedNavigationEvent))
            {
                LoadView(linkedNavigationEvent);
                Configure(Destination + " : " + linkedNavigationEvent.Key);
            }
        }

        #endregion

        #region Base Class Overrides

        public override void Initialize(LinkedNavigationEvent linkedNavigationEvent)
        {
            Destination = linkedNavigationEvent.Destination;
            Group = linkedNavigationEvent.Group;
            SelectedColorGroup = linkedNavigationEvent.ColorGroup;
            Configure(linkedNavigationEvent.Key);
        }

        #endregion

    }
}
