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

namespace DarkLight.Backtest.ViewModels
{
    class BacktestStatusViewModel : DarkLightScreen
    {
        double _progress1 = 0.5;
        double _progress2 = 0.5;
        double _progress3 = 0.5;
        double _progress4 = 0.5;

        public double ProgressSlot1
        {
            get { return _progress1; }
            set
            {
                _progress1 = value;
                NotifyOfPropertyChange(() => ProgressSlot1);
            }
        }

        public double ProgressSlot2
        {
            get { return _progress2; }
            set
            {
                _progress2 = value;
                NotifyOfPropertyChange(() => ProgressSlot2);
            }
        }

        public double ProgressSlot3
        {
            get { return _progress3; }
            set
            {
                _progress3 = value;
                NotifyOfPropertyChange(() => ProgressSlot3);
            }
        }

        public double ProgressSlot4
        {
            get { return _progress4; }
            set
            {
                _progress4 = value;
                NotifyOfPropertyChange(() => ProgressSlot4);
            }
        }

        public void Increment()
        {
            ProgressSlot1 += 0.1;
        }

        public void Decrement()
        {
            ProgressSlot1 -= 0.1;
        }

    }
}
