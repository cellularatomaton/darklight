using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using TradeLink.AppKit;

namespace DarkLight.Utilities
{
    public class PlottablePoint
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class PlottableValue<T>
    {
        public double X { get; set; }
        public double Y { get; set; }
        public T Value { get; set; }
    }

    public class TimePlot
    {
        public bool Selected { get; set; }
        public string Label { get; set; }
        public Color PointColor { get; set; }
        public List<TimePlotPoint> PlotPoints { get; set; }
    }

    public class TimePlotPoint
    {
        public DateTime Time { get; set; }
        public decimal Value { get; set; }
    }

    public class PlottableProperty : INotifyPropertyChanged
    {
        private string _propertyName;
        public string PropertyName
        {
            get { return _propertyName; }
            set
            {
                if (value != _propertyName)
                {
                    _propertyName = value;
                    NotifyPropertyChanged("PropertyName");
                }
            }
        }

        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (value != _selected)
                {
                    _selected = value;
                    NotifyPropertyChanged("Selected");
                }
            }
        }

        private Color _plotColor;
        public Color PlotColor
        {
            get { return _plotColor; }
            set
            {
                if (value != _plotColor)
                {
                    _plotColor = value;
                    NotifyPropertyChanged("PlotColor");
                }
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }

    public class DarkLightResults : Results
    {
        
        public decimal NetProfitOrLoss { get; set; }
        public decimal ProfitPerShare { get; set; }

        public DarkLightResults(Results results)
        {
            PlottingUtilities.CopyParameters(results, this);
            NetProfitOrLoss = GrossPL - (HundredLots*100*ComPerShare);
            ProfitPerShare = NetProfitOrLoss / SharesTraded;
        }

        public static DarkLightResults AddResults(DarkLightResults r1, DarkLightResults r2)
        {
            Results results = new Results();
            DarkLightResults dLResults = new DarkLightResults(results);

            dLResults.BuyLosers = r1.BuyLosers + r2.BuyLosers;
            dLResults.BuyPL = r1.BuyPL + r2.BuyPL;
            dLResults.BuyWins = r1.BuyWins + r2.BuyWins;
            //dLResults.Commissions = r1.Commissions    + r2.Commissions;
            dLResults.ComPerShare = r1.ComPerShare + r2.ComPerShare;
            dLResults.ConsecLose = r1.ConsecLose + r2.ConsecLose;
            dLResults.ConsecWin = r1.ConsecWin + r2.ConsecWin;
            dLResults.DaysTraded = r1.DaysTraded + r2.DaysTraded;
            dLResults.Flats = r1.Flats + r2.Flats;
            dLResults.GrossPerDay = r1.GrossPerDay + r2.GrossPerDay;
            dLResults.GrossPerSymbol = r1.GrossPerSymbol + r2.GrossPerSymbol;
            dLResults.GrossPL = r1.GrossPL + r2.GrossPL;
            dLResults.Losers = r1.Losers + r2.Losers;
            dLResults.MaxLoss = Math.Max(r1.MaxLoss, r2.MaxLoss);
            dLResults.MaxOpenLoss = Math.Max(r1.MaxOpenLoss, r2.MaxOpenLoss);
            dLResults.MaxOpenWin = Math.Max(r1.MaxOpenWin, r2.MaxOpenWin);
            dLResults.MaxPL = Math.Max(r1.MaxPL, r2.MaxPL);
            dLResults.MaxWin = Math.Max(r1.MaxWin, r2.MaxWin);
            dLResults.MinPL = Math.Min(r1.MinPL, r2.MinPL);
            //dLResults.NetPL = r1.NetPL + r2.NetPL;
            dLResults.NetProfitOrLoss = r1.NetProfitOrLoss + r2.NetProfitOrLoss;
            dLResults.ProfitPerShare = r1.ProfitPerShare + r2.ProfitPerShare;
            dLResults.RoundLosers = r1.RoundLosers + r2.RoundLosers;
            dLResults.RoundTurns = r1.RoundTurns + r2.RoundTurns;
            dLResults.RoundWinners = r1.RoundWinners + r2.RoundWinners;
            dLResults.SellLosers = r1.SellLosers + r2.SellLosers;
            dLResults.SellPL = r1.SellPL + r2.SellPL;
            dLResults.SellWins = r1.SellWins + r2.SellWins;
            dLResults.SharesTraded = r1.SharesTraded + r2.SharesTraded;
            dLResults.SymbolCount = r1.SymbolCount + r2.SymbolCount;
            dLResults.Trades = r1.Trades + r2.Trades;
            dLResults.Winners = r1.Winners + r2.Winners;
            
            return dLResults;
        }

        public static List<Descriptive> GetResultStatistics(List<DarkLightResults> results)
        {
            List<Descriptive> DescriptiveStats = new List<Descriptive>();
            ArrayList consecWinners = new ArrayList();
            ArrayList consecLosers = new ArrayList();
            ArrayList loserSum = new ArrayList();
            ArrayList winnerSum = new ArrayList();
            ArrayList grossPLSum = new ArrayList();
            
            //get summations
            foreach (var result in results)
            {
                consecWinners.Add(result.ConsecWin);
                consecLosers.Add(result.ConsecLose);
                if (result.GrossPL < 0) loserSum.Add(result.GrossPL);
                if (result.GrossPL > 0) winnerSum.Add(result.GrossPL);
                grossPLSum.Add(result.GrossPL);
            }
            //get descriptive stats
            double[] consecWins = consecWinners.ToArray(typeof(double)) as double[];
            Descriptive consecWinsStats = new Descriptive(consecWins);
            consecWinsStats.Analyze();
            DescriptiveStats.Add(consecWinsStats);

            double[] consecLoss = consecWinners.ToArray(typeof(double)) as double[];
            Descriptive consecLossStats = new Descriptive(consecLoss);
            consecLossStats.Analyze();
            DescriptiveStats.Add(consecLossStats);

            double[] grossPL = consecWinners.ToArray(typeof(double)) as double[];
            Descriptive GrossPLStats = new Descriptive(grossPL);
            GrossPLStats.Analyze();
            DescriptiveStats.Add(GrossPLStats);

            double[] lossPL = consecWinners.ToArray(typeof(double)) as double[];
            Descriptive LoserStats = new Descriptive(lossPL);
            LoserStats.Analyze();
            DescriptiveStats.Add(LoserStats);

            double[] winPL = consecWinners.ToArray(typeof(double)) as double[];
            Descriptive WinnerStats = new Descriptive(winPL);
            WinnerStats.Analyze();
            DescriptiveStats.Add(WinnerStats);

            return DescriptiveStats;
        }

    }
}