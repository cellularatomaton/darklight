using System;
using System.Collections.Generic;
using System.Text;
using DarkLight.Framework.Data.Common;
using DarkLight.Framework.Interfaces.Services;

namespace DarkLight.Framework.Interfaces.Adapters
{
    public interface IBacktestAdapter : IAdapter
    {
        //Func<IHistDataService, DarkLightResponse, string> OnRunBacktestString { get; set; }
        Action<IHistDataService, DarkLightResponse> OnRunBacktest { get; set; }
        Action<string> OnPauseBacktest { get; set; }
        Action<string> OnResumeBacktest { get; set; }
        Action<string> OnCancelBacktest { get; set; }
    }
}
