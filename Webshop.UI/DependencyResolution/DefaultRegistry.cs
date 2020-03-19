// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using E_commerce.Data;
using StructureMap;
using Webshop.UI.Controllers;

namespace Webshop.UI.DependencyResolution
{
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;

    public class DefaultRegistry : Registry
    {
        #region Constructors and Destructors

        public DefaultRegistry()
        {
            var profiles = from t in typeof(DefaultRegistry).Assembly.GetTypes()
                where typeof(Profile).IsAssignableFrom(t)
                select Activator.CreateInstance(t) as Profile;

            //For each Profile, include that profile in the MapperConfiguration
            var config = new MapperConfiguration(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });

            //Create a mapper that will be used by the DI container
            var mapper = config.CreateMapper();

            //Register the DI interfaces with their implementation
            For<MapperConfiguration>().Use(config);
            For<IMapper>().Use(mapper);


            Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                    scan.With(new ControllerConvention());
                });
            For<WebshopContext>().Use<WebshopContext>().Transient();
            For<CategoryRepository>().Use<CategoryRepository>().Transient();
            For<ProductRepository>().Use<ProductRepository>().Transient();
        }

        #endregion
    }
}