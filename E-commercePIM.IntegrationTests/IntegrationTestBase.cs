using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using AutoMapper;
using Dapper;
using E_commerce.Data;
using E_commerce.Data.Migrations;
using E_commercePIM.Mapping;
using Newtonsoft.Json;


namespace E_commercePIM.IntegrationTests
{
    public class IntegrationTestBase : IDisposable
    {
        protected WebshopContext _context;
        protected IMapper _mapper;
        private const string CONNECTION_STRING = @"data source=(localdb)\MSSQLLocalDB;initial catalog=E-comWebshop-IntegrationTests;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";

        public IntegrationTestBase()
        {
            _context = new WebshopContext();
            ClearDatabase();
            var migration = new MigrateDatabaseToLatestVersion<WebshopContext, Configuration>();
            migration.InitializeDatabase(_context);


            var config = new MapperConfiguration(opts => { opts.AddProfile(new ViewModelsProfile()); });
            _mapper = config.CreateMapper();

            
        }

        private void ClearDatabase()
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                string clearConnectionsCommand = @"ALTER DATABASE [E-ComWebshop-IntegrationTest] 
                                                   SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                var affectedRows = connection.Execute(clearConnectionsCommand);
            }
            _context.Database.Delete();

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