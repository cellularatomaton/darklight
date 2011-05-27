using System;
using System.Configuration;
using System.Net;
using System.ServiceModel;
using System.Windows.Forms;
using DarkLight.Common.Config;
using DarkLight.Common.Engines;
using DarkLight.Common.Interfaces;
using DarkLight.Common.Services;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Collections.Generic;

namespace DarkLight.Server
{
    public partial class DarkLightServerForm : Form, IDisposable
    {
        private Configuration config;

        // For GUI communication
        private ServiceHost strategyManagerService;

        public DarkLightServerForm()
        {
            InitializeComponent();
            InitializeStrategyDependencies();
        }

        private void InitializeStrategyDependencies()
        {
            var map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = "DarkLight.config";
            config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
        }

        private iManageable CreateStrategy(StrategyInstanceElement stratElement, ProductManagerConfigSection prodSection, UnityConfigurationSection unitySection)
        {
            var strategyInfo = new StrategyInfo
            {
                Status = StrategyStatus.STOPPED,
                StrategyName = stratElement.Name
            };
            //var allProductInformation = BM_DatabaseClient.AssembleTheListOfProducts(stratConfig.StrategyID, sConnection, consumerManager, exceptionLog);
            var prodGroup = prodSection.ProductGroups[stratElement.ProductGroup];
            var tickers = new List<string>();
            for (int i = 0; i < prodGroup.Products.Count; i++)
            {
                var prod = prodGroup.Products[i];
                tickers.Add(prod.Ticker);
            }
            
            var container = new UnityContainer();
            container.RegisterInstance(strategyInfo);
            
            unitySection.Configure(container, stratElement.Binding);
            var strat = container.Resolve<iManageable>();
            strat.InitializeDependencies(tickers);
            return strat;
        }

        #region Implementation of IDisposable

        new public void Dispose()
        {
            if (strategyManagerService != null)
            {
                strategyManagerService.Close();
            }
            base.Dispose();
        }

        #endregion

        private void btnCreateManagerService_Click(object sender, EventArgs e)
        {
            if(strategyManagerService == null)
                strategyManagerService = new ServiceHost(typeof(StrategyManagementService));
            if(strategyManagerService.State != CommunicationState.Opened ||
                strategyManagerService.State != CommunicationState.Opening)
                strategyManagerService.Open();
        }

        private void btnLoadStrategies_Click(object sender, EventArgs e)
        {
            var unitySection = SectionHandler<UnityConfigurationSection>.GetSection("unity", config);
            var stratSection = SectionHandler<StrategyManagerConfigSection>.GetSection("DarkLight/StrategyManager", config);
            var prodSection = SectionHandler<ProductManagerConfigSection>.GetSection("DarkLight/ProductManager", config);
            var stratInstances = stratSection.Strategies;
            foreach (StrategyInstanceElement stratElement in stratInstances)
            {
                StrategyManagementEngine.Instance.AddStrategy(CreateStrategy(stratElement, prodSection, unitySection));
            }
        }
    }
}
