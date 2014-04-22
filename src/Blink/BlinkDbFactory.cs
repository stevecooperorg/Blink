using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blink
{
    public class BlinkDbFactory<TContext, TMigrationsConfiguration> 
        where TContext : DbContext 
        where TMigrationsConfiguration : DbMigrationsConfiguration<TContext>, new()
    {

        private static object globalSyncRoot = new object();

        private readonly BlinkDbFactoryMethod<TContext> createContext;
        private readonly BlinkPreparationOptions preparationOptions;

        internal BlinkDbFactory(BlinkDbFactoryMethod<TContext> createContext, BlinkPreparationOptions preparationOptions)
        {
            this.createContext = createContext;
            this.preparationOptions = preparationOptions;
        }

        public void ExecuteDbCode(BlinkDBWorkerMethod<TContext> workPayload)
        {
            lock (globalSyncRoot)
            {
                var initializer = new BlinkDatabaseInitializer<TContext, TMigrationsConfiguration>(this.preparationOptions);
                //var initializer = new NullDatabaseInitializer<TContext>();
                Database.SetInitializer<TContext>(initializer);

                var ctx = this.createContext();

                ctx.Database.Initialize(force: true);

                var tran = ctx.Database.BeginTransaction();
                try
                {
                    workPayload(ctx);
                }
                finally
                {
                    tran.Rollback();
                }
            }
        }
    }
}
