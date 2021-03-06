﻿using System.Windows.Controls;
using Caliburn.Micro.Autofac;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Caliburn.Micro;
using Autofac;
using DarkLight.Client;
using DarkLight.Client.Backtest.ViewModels;
using DarkLight.Client.Common.ViewModels;
using DarkLight.Client.Customizations;
using DarkLight.Client.LiveTrading.ViewModels;
using DarkLight.Client.Utilities;
using DarkLight.Client.WPFServices;
using DarkLight.Framework.Interfaces.Adapters;
using DarkLight.Framework.Interfaces.CEP;
using DarkLight.Framework.Interfaces.Common;
using DarkLight.Framework.Interfaces.Repository;
using DarkLight.Framework.Interfaces.Services;
using DarkLight.Infrastructure;
using DarkLight.Infrastructure.EventBroker;
using DarkLight.Infrastructure.Mediator;
using DarkLight.Infrastructure.ServiceBus;
using DarkLight.Client.LiveTrading.ViewModels;
using DarkLight.Client.Optimization.ViewModels;
using DarkLight.Framework.Utilities;

namespace DarkLight.Client
{
    #region Caliburn.Micro Bootstrapper

    //public class AppBootstrapper : Bootstrapper<ShellViewModel> { }

    #endregion

    #region Autofac Bootstrapper

    public class AppBootstrapper : AutofacBootstrapper<ShellViewModel>
    {
        static AppBootstrapper()
        {
            LogManager.GetLog = type => new NLogLogger(type);
        }

        protected override void ConfigureBootstrapper()
        {            
            //  you must call the base version first!            
            base.ConfigureBootstrapper();
            //  override namespace naming convention
            EnforceNamespaceConvention = false;
            //  change our view model base type
            ViewModelBaseType = typeof(IShell);
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            // Register Types:
            builder.RegisterType<DefaultColorService>().As<IColorService>();
            builder.RegisterType<DefaultFilterService>().As<IFilterService>();
            builder.RegisterType<DefaultViewModelService>().As<IViewModelService>();
            builder.RegisterType<DarkLightWindowManager>().As<IWindowManager>();
            builder.RegisterType<MockBacktestRepository>().As<IBacktestRepository>();
            builder.RegisterType<MockBacktestService>().As<IBacktestService>().SingleInstance();
            builder.RegisterType<MockHistDataService>().As<IHistDataService>();
            builder.RegisterType<EventBrokerLocal>().As<IEventBroker>().SingleInstance();
            builder.RegisterType<MockBacktestService>().As<IBacktestService>().SingleInstance();
            //builder.RegisterType<Mediator>().As<IMediator>().SingleInstance();
            //builder.RegisterType<ServiceBusLocal>().As<IMediatorAdapter,IBacktestAdapter>().SingleInstance();
            //builder.RegisterType<MediatorCEP>().As<IMediator>().SingleInstance();
            //builder.RegisterType<ServiceBusLocalCEP>().As<IMediatorAdapter, IBacktestAdapter>().SingleInstance();
            
            // Register Service Implementations:
            builder.Register(c => new DefaultColorService()).SingleInstance();
            builder.Register(c => new DefaultFilterService(IoC.Get<IColorService>())).SingleInstance();
            builder.Register(c => new DarkLightWindowManager()).SingleInstance();
            builder.Register(c => new EventBrokerLocal()).SingleInstance();
            builder.Register(c => new MockBacktestService(IoC.Get<IEventBroker>())).SingleInstance();
            //builder.Register(c => new Mediator(IoC.Get<IMediatorAdapter>(), IoC.Get<IEventAggregator>())).SingleInstance();            
            
            // Register Modules:
            builder.Register(c => new LinkableViewModel(IoC.Get<IColorService>(), IoC.Get<IViewModelService>()));
            builder.Register(c => new BacktestModuleViewModel()).SingleInstance();
            builder.Register(c => new BacktestLauncherViewModel(IoC.Get<IViewModelService>()));
            builder.Register(c => new BacktestBrowserViewModel(IoC.Get<IColorService>(), IoC.Get<IViewModelService>()));
            builder.Register(c => new OptimizationModuleViewModel()).SingleInstance();
            builder.Register(c => new OptimizationSchedulerViewModel()).SingleInstance();
            builder.Register(c => new LiveTradingModuleViewModel()).SingleInstance();
            builder.Register(c => new LiveTradingPortfoliosViewModel()).SingleInstance();
            builder.Register(c => new EventPublisherViewModel(IoC.Get<IColorService>())).SingleInstance();

            // Register View Models:
            builder.Register(c => new StatisticsViewModel());
            builder.Register(c => new ResultsViewModel());
            builder.Register(c => new FillsViewModel());
            builder.Register(c => new IndicatorsViewModel());
            builder.Register(c => new MessagesViewModel());
            builder.Register(c => new TickDataViewModel());
            builder.Register(c => new OrdersViewModel());
            builder.Register(c => new PositionsViewModel());
            builder.Register(c => new TimeseriesViewModel());
            builder.Register(c => new ResponseSelectionViewModel());
            builder.Register(c => new ParametricRangeViewModel());
            builder.Register(c => new TemporalRangeViewModel());
            builder.Register(c => new BacktestStatusViewModel());
            builder.Register(c => new ErrorViewModel());
            builder.Register(c => new DefaultViewModel());

            //IoC.Get<IBacktestService>().Initialize();

            // Context Menu:
            //http://compiledexperience.com/blog/posts/wp7-context-menus-with-caliburn-micro            
            ConventionManager.AddElementConvention<MenuItem>(ItemsControl.ItemsSourceProperty, "DataContext", "Click");

        }
    }

    #endregion

    #region MEF Bootstrapper

    //public class AppBootstrapper : Bootstrapper<IShell>
    //{
    //    CompositionContainer container;

    //    /// <summary>
    //    /// By default, we are configured to use MEF
    //    /// </summary>
    //    protected override void Configure() {
    //        var catalog = new AggregateCatalog(
    //            AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()
    //            );

    //        container = new CompositionContainer(catalog);

    //        var batch = new CompositionBatch();

    //        batch.AddExportedValue<IWindowManager>(new WindowManager());
    //        batch.AddExportedValue<IEventAggregator>(new EventAggregator());
    //        batch.AddExportedValue(container);
    //        batch.AddExportedValue(catalog);

    //        container.Compose(batch);
    //    }

    //    protected override object GetInstance(Type serviceType, string key)
    //    {
    //        string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
    //        var exports = container.GetExportedValues<object>(contract);

    //        if (exports.Count() > 0)
    //            return exports.First();

    //        throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
    //    }

    //    protected override IEnumerable<object> GetAllInstances(Type serviceType)
    //    {
    //        return container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
    //    }

    //    protected override void BuildUp(object instance)
    //    {
    //        container.SatisfyImportsOnce(instance);
    //    }
    //}

    #endregion
}
