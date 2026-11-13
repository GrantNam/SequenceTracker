using Oracle.ManagedDataAccess.Client;
using OracleInternal.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace SequenceTracker
{
    internal abstract class iDataRepository
    {
        //Managers
        //---------------------------------------
        internal OracleManager oracleManager;
        internal DataManager dataManager;

        //Fields
        //---------------------------------------
        internal DataTable dataTable;
        internal DataView dataView;

        //Events
        //------------------------------------------------------------
        internal event EventHandler DataUpdated;
        internal event EventHandler AutoDataUpdated;
        internal event EventHandler RowDataUpdated;

        //Evnet Handlers
        //-----------------------------------------------------------
        protected virtual void OnDataUpdated()
        {
            DataUpdated?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnAutoDataUpdated()
        {
            AutoDataUpdated?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnRowDataUpdated()
        {
            RowDataUpdated?.Invoke(this, EventArgs.Empty);
        }
        //Constructor
        //------------------------------------------------------------
        public iDataRepository(OracleManager theOracleManager, DataManager theDataManager)
        {
            oracleManager = theOracleManager;
            dataManager = theDataManager;
        }

        //Abstract Methods
        //------------------------------------------------------------
        //returns string of sql for Oracle Querrying 
        internal abstract (string, OracleParameter[]) getSQL(Filter thefilter);
        internal abstract void Initialize();

        internal abstract void updateTable(DataTable dataTable);
        internal abstract void requestUpdate(Filter thefilter);
        internal abstract void AutoUpdate();
        //Shared Methods
        //-------------------------------------------------------------
        internal string convertDate(DateTime date)
        {
            if (date == null)
            {
                throw new Exception("Date cannot be null");
            }
            string YYYYMMDD = date.ToString("yyyyMMdd");
            return YYYYMMDD;
        }

        internal bool isLine1(string line)
        {
            if(line.ToString() == "FINAL_1" ||  line.ToString() == "TRIM_1" || line.ToString() == "DOOR_1")
            {
                return true;
            }
            return false;
        }

        internal DataTable getDataTable()
        {
            return this.dataTable;
        }
    }
}
