using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using E_commerce.Data;

namespace E_commercePIM
{
    public class ContainerConfig
    {
        public static void ConfigureIOC()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.RegisterType(typeof(WebshopContext)).InstancePerLifetimeScope();
            
            builder.RegisterType(typeof(ProductRepository)).InstancePerDependency();


            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}