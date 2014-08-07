using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blink
{
    public class BlinkDB
    {
        public static BlinkDbFactory<TContext, TMigrationsConfiguration> CreateDbFactory<TContext, TMigrationsConfiguration>(BlinkDBCreationMode dbCreationMode, BlinkDbFactoryMethod<TContext> createContext, DatabaseStateSignatureGenerator signatureGenerator = null) 
            where TContext : DbContext 
            where TMigrationsConfiguration : DbMigrationsConfiguration<TContext>, new()

        {
            var options = new BlinkPreparationOptions(dbCreationMode) { SignatureGenerator = signatureGenerator };
            var factory = new BlinkDbFactory<TContext, TMigrationsConfiguration>(createContext, options);
            return factory;

        }
    }
}
