using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Hangfire;
using Hangfire.SqlServer;
using StructureMap;
using Hangfire.StructureMap;

[assembly: OwinStartup("HappyRE", typeof(HappyRE.App.Startup))]

namespace HappyRE.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //HttpConfiguration config = new HttpConfiguration();
            ConfigureAuth(app);
            log4net.Config.XmlConfigurator.Configure();
            app.MapSignalR();
            //WebApiConfig.Register(config);
            //app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            //app.UseWebApi(config);

            //JobStorage.Current = new SqlServerStorage(System.Configuration.ConfigurationManager.ConnectionStrings["HangfireConnection"].ConnectionString); 

            var container = new Container(c =>
            {
                c.AddRegistry<HappyRE.Core.BLL.DI.BLLRegistry>();
            });

            Hangfire.GlobalConfiguration.Configuration.UseStructureMapActivator(container);
            Hangfire.GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(System.Configuration.ConfigurationManager.ConnectionStrings["HangfireConnection"].ConnectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                });
            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new MyAuthorizationFilter() }
            });
        }
    }

    //public class ContainerJobActivator : JobActivator
    //{
    //    private IContainer _container;

    //    public ContainerJobActivator(IContainer container)
    //    {
    //        container = new Container(c =>
    //        {
    //            c.AddRegistry<HappyRE.Api.DependencyResolution.DefaultRegistry>();
    //        });
    //        _container = container;
    //    }

    //    public override object ActivateJob(Type type)
    //    {
    //        return _container.GetInstance(type);
    //    }
    //}
    public class MyAuthorizationFilter : Hangfire.Dashboard.IDashboardAuthorizationFilter
    {
        public bool Authorize(Hangfire.Dashboard.DashboardContext context)
        {
            return true;
        }
    }
}
