using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blink
{
    internal class BlinkPreparationContext
    {
        public string BackupLocation { get; private set; }
        public string DataLocation { get; private set; }

        public BlinkDBCreationMode DBCreationMode { get; private set; }
        public BlinkPreparationContext(string backupLocation, string dataLocation, BlinkDBCreationMode dbCreationMode)
        {
            this.BackupLocation = backupLocation;
            this.DataLocation = dataLocation;
            this.DBCreationMode = dbCreationMode;
        }
    }
}
