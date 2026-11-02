using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
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

        [OracleColumn("PRODDATE")]
        internal int Count;

        //internal LINE Line { get; set; }

        //Constructor
        //-----------------------------------
        public SequenceCount() { }
        internal SequenceCount(string theStationCode, DateTime theDate, int theCount): this()
        {
            this.StationCode = theStationCode;
            this.Date = theDate;
            this.Count = theCount;
        }

        //Methods
        //-----------------------------------
    }
}
