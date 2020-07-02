using System;
using System.Transactions;
using E_commerce.Data;

namespace E_commercePIM.IntegrationTests
{
    public class IntegrationTestBase : IDisposable
    {
        private TransactionScope _scope;
        public WebshopContext Context { get;}

        protected IntegrationTestBase()
        {
            Context = new WebshopContext(); //Create new for each test as the Context keeps a in-memory cache of DbSets and changes to them
            _scope = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);
        }

        public void Dispose()
        {
            _scope?.Dispose();
            _scope = null;
        }
    }
}