﻿using System;
using Blink.Tests.TestDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blink.Tests
{
    [TestClass]
    public class InitializationTests
    {
        [TestMethod]
        public void ShouldInitializeANewContext()
        {
            // use Blink to create a new database;
            var factory = Blink.BlinkDb.CreateDbFactory<TestDbContext, TestDbConfiguration>(() => new TestDbContext());

            bool called = false;

            factory.ExecuteDbCode(context =>
            {
                // use the context here;
                Assert.IsNotNull(context);

                called = true;
            });

            Assert.IsTrue(called);
        }
        [TestMethod]
        public void ShouldInitializeANewContext2()
        {
            ShouldInitializeANewContext();
        }
        [TestMethod]
        public void ShouldInitializeANewContext3()
        {
            ShouldInitializeANewContext();
        }
        [TestMethod]
        public void ShouldInitializeANewContext4()
        {
            ShouldInitializeANewContext();
        }
    }
}