using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blink.Util;

namespace Blink
{
    public class BlinkDbFactory<TContext, TMigrationsConfiguration> 
        where TContext : DbContext 
        where TMigrationsConfiguration : DbMigrationsConfiguration<TContext>, new()
    {

        private static object globalSyncRoot = new object();

        private readonly BlinkDbFactoryMethod<TContext> createContext;
        private readonly BlinkPreparationOptions preparationOptions;

        private void Log(string message)
        {
            Logging.Log(message);
        }

        internal BlinkDbFactory(BlinkDbFactoryMethod<TContext> createContext, BlinkPreparationOptions preparationOptions)
        {
            this.createContext = createContext;
            this.preparationOptions = preparationOptions;
        }

        public void ExecuteDbCode(BlinkDBWorkerMethod<TContext> workPayload)
        {
            Log("Acquiring lock");
            lock (globalSyncRoot)
            {
                Log("Setting initializer");

                var initializer = new BlinkDatabaseInitializer<TContext, TMigrationsConfiguration>(this.preparationOptions);
                //var initializer = new NullDatabaseInitializer<TContext>();
                Database.SetInitializer<TContext>(initializer);

                Log("Creating context");

                var ctx = this.createContext();

                Log("Initializing DB");

                ctx.Database.Initialize(force: true);

                Log("Opening transaction");

                var tran = ctx.Database.BeginTransaction();
                try
                {
                    Log("Performing work");

                    workPayload(ctx);

                    Log("Work performed successfully");

                }
                finally
                {
                    Log("Rolling back transaction");
                    tran.Rollback();
                    Log("Rolled back transaction");
                }
            }
        }
    }
}
