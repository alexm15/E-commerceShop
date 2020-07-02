using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Moq;

namespace E_commercePIM.Tests.Helpers
{
    public static class TestHelper
    {
        public static DbSet<T> MockDbSet<T>() where T : class
        {
            return MockDbSet<T>(null);
        }

        public static DbSet<T> MockDbSet<T>(IList<T> input) where T : class
        {
            if (input == null) input = new List<T>();

            var mockSet = new Mock<DbSet<T>>();

            var data = input.AsQueryable();

            //Async Operations needs to be configured before normal, else they won't work
            mockSet.As<IDbAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(data.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(data.Provider));

            //Regular operations
            mockSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(input.Add);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mockSet.Setup(x => x.AsNoTracking()).Returns(mockSet.Object);
            mockSet.Setup(x => x.Include(It.IsAny<string>())).Returns(mockSet.Object);

            

            return mockSet.Object;
        }
    }
}