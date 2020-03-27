using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using AutoMapper;
using E_commerce.Data;
using E_commerce.Data.Migrations;
using E_commerce.Library;
using E_commercePIM.Mapping;
using Newtonsoft.Json;
using Xunit;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;


namespace E_commercePIM.IntegrationTests
{
    public class IntegrationTestBase : IDisposable
    {
        protected WebshopContext _context;
        protected IMapper _mapper;

        public IntegrationTestBase()
        {
            _context = new WebshopContext();
            _context.Database.CreateIfNotExists();
            Seed();

            var config = new MapperConfiguration(opts => { opts.AddProfile(new ViewModelsProfile()); });
            _mapper = config.CreateMapper();

            //var configuration = new E_commerce.Data.Migrations.Configuration();
            //var migrator = new DbMigrator(configuration);
            //migrator.Update();
        }

        public void Seed()
        {
            new Configuration(_context);
        }

        private static List<T> LoadFromJson<T>(string jsonFileName)
        {
            return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText($@"..\..\{jsonFileName}"));
        }


        public void Dispose()
        {
            _context.Database.Delete();
            _context.Dispose();
        }
    }
}