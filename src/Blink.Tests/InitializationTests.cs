using System;
using System.Linq;
using Blink.Tests.TestDb;
using Blink.Tests.TestDb.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blink.Tests
{
    [TestClass]
    public class InitializationTests
    {
        [TestMethod]
        public void ShouldInitializeANewContext()
        {
            // Create a new BlinkDBFactory;
            var factory = Blink.BlinkDB.CreateDbFactory<TestDbContext, TestDbConfiguration>(
                BlinkDBCreationMode.UseDBIfItAlreadyExists,
                () => new TestDbContext());

            bool called = false;

            for (var i = 0; i < 5; i++)
            {
                // execute code, inside a transaction, with a fresh DB every time;
                factory.ExecuteDbCode(context =>
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
        public void ShouldAllowMultipleSequentialSessions()
        {
            // Create a new BlinkDBFactory;
            var factory = Blink.BlinkDB.CreateDbFactory<TestDbContext, TestDbConfiguration>(
                BlinkDBCreationMode.UseDBIfItAlreadyExists,
                () => new TestDbContext());

            bool called = false;

            // execute code, inside a transaction, with a fresh DB every time;
            factory.ExecuteDbCode(context =>
            {
                // use the context here;
                Assert.IsNotNull(context);

                context.TestObjects.Add(new TestObject { Name = "quux" });
                context.SaveChanges();
                Assert.AreEqual(4, context.TestObjects.Count(), "doesn't look transactional!");

                called = true;
            }, context =>
            {
                // make sure that the data persists into this next step;
                Assert.AreEqual(4, context.TestObjects.Count(), "doesn't look transactional!");
            });

            Assert.IsTrue(called);

            factory.ExecuteDbCode(context =>
            {
                // make sure that the data cidn't persist beyond the step sequence above;
                Assert.AreEqual(3, context.TestObjects.Count(), "doesn't look transactional!");
            });
        }


    }
}
