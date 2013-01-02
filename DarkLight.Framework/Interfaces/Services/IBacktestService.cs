using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using DarkLight.Framework.Data.Common;
using com.espertech.esper.client;


namespace DarkLight.Framework.Interfaces.Services
{
    public interface IBacktestService : IDarkLightService
    {
        //string RunBackTestString(IHistDataService _histDataService, DarkLightResponse _response);
        void RunBackTest(IHistDataService _histDataService, DarkLightResponse _response);
        void PauseBackTest(string key);
        void ResumeBackTest(string key);
        void CancelBackTest(string key);        
        ResponseSessionRecord GetBackTest(string backtestID);        
    }
}
