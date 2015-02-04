**This software is in early alpha.**

# Blink

A library for resetting Entity Framework databases as fast as possible, for integration testing. Available on NuGet at [https://www.nuget.org/packages/Blink/](https://www.nuget.org/packages/Blink/)

## Introduction

When performing automated testing, it can be very expensive to initialize a fresh, real database. So expensive that you avoid testing against the real database at all costs. For example, the project that inspired me to start this library takes about a minute to build its database; that's fine in a deployment scenario, but intolerable if you want to write tens or hundreds of integration tests. Blink re-initialises the DB in milliseconds, which puts me in the position of being able to do TDD with lots of full-stack, end-to-end database-rich tests. 

This package helps you keep database initialization as fast as possible, to make it more feasible to perform database tests and give you more confidence about the operation of your database.

Here's how to use it...

## Usage

        // Create a new BlinkDBFactory, maybe inside [TestInitialize] or [SetUp]
        var factory = Blink.BlinkDB.CreateDbFactory<TestDbContext, TestDbConfiguration>(
            BlinkDBCreationMode.UseDBIfItAlreadyExists,
                () => new TestDbContext());
    
        // execute code, inside a transaction, with a fresh DB;
        await factory.ExecuteDbCode(async context =>
        {
            // use the context here; add, delete, etc.
        }, async context =>
        {
            // add as many extra steps as you like,
            // each one a new DbContext based on the same
            // underlying SQL DB, all within the same transaction
        });
        
       // db edits are rolled back automatically for all the methods passed above

What does it do?
-----

The first time it's called, it creates a database from scratch, applying all migrations and running the seed method, so that you have a fresh Entity Framework database, just as if you'd deleted the DB and called 'update-database' in the Package Manager console. 

Subsequent times, it searches a cache for a backup, and restores the backup instead of rebuilding the database. This means that your tests should now work much faster.

## Modes

Blink comes with two modes;

**UseDBIfItAlreadyExists** will re-use an existing database, if it already exists. Right now, this means that you are expected to (a) not change your database structure, and (b) not make any changes to the database outside Blink's transactions. If you do, your DB will be out of date with the tests and things get unpredictable.

**RecreateEveryTest** drops the database and restores from backup every time. This avoids long sequences which apply migrations and run seed methods,

## Gotchas

**Threading.** The threading is pretty brutal. It's up to you to make sure that no tests are run simulateously. This is because thre is only one database, and since it's a global resource, it can't be shared between tests. We track whether a test is already running, and reject any attempt to run a second concurrently. It's therefore up to you to work out, in your test framework, how you'd like to lock these things off so that they are executed concurrently. 

**Staleness.** While the library will keep your database initializations fast, it does this by avoiding all the expensive tests that EF to make sure the database hasn't gone stale. So several things might cause the caching to go wrong;

1) You change the content of the `Configuration.Seed()` method. If you seed different data, Blink doesn't know about it, and it's internal cache will be stale.

2) You add a migration, or other changes to the domain. Blink won't know about it and will continue to use your old data, incorrectly.

To fix this, it's necessary to wipe Blink's cache of database backups. Right now, they are stored in the hard-coded backup directory of 64-bit SQL 2012;

    C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\Backup

Get source, and edit BlinkDB.cs, to alter this backup location.

Delete backups that start 'BlinkDB_' to reset the cache and get your testing back on track.
