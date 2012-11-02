using System.ComponentModel;
using System.Windows.Data;
using Caliburn.Micro;
using DarkLight.Common.Models;
using DarkLight.Customizations;
using DarkLight.Events;
using DarkLight.Repositories;
using DarkLight.Utilities;

namespace DarkLight.Common.ViewModels
{
    public class TickDataViewModel : DarkLightScreen
    {
        #region Properties

        string _sortColumn = "Time";
        ListSortDirection _sortDirection = ListSortDirection.Descending;

        private BindableCollection<DarkLightTick> _ticks;
        public BindableCollection<DarkLightTick> Ticks
        {
            get { return _ticks; }
            set
            {
                _ticks = value;
                NotifyOfPropertyChange(() => Ticks);
            }
        }

        public ICollectionView TickView { get; set; }
    
        #endregion

        #region Constructor

        public TickDataViewModel()
        {
            Ticks = new BindableCollection<DarkLightTick>();
        }

        #endregion

        #region Public Methods

        public void AddTick()
        {
            Ticks.Add(MockUtilities.GenerateTicks("backtestidToImplement",1)[0]);
        }

        public void Sort(string column)
        {
            if (_sortColumn == column)
                _sortDirection = _sortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            else //default
                _sortDirection = ListSortDirection.Ascending;

            _sortColumn = column;
            TickView.SortDescriptions.Clear();
            TickView.SortDescriptions.Add(new SortDescription(_sortColumn, _sortDirection));
        }

        #endregion

        #region Base Class Overrides

        public override void Initialize(LinkedNavigationEvent linkedNavigationEvent)
        {
            var ticks = IoC.Get<IBacktestRepository>().GetBacktestTicks(linkedNavigationEvent.Key);
            Ticks.Clear();
            foreach (var tick in ticks)
            {
                Ticks.Add(tick);
            }

            TickView = CollectionViewSource.GetDefaultView(Ticks);
        }

        #endregion
    }
}
