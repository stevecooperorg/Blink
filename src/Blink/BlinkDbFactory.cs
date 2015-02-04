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

        public void ExecuteDbCode(BlinkDBWorkerMethod<TContext> workPayload, params BlinkDBWorkerMethod<TContext>[] extraWorkPayloads)
        {
            lock (globalSyncRoot)
            {
                if (this.preparationOptions.EnableLogging)
                {
                    Logging.Enable();
                }
                else
                {
                    Logging.Disable();
                }


                Log("Setting initializer");

                var initializer = new BlinkDatabaseInitializer<TContext, TMigrationsConfiguration>(this.preparationOptions);
                //var initializer = new NullDatabaseInitializer<TContext>();
                Database.SetInitializer<TContext>(initializer);

                Log("Creating context");

                using (var ctx = this.createContext())
                {

                    Log("Initializing DB");

                    ctx.Database.Initialize(force: true);

                    Log("Opening transaction");

                    using (var scope = new TransactionScope())
                    {
                        Log("Performing work");

                        workPayload(ctx);

                        if (extraWorkPayloads.Length > 0)
                        {
                            foreach (var extraWorkPayload in extraWorkPayloads)
                            {
                                Log("Performing additional work item");
                                using (var extraContext = this.createContext())
                                {

                                    //extraContext.Database.UseTransaction(tran.UnderlyingTransaction);
                                    extraWorkPayload(extraContext);
                                }
                                Log("Finished performing additional work item");
                            }
                        }

                        Log((extraWorkPayloads.Length > 0 ? "All work" : "work") + " performed successfully");
                    }
                }
            }
        }
    }
}
