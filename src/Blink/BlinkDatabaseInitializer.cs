using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blink.SQL;
using Blink.Util;

namespace Blink
{
    class BlinkDatabaseInitializer<TContext, TMigrationsConfiguration> : IDatabaseInitializer<TContext>
        where TContext : DbContext
        where TMigrationsConfiguration : DbMigrationsConfiguration<TContext>, new()
    {
        private readonly BlinkPreparationOptions context;

        public BlinkDatabaseInitializer(BlinkPreparationOptions context)
        {
            this.context = context;
        }

        private static string previouslyCalculatedHash = null;

        public void InitializeDatabase(TContext context)
        {
            Log("Initializing " + context.Database.Connection.Database);

            var mode = this.context.DBCreationMode;
            var dbExists = context.Database.Exists();
            if (!dbExists)
            {
                // we can't re-use the db if it doesn't yet exist, so for this run we're
                // switching to creating it;
                Log("Recreating the database because it does not yet exist.");
                mode = BlinkDBCreationMode.RecreateEveryTest;
            }

            Log("Reading SQL Server configuration and backup directory");
            var sqlServer = new SqlServerInfo(context);
            var backupDirectory = sqlServer.GetBackupDirectory();
            Log("Backup directory is " + backupDirectory);

            // identify this context in the cache;
            string hash;
            if (previouslyCalculatedHash == null)
            {
                Log("Calculating DB hash");
            
                hash = context.DbContextHash().WithoutPathCharacters();
                previouslyCalculatedHash = hash;
                Log("DB hash calculated");
            } 
            else
            {
                Log("Hash has already been calculated this run!");
                hash = previouslyCalculatedHash;
            }

            
            var dbName = context.Database.Connection.Database;
            var safeDbName = dbName.WithoutPathCharacters();
            var backupFile = Path.Combine(backupDirectory, string.Format("BlinkDb_{0}_{1}.bak", safeDbName, hash));

            // is there a backup file for this context?
            bool backupExists = File.Exists(backupFile);
            Log("Using backup file '" + backupFile + " for the database, which " + (backupExists ? "exists." : "does not exist."));

            if (!backupExists)
            {
                // no backup for this version -- we'll need to 
                // recreate - the DB will be out of date.
                Log("Recreating the database because there is no appropriate cache.");
                mode = BlinkDBCreationMode.RecreateEveryTest;
            }

            if (mode == BlinkDBCreationMode.UseDBIfItAlreadyExists)
            {
                // no need to do anything
                Log("The database already exists. Initialization is complete.");
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

                // because this is a new DB, the hash needs to be cleared
                previouslyCalculatedHash = null;
            }

            Log("The database already exists. Initialization is complete.");

        }

        private void Log(string message)
        {
            Logging.Log(message);
        }

        private void RestoreDb(TContext context, string backupFile)
        {
            Log("Restoring the database from '" + backupFile + "'.");
            context.ExecuteSqlAsMaster(SqlScripts.Restore, new
            {
                databaseName = context.Database.Connection.Database,
                backupPath = backupFile
            });
        }

        private void BackupDb(TContext context, string backupFile)
        {
            Log("Backing up the existing database.");
            context.ExecuteSqlAsMaster(SqlScripts.Backup, new
            {
                sourceDb = context.Database.Connection.Database,
                backupPath = backupFile
            });
        }

        private void BuildDb(TContext context)
        {
            Log("Creating the databsase and applying migrations.");
            var config = new TMigrationsConfiguration();
            config.TargetDatabase = new System.Data.Entity.Infrastructure.DbConnectionInfo(context.Database.Connection.ConnectionString, "System.Data.SqlClient");
            var migrator = new DbMigrator(config);
            migrator.Update();
        }

        private void ForceDropDatabase(TContext context)
        {
            if (context.Database.Exists())
            {
                Log("Dropping thr existing database.");

                // set the database to SINGLE_USER so it can be dropped
                context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "ALTER DATABASE [" + context.Database.Connection.Database + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");

                // drop the database
                context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "USE master DROP DATABASE [" + context.Database.Connection.Database + "]");
            }
        }
    }
}
