using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blink.SQL;

namespace Blink
{
    class BlinkDatabaseInitializer<TContext, TMigrationsConfiguration> : IDatabaseInitializer<TContext>
        where TContext : DbContext
        where TMigrationsConfiguration : DbMigrationsConfiguration<TContext>, new()
    {
        private readonly BlinkPreparationContext context;

        public BlinkDatabaseInitializer(BlinkPreparationContext context)
        {
            this.context = context; 
        }

        public void InitializeDatabase(TContext context)
        {
            var mode = this.context.DBCreationMode;
            var dbExists = context.Database.Exists();
            if (!dbExists)
            {
                // we can't re-use the db if it doesn't yet exist, so for this run we're
                // switching to creating it;
                mode = BlinkDBCreationMode.RecreateEveryTest;
            }

            // identify this context in the cache;
            var hash = context.DbContextHash();
            var dbName = context.Database.Connection.Database;
            var backupFile = Path.Combine(this.context.BackupLocation, string.Format("BlinkDb_{0}.bak", hash));
   
            // is there a backup file for this context?
            bool backupExists = File.Exists(backupFile);

            if (!backupExists)
            {
                // no backup for this version -- we'll need to 
                // recreate - the DB will be out of date.
                mode = BlinkDBCreationMode.RecreateEveryTest;
            }

            if (mode == BlinkDBCreationMode.UseDBIfItAlreadyExists)
            {
                // no need to do anything
                return;
            }

            if (dbExists)
            { 
                // nuke the database
                ForceDropDatabase(context);
            }

            if (backupExists)
            {
                // restore from disk
                RestoreDb(context, backupFile);
            }
            else
            {
                // rebuild db from scratch and backup for later runs;
                BuildDb(context);
                BackupDb(context, backupFile);
            }
        }

        private void RestoreDb(TContext context, string backupFile)
        {
            context.ExecuteSqlAsMaster(SqlScripts.Restore, new
            {
                databaseName = context.Database.Connection.Database,
                backupPath = backupFile
            });
        }

        private void BackupDb(TContext context, string backupFile)
        {
            context.ExecuteSqlAsMaster(SqlScripts.Backup, new {
                sourceDb = context.Database.Connection.Database,
                backupPath = backupFile
            });
        }

        private void BuildDb(TContext context)
        {
            var config = new TMigrationsConfiguration();
            config.TargetDatabase = new System.Data.Entity.Infrastructure.DbConnectionInfo(context.Database.Connection.ConnectionString, "System.Data.SqlClient");
            var migrator = new DbMigrator(config);
            migrator.Update();
        }

        private void ForceDropDatabase(TContext context)
        {
            if (context.Database.Exists())
            {
                // set the database to SINGLE_USER so it can be dropped
                context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "ALTER DATABASE [" + context.Database.Connection.Database + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");

                // drop the database
                context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "USE master DROP DATABASE [" + context.Database.Connection.Database + "]");
            }
        }
    }
}
