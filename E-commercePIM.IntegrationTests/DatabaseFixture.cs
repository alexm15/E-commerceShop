using System;
using System.Data.Entity.Migrations;
using Xunit;

namespace E_commercePIM.IntegrationTests
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
            var configuration = new E_commerce.Data.Migrations.Configuration();
            var migrator = new DbMigrator(configuration);
            migrator.Update();
        }

        public void Seed()
        {

        }


        public void Dispose()
        {
        }
    }
}