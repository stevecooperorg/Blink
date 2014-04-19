**THIS README IS A LIE! It's my way of thinking out the API I'd like to build. No code ready quite yet.**

# Blink


A library for resetting Entity Framework databases as fast as possible, for integration testing.

## Introduction

When performing automated testing, it can be very expensive to initialize a fresh, real database. So expensive that you avoud testing against the real database at all costs. The project that inspired me to start this library takes about a minute to build it's database; fine in a deployment scenario, but intolerable if you want to write tens and hundreds of integration tests.

This package helps you keep database initialization as fast as possible, to make it more feasible to perform database tests and give you more confidence about the operation of your database.

Here's how to use it...

## Usage

    // use Blink to create a new database;
    var factory = Blink.CreateDbFactory<MyContext, MyEfProject.Configuration>( () => new MyContext() );
    
    using(var ctx = factory.GetDatabase())
    {
        // use the context here;
        ...
    }
 
OR THIS???

    // use Blink to create a new database;
    var factory = Blink.CreateDbFactory<MyContext, MyEfProject.Configuration>( () => new MyContext() );
    
    factory.ExecuteDbCode(context => {
        // use the context here;
        ...
    });


<!--
    // use the Blink initializer;
    var initializer = new BlinkInitializer<MyContext, MyEfProject.Configuration>(usingTransactions: false);
    Database.SetInitializer<AiTrackRecordContext>(initializer);
    
    // create your context;
    using(var context = new MyContext())
    {
        context.Database.Initialize(force: true);
    
        // use the context here;
        ...
    }
-->

What does it do?
-----

The first time it's called, it creates a database from scratch, applying all migrations and running the seed method, so that you have a fresh Entity Framework database, just as if you'd deleted the DB and called 'update-database' in the Package Manager console. 

Subsequent times, it searches a cache for a backup, and restores the backup instead of rebuilding the database. This means that your tests now work much faster.

<!--

## Tips and tricks

For extra speed, you can use transactions to speed up your code.

As above, use the Blink intializer, but like this, passing `useTransactions: true` to the constructor and putting all your DB code in a transaction;

    // use the Blink initializer with transactions;
    var initializer = new BlinkInitializer<MyContext, MyEfProject.Configuration>(usingTransactions: true);
    Database.SetInitializer<AiTrackRecordContext>(initializer);
    
    // create your context;
    using(var context = new MyContext())
    {
        context.Database.Initialize(force: true);
        var tran = context.Database.BeginTransaction;

        try
        {
        // use the context here;
        ...
        }
        finally
        {
            tran.Rollback();
        }
    }

The parameter to the constructor is a promise *from* you, *to* blink. You are promising to use transactions to roll back any changes you make to the database. That means on the second and subsequent call to the test method, there's no need to reinitialize *at all*, since it's a guaranteed, fresh database. This means your tests will run even faster.

-->

## Gotchas

While the library will keep your database initializations fast, it does this by avoiding all the expensive tests that EF to make sure the database hasn't gone stale. So several things might cause the caching to go wrong;

1) You change the content of the `Configuration.Seed()` method. If you seed different data, Blink doesn't know about it, and it's internal cache will be stale.

2) You add a migration, or other changes to the domain. Blink won't know about it and will continue to use your old data, incorrectly.

To fix this, it's necessary to wipe Blink's cache of database backups. They live in your personal AppData directory, in

    C:\Users\firstname.lastname\AppData\Roaming\Blink

Delete the contents of this folder between test runs to reset the cache and get your testing back on track.