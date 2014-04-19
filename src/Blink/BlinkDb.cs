using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blink
{
    public class BlinkDb
    {
        public static BlinkDbFactory<TContext, TMigrationsConfiguration> CreateDbFactory<TContext, TMigrationsConfiguration>(BlinkDbFactoryMethod<TContext> createContext) 
            where TContext : DbContext 
            where TMigrationsConfiguration : DbMigrationsConfiguration<TContext>, new()

        {
            var appData = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var cacheLocation = System.IO.Path.Combine(appData, @"Blink\DatabaseCache");
            var factory = new BlinkDbFactory<TContext, TMigrationsConfiguration>(createContext, cacheLocation);
            return factory;

        }
    }
}
