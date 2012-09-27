using Caliburn.Micro;

namespace DarkLight.Backtest.ViewModels
{
    public class BacktestModuleViewModel : Screen
    {
        public BacktestModuleViewModel()
        {
            this.DisplayName = this.GetType().Name;
        }
    }
}
