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
