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
