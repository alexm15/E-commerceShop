using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Webshop.UI.App_Data;

namespace Webshop.UI
{
    public class ContainerConfig
    {
        public static void RegisterContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.RegisterType(typeof(WebshopContext)).InstancePerLifetimeScope();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}