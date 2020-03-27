using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using E_commerce.Data;
using E_commerce.Data.Migrations;
using E_commerce.Library;
using Newtonsoft.Json;
using Xunit;

namespace E_commercePIM.IntegrationTests
{
    public class DatabaseFixture : IDisposable
    {
        protected WebshopContext _context;

        public DatabaseFixture()
        {
            _context = new WebshopContext();
            _context.Database.CreateIfNotExists();

            Seed();

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