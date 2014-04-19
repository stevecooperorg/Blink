using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blink
{
    class BlinkDatabaseInitializer<TContext, TMigrationsConfiguration> : IDatabaseInitializer<TContext>
        where TContext : DbContext
        where TMigrationsConfiguration : DbMigrationsConfiguration<TContext>, new()
    {
        private readonly string cacheLocation;

        public BlinkDatabaseInitializer(string cacheLocation)
        {
            this.cacheLocation = cacheLocation;  
        }

        public void InitializeDatabase(TContext context)
        {
            // make sure the file cache exists;
            Directory.CreateDirectory(this.cacheLocation);

            // identify this context in the cache;
            var hash = context.DbContextHash();

            var backupFile = Path.Combine(this.cacheLocation, string.Format("BlinkDb_{0}.bak", hash));

            bool backupExists = File.Exists(backupFile);

            throw new NotImplementedException();

            //if (context.Database.Exists())
            //{
            //    // set the database to SINGLE_USER so it can be dropped
            //    context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "ALTER DATABASE [" + context.Database.Connection.Database + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");

            //    // drop the database
            //    context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "USE master DROP DATABASE [" + context.Database.Connection.Database + "]");
            //}

            //// SC: some code below from http://stackoverflow.com/a/15919627/6722
            //var config = new TMigrationsConfiguration();
            //config.TargetDatabase = new System.Data.Entity.Infrastructure.DbConnectionInfo(context.Database.Connection.ConnectionString, "System.Data.SqlClient");
            //var migrator = new DbMigrator(config);
            //migrator.Update();
        }
    }
}
