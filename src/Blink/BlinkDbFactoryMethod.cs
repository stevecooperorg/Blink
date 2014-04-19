using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blink
{
    public delegate TContext BlinkDbFactoryMethod<TContext>() where TContext : DbContext;
}
