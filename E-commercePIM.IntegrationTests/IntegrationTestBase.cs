using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
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
        protected static WebshopContext _context;
        protected IMapper _mapper;
        private const string CONNECTION_STRING = @"data source=(localdb)\MSSQLLocalDB;initial catalog=E-comWebshop-IntegrationTests;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";

        private static string FILENAME => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "E-comWebshop-IntegrationTests.mdf");
        private static SqlConnectionStringBuilder Master =>
            new SqlConnectionStringBuilder
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                InitialCatalog = "master",
                IntegratedSecurity = true
            };

        public IntegrationTestBase()
        {
            DestroyDatabase();
            _context = new WebshopContext();
            CreateDatabase();


            var config = new MapperConfiguration(opts => { opts.AddProfile(new ViewModelsProfile()); });
            _mapper = config.CreateMapper();
        }

        private static void CreateDatabase()
        {
            ExecuteSqlCommand(Master, $@"
                CREATE DATABASE [E-comWebshop-IntegrationTests]
                ON (NAME = 'E-comWebshop-IntegrationTests',
                FILENAME = '{FILENAME}')");

            var migration = new MigrateDatabaseToLatestVersion<
                WebshopContext, Configuration>();
            migration.InitializeDatabase(_context);
        }

        private static void DestroyDatabase()
        {
            var fileNames = ExecuteSqlQuery(Master, @"
                SELECT [physical_name] FROM [sys].[master_files]
                WHERE [database_id] = DB_ID('E-comWebshop-IntegrationTests')",
                row => (string)row["physical_name"]);

            if (fileNames.Any())
            {
                ExecuteSqlCommand(Master, @"
                    ALTER DATABASE [E-comWebshop-IntegrationTests] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    EXEC sp_detach_db 'E-comWebshop-IntegrationTests'");

                fileNames.ForEach(File.Delete);
            }
        }


        private static List<T> LoadFromJson<T>(string jsonFileName)
        {
            return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText($@"..\..\{jsonFileName}"));
        }

        private static void ExecuteSqlCommand(
            SqlConnectionStringBuilder connectionStringBuilder,
            string commandText)
        {
            using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }
            }
        }

        private static List<T> ExecuteSqlQuery<T>(
            SqlConnectionStringBuilder connectionStringBuilder,
            string queryText,
            Func<SqlDataReader, T> read)
        {
            var result = new List<T>();
            using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = queryText;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(read(reader));
                        }
                    }
                }
            }
            return result;
        }


        public void Dispose()
        {
            DestroyDatabase();
        }
    }
}