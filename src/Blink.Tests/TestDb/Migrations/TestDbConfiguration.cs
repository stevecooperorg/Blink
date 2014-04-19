using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blink.Tests.TestDb.Migrations
{
    public class TestDbConfiguration : DbMigrationsConfiguration<TestDbContext>
    {
        public TestDbConfiguration()
        {
        }

        protected override void Seed(TestDbContext context)
        {
            context.TestObjects.AddOrUpdate(new TestObject[] {
                new TestObject { Id=1, Name ="foo" },
                new TestObject { Id=1, Name ="bar" },
                new TestObject { Id=1, Name ="baz" },
            });

            base.Seed(context);
        }


    }
}
