using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitForByteSupport
{
    public class BFBTemps
    {
        public BFBTemps(int index, int temperature)
        {
            Index = index;
            OldTemperature = temperature;
        }

        public int Index { get; set; }
        public int OldTemperature { get; set; }
        public int NewTemperature { get; set; }
    }
}
