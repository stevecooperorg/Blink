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

        private readonly BlinkDbFactoryMethod<TContext> createContext;
        private readonly BlinkPreparationContext context;

        internal BlinkDbFactory(BlinkDbFactoryMethod<TContext> createContext, BlinkPreparationContext context)
        {
            this.createContext = createContext;
            this.context = context;
        }

        public void ExecuteDbCode(BlinkDBWorkerMethod<TContext> workPayload)
        {
            var initializer = new BlinkDatabaseInitializer<TContext, TMigrationsConfiguration>(this.context);
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
