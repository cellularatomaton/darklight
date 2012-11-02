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
    public class PositionsViewModel : DarkLightScreen
    {
        #region Properties

        string _sortColumn = "Time";
        ListSortDirection _sortDirection = ListSortDirection.Descending;

        private BindableCollection<DarkLightPosition> _positions;
        public BindableCollection<DarkLightPosition> Positions
        {
            get { return _positions; }
            set
            {
                _positions = value;
                NotifyOfPropertyChange(() => Positions);
            }
        }

        public ICollectionView PositionView { get; set; }
    
        #endregion

        #region Constructor

        public PositionsViewModel()
        {
            Positions = new BindableCollection<DarkLightPosition>();
        }

        #endregion

        #region Public Methods

        public void AddPosition()
        {
            Positions.Add(MockUtilities.GeneratePositions("backtestidToImplement",1)[0]);
        }

        public void Sort(string column)
        {
            if (_sortColumn == column)
                _sortDirection = _sortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            else //default
                _sortDirection = ListSortDirection.Ascending;

            _sortColumn = column;
            PositionView.SortDescriptions.Clear();
            PositionView.SortDescriptions.Add(new SortDescription(_sortColumn, _sortDirection));
        }

        #endregion

        #region Base Class Overrides

        public override void Initialize(LinkedNavigationEvent linkedNavigationEvent)
        {
            var positions = IoC.Get<IBacktestRepository>().GetBacktestPositions(linkedNavigationEvent.Key);
            Positions.Clear();
            foreach (var position in positions)
            {
                Positions.Add(position);
            }

            PositionView = CollectionViewSource.GetDefaultView(Positions);
        }

        #endregion
    }
}
