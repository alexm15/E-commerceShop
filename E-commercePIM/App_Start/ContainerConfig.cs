using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
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
            builder.RegisterType(typeof(CategoryRepository)).InstancePerDependency();

            var assemblyNames = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t) && t.IsPublic && !t.IsAbstract);


            var autoMapperProfiles = assemblyNames
                .Select(p => (Profile)Activator.CreateInstance(p)).ToList();

            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                foreach (var profile in autoMapperProfiles)
                {
                    cfg.AddProfile(profile);
                }
            }));

            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>().InstancePerLifetimeScope();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}