using System;
using System.Data.Entity;
using System.Transactions;
using E_commerce.Data;

namespace E_commercePIM.IntegrationTests
{
    public class IntegrationTestBase : IDisposable
    {
        private readonly WebshopContext _context;
        protected TransactionScope _scope;
        private DbContextTransaction _transaction;

        public IntegrationTestBase(WebshopContext context)
        {
            _context = context;
            _transaction = _context.Database.BeginTransaction();
        }

        public void Dispose()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;
        }
    }
}