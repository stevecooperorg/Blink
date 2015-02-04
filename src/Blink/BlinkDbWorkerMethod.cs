using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blink
{
    public delegate Task BlinkDBWorkerMethod<TContext>(TContext context) where TContext : DbContext;
}
