using System;
using System.ComponentModel;
using System.Windows.Data;
using Caliburn.Micro;
using DarkLight.Backtest.Models;
using DarkLight.Customizations;
using DarkLight.Events;
using DarkLight.Repositories;
using DarkLight.Common.Models;
using DarkLight.Utilities;

namespace DarkLight.Common.ViewModels
{
    public class FillsViewModel : DarkLightScreen
    {
        #region Properties

        private BindableCollection<DarkLightFill> _fills;
        public BindableCollection<DarkLightFill> Fills
        {
            get { return _fills; }
            set
            {
                _fills = value;
                NotifyOfPropertyChange(() => Fills);
            }
        }

        public ICollectionView FillView { get; set; }
    
        #endregion

        #region Constructor

        public FillsViewModel()
        {
            Fills = new BindableCollection<DarkLightFill>();
        }

        #endregion

        #region Public Methods

        public void AddFill()
        {
            Fills.Add(MockUtilities.GenerateFills("backtestidToImplement", 1)[0]);
        }

        #endregion

        #region Base Class Overrides

        public override void Initialize(LinkedNavigationEvent linkedNavigationEvent)
        {
            var fills = IoC.Get<IBacktestRepository>().GetBacktestFills(linkedNavigationEvent.Key);
            Fills.Clear();
            foreach (var fill in fills)
            {
                Fills.Add(fill);
            }

            FillView = CollectionViewSource.GetDefaultView(Fills);
        }

        #endregion

    }
}
