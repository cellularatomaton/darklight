using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using TradeLink.API;
using TradeLink.AppKit;

namespace DarkLight.Analytics.Models
{
    public class ResultsModel
    {
        DataTable _resultsTable = new DataTable("results");
        
        List<List<TradeResult>> _resultlists = new List<List<TradeResult>>();
        decimal _rfr = .01m;
        decimal _comm = .01m;
        delegate void newresulttradesdel(string name, List<Trade> trades);

        public const string RESULTS_POSTFIX = "Trades.csv";
        public event DebugDelegate SendDebug;
        public Results CurrentResults = new Results();

        public ResultsModel()
        {
            _resultsTable.Columns.Add("Stat");
            _resultsTable.Columns.Add("Result");
        }

        public void Clear()
        {
            _resultsTable.Clear();
            _resultlists.Clear();
        }

        public DataTable RunResults(string name, List<Trade> trades)
        {
            name = System.IO.Path.GetFileNameWithoutExtension(name);
            List<TradeResult> newResults;
            if (trades.Count == 0)
            {
                debug("No results found for: " + name);
                newResults = new List<TradeResult>();
            }
            else
            {
                newResults = TradeResult.ResultsFromTradeList(trades);
            }
            FillDataTableWithResults(Results.FetchResults(newResults, _rfr, _comm, debug));
            return _resultsTable;
        }

        void debug(string msg)
        {
            if (SendDebug != null)
                SendDebug(msg);
        }

        void FillDataTableWithResults(Results r)
        {
            CurrentResults = r;
            _resultsTable.BeginLoadData();
            _resultsTable.Clear();
            Type t = r.GetType();
            FieldInfo[] fis = t.GetFields();
            foreach (FieldInfo fi in fis)
            {
                string format = null;
                if (fi.FieldType == typeof(Decimal)) format = "{0:N2}";
                _resultsTable.Rows.Add(fi.Name, (format != null) ? string.Format(format, fi.GetValue(r)) : fi.GetValue(r).ToString());
            }
            PropertyInfo[] pis = t.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                string format = null;
                if (pi.PropertyType == typeof(Decimal)) format = "{0:N2}";
                _resultsTable.Rows.Add(pi.Name, (format != null) ? string.Format(format, pi.GetValue(r, null)) : pi.GetValue(r, null).ToString());
            }
            //foreach (string ps in r.PerSymbolStats)
            //{
            //    string[] rs= ps.Split(':');
            //    if (rs.Length != 2) continue;
            //    _resultsTable.Rows.Add(rs[0], rs[1]);
            //}
            _resultsTable.EndLoadData();
        }
    }
}
