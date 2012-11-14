using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using DarkLight.Events;
using DarkLight.Interfaces;
using DarkLight.Backtest.Models;
using DarkLight.Customizations;
using DarkLight.Backtest.ViewModels;


namespace DarkLight.Services
{
    public interface IBacktestService
    {
        string RunBackTest(IHistDataService _histDataService, DarkLightResponse _response);
        BacktestRecord GetBackTest(string backtestID);
    }
}
