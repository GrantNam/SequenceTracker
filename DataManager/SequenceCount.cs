using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SequenceTracker
{
    [OracleTable("SEQUENCEINFO")]
    internal class SequenceCount
    {
        //Index
        //-----------------------------------
        [OracleIndex("CARTYPEMAPPING", "CARTYPE", "CARTYPENO")]
        internal static List<string> Line_1_CARTYPE = new List<string>();

        [OracleIndex("CARTYPEMAPPING", "CARTYPE", "CARTYPENO")]
        internal static List<string> Line_2_CARTYPE = new List<string>();

        //Fields
        //-------------------------------------
        [OracleColumn("STATIONCODE")]
        internal string StationCode;

        [OracleColumn("PRODDATE")]
        internal DateTime Date;

        [OracleColumn("CARTYPE")]
        internal string CarType = "";

        [OracleColumn("CARTYPE")]
        internal int Count { get; set; }

        //Flags
        //-----------------------------------
        internal static bool isInitialized = false;

        //Constructor
        //-----------------------------------
        public SequenceCount() { }

        internal SequenceCount(string theStationCode, DateTime theDate, string theCarType, int theCount) : this()
        {
            this.StationCode = theStationCode;
            this.Date = theDate;
            this.CarType = theCarType;
            this.Count = theCount;
        }

        //Initialize
        //----------------------------------
        internal static void Init(List<string>line_1, List<string>line_2)
        {
            if (line_1.Count > 3) { throw new ArgumentException("Too many cartypes on line 1"); }
            if (line_2.Count > 3) { throw new ArgumentException("Too many cartypes on line 2"); }

            Line_1_CARTYPE.Clear();
            Line_2_CARTYPE.Clear();

            int i = 1;
            int j = 1;

            foreach (string line in line_1)
            {   
                if(line is null) { continue; }
                Line_1_CARTYPE.Add(line);
                AppConstants.CarTypes_Line_1.Add(i, line);
                i++;
            }
            foreach (string line in line_2)
            {
                if (line is null) { continue; }
                Line_2_CARTYPE.Add(line);
                AppConstants.CarTypes_Line_2.Add(j, line);
                j++;
            }

            isInitialized = true;
        }
        //Methods
        //-----------------------------------

       
    }
}
