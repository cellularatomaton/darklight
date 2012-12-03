using System;
using System.Collections.Generic;
using System.Text;
using DarkLight.Customizations;
using DarkLight.Infrastructure.Adapters;
using DarkLight.Services;

namespace DarkLight.Infrastructure
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
