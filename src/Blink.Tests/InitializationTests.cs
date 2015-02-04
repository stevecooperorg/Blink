using System;
using System.Linq;
using System.Threading.Tasks;
using Blink.Tests.TestDb;
using Blink.Tests.TestDb.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blink.Tests
{
    [TestClass]
    public class InitializationTests
    {
        [TestMethod]
        public async Task ShouldInitializeANewContext()
        {
            // Create a new BlinkDBFactory;
            var factory = Blink.BlinkDB.CreateDbFactory<TestDbContext, TestDbConfiguration>(
                BlinkDBCreationMode.UseDBIfItAlreadyExists,
                () => new TestDbContext());

            bool called = false;

            for (var i = 0; i < 5; i++)
            {
                // execute code, inside a transaction, with a fresh DB every time;
                await factory.ExecuteDbCode(async context =>
                {
                    // use the context here;
                    Assert.IsNotNull(context);

                    context.TestObjects.Add(new TestObject { Name = "quux" });
                    context.SaveChanges();
                    Assert.AreEqual(4, context.TestObjects.Count(), "doesn't look transactional!");

                    called = true;
                });
            }

            Assert.IsTrue(called);
        }

        [TestMethod]
        public async Task ShouldAllowMultipleSequentialSessions()
        {
            // Create a new BlinkDBFactory;
            var factory = Blink.BlinkDB.CreateDbFactory<TestDbContext, TestDbConfiguration>(
                BlinkDBCreationMode.UseDBIfItAlreadyExists,
                () => new TestDbContext());

            bool called = false;

            // execute code, inside a transaction, with a fresh DB every time;
            await factory.ExecuteDbCode(async context =>
            {
                Assert.AreEqual(3, context.TestObjects.Count(), "should start with three");

                context.TestObjects.Add(new TestObject { Name = "quux" });
                context.SaveChanges();
                Assert.AreEqual(4, context.TestObjects.Count(), "didn't save as expected");

                called = true;
            }, async context =>
            {
                // make sure that the data persists into this next step;
                Assert.AreEqual(4, context.TestObjects.Count(), "content was not persisted in chain!");
            });

            Assert.IsTrue(called);

            await factory.ExecuteDbCode(async context =>
            {
                // make sure that the data cidn't persist beyond the step sequence above;
                Assert.AreEqual(3, context.TestObjects.Count(), "doesn't look transactional - we should have rolled back to three!");
            });
        }


    }
}
