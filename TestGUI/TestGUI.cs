using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows.Forms;
using DarkLight.Common.Services;

namespace TestGUI
{
    public partial class TestGUI : Form
    {
        private List<StrategyInfo> infos;

        public TestGUI()
        {
            InitializeComponent();

            
        }

        private void btnGetStrategies_Click(object sender, EventArgs e)
        {
            using (var factory = new ChannelFactory<iStrategyManagementService>("StrategyManagementClient"))
            {
                var proxy = factory.CreateChannel();
                infos = new List<StrategyInfo>(proxy.GetAllStrategyInfoForServer());
                lblNumStrats.Text = "Number Strategies: " + infos.Count();
            }
        }

        private void btnStartStrategy_Click(object sender, EventArgs e)
        {
            if (infos != null)
            {
                using (var factory = new ChannelFactory<iStrategyManagementService>("StrategyManagementClient"))
                {
                    var proxy = factory.CreateChannel();
                    foreach (var strategyInfo in infos)
                    {
                        proxy.StartStrategy(strategyInfo);
                    }
                }
            }
        }

        private void btnStopStrategy_Click(object sender, EventArgs e)
        {
            if (infos != null)
            {
                using (var factory = new ChannelFactory<iStrategyManagementService>("StrategyManagementClient"))
                {
                    var proxy = factory.CreateChannel();
                    foreach (var strategyInfo in infos)
                    {
                        proxy.StopStrategy(strategyInfo);
                    }
                }
            }
        }
    }
}
