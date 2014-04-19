using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blink
{
    public enum BlinkDBCreationMode
    {
        /// <summary>
        /// REcreates the database from backup every test
        /// </summary>
        RecreateEveryTest,

        /// <summary>
        /// Uses the database if it's already there; the DB must, therefore, be pristine.
        /// </summary>
        UseDBIfItAlreadyExists
    }
}
