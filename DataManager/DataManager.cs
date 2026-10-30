using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequenceTracker
{
    internal class DataManager
    {
        //Managers
        //---------------------------------------------------------------
        OracleManager oracleManager;


        internal List<SequenceCount> Sequences = new List<SequenceCount>();


        //constructor
        //----------------------------------------------------------------
        public DataManager() { }
        internal DataManager(OracleManager theManager): this()
        {
            oracleManager = theManager;
        }
    }
}
