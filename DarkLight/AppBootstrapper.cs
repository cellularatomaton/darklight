﻿using Caliburn.Micro.Autofac;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Caliburn.Micro;
using Autofac;
using DarkLight.Backtest.ViewModels;
using DarkLight.Common.ViewModels;
using DarkLight.LiveTrading.ViewModels;
using DarkLight.Optimization.ViewModels;
using DarkLight.Services;

namespace DarkLight
{
    #region Caliburn.Micro Bootstrapper

    //public class AppBootstrapper : Bootstrapper<ShellViewModel> { }

    #endregion

    #region Autofac Bootstrapper

    public class AppBootstrapper : AutofacBootstrapper<ShellViewModel>
    {
        protected override void ConfigureBootstrapper()
        {  //  you must call the base version first!
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

            // Register Service Implementations:
            builder.Register(c => new DefaultColorService()).SingleInstance();
            builder.Register(c => new DefaultFilterService(IoC.Get<IColorService>())).SingleInstance();

            // Register Modules:
            builder.Register(c => new LinkableViewModel(IoC.Get<IColorService>()));
            builder.Register(c => new BacktestModuleViewModel(IoC.Get<IColorService>())).SingleInstance();
            builder.Register(c => new OptimizationModuleViewModel()).SingleInstance();
            builder.Register(c => new LiveTradingModuleViewModel()).SingleInstance();

            // Register View Models:
            builder.Register(c => new TestViewModel());
            builder.Register(c => new StatisticsViewModel());
            builder.Register(c => new ResultsViewModel());
            builder.Register(c => new FillsViewModel());
            builder.Register(c => new IndicatorsViewModel());
            builder.Register(c => new MessagesViewModel());
            builder.Register(c => new TickDataViewModel());
            builder.Register(c => new OrdersViewModel());
            builder.Register(c => new PositionsViewModel());
            builder.Register(c => new TimeseriesViewModel());
            builder.Register(c => new DefaultViewModel());
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