using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blink
{
    /// <summary>
    /// Use this to provide a string that changes when some part of the database setup code changes, separate from the entity data model. For instance, when the seed data changes, changing this will make sure the database is re-seeded.
    /// </summary>
    /// <returns>
    /// A string which differs for different seed data or other initialization process.
    /// </returns>
    public delegate string DatabaseStateSignatureGenerator();
}
