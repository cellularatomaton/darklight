using Caliburn.Micro.Autofac;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Caliburn.Micro;
using Autofac;

namespace DarkLight
{
    #region Caliburn.Micro Bootstrapper

    public class AppBootstrapper : Bootstrapper<ShellViewModel> { }

    #endregion

    #region Autofac Bootstrapper

    //public class AppBootstrapper : AutofacBootstrapper<ShellViewModel>
    //{
    //    protected override void ConfigureBootstrapper()
    //    {  //  you must call the base version first!
    //        base.ConfigureBootstrapper();
    //        //  override namespace naming convention
    //        EnforceNamespaceConvention = false;
    //        //  change our view model base type
    //        ViewModelBaseType = typeof(IShell);
    //    }

    //    protected override void ConfigureContainer(ContainerBuilder builder)
    //    {
    //        base.ConfigureContainer(builder);

    //        builder.Register(c => new TestViewModel());
    //    }
    //}

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
