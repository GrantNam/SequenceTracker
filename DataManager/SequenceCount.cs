using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequenceTracker
{
    [OracleTable("SEQUENCEINFO")]
    internal class SequenceCount
    {
        //Field
        //-----------------------------------
        [OracleColumn("STATIONCODE")]
        internal string StationCode;

        [OracleColumn("PRODDATE")]
        internal DateTime Date;

        [OracleCount("PRODDATE")]
        internal int Count;


        //Constructor
        //-----------------------------------
        public SequenceCount() { }
        internal SequenceCount(int theCount): this()
        {
        }
    }
}
