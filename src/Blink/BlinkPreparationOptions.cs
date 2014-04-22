using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blink
{
    internal class BlinkPreparationOptions
    {
        public BlinkDBCreationMode DBCreationMode { get; private set; }
        public BlinkPreparationOptions(BlinkDBCreationMode dbCreationMode)
        {
            this.DBCreationMode = dbCreationMode;
        }
    }
}
