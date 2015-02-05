using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Blink.Util;

namespace Blink
{
    public class BlinkDbFactory<TContext, TMigrationsConfiguration>
        where TContext : DbContext
        where TMigrationsConfiguration : DbMigrationsConfiguration<TContext>, new()
    {

        //private static object globalSyncRoot = new object();

        private static bool runningATest = false;

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

        public async Task ExecuteDbCode(BlinkDBWorkerMethod<TContext> workPayload, params BlinkDBWorkerMethod<TContext>[] extraWorkPayloads)
        {
            if (runningATest)
            {
                throw new InvalidOperationException("Cannot run more than one Blink test at a time; find out how your test environment can be made to serialise tests");
            }

            runningATest = true;

            //lock (globalSyncRoot)
            try
            {
                Log("Setting initializer");

                var initializer = new BlinkDatabaseInitializer<TContext, TMigrationsConfiguration>(this.preparationOptions);

                Database.SetInitializer<TContext>(initializer);

                Log("Creating context");

                using (var ctx = this.createContext())
                {
                    Log("Initializing DB");

                    ctx.Database.Initialize(force: true);

                    Log("Opening transaction");

                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        Log("Performing work");

                        // do the work on the first, initialised context
                        await workPayload(ctx);

                        if (extraWorkPayloads.Length > 0)
                        {
                            foreach (var extraWorkPayload in extraWorkPayloads)
                            {
                                // do extra work on a new context but in the same transaction
                                Log("Performing additional work item");

                                using (var extraContext = this.createContext())
                                {
                                    await extraWorkPayload(extraContext);
                                }

                                Log("Finished performing additional work item");
                            }
                        }

                        Log((extraWorkPayloads.Length > 0 ? "All work" : "work") + " performed successfully");
                    }
                }
            }
            finally
            {
                runningATest = false;
            }

        }
    }
}
