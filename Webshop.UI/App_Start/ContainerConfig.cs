using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
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

            var assemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            var assemblyTypes = assemblyNames
                .Where(a => a.Name.Equals("Webshop.UI.Mappers", StringComparison.OrdinalIgnoreCase))
                .SelectMany(an => Assembly.Load(an).GetTypes())
                .Where(p => typeof(Profile).IsAssignableFrom(p) && p.IsPublic && !p.IsAbstract)
                .Distinct();

            var autoMapperProfiles = assemblyTypes
                .Select(p => Activator.CreateInstance(p) as Profile).ToList();

            builder.Register(ctx => new MapperConfiguration(cfg => autoMapperProfiles.ForEach(cfg.AddProfile)));

            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper())
                .As<IMapper>()
                .InstancePerLifetimeScope();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}