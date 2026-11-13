using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SequenceTracker
{
    internal class DailySequenceCountRepository : iDataRepository
    {
        //Fields
        //-----------------------------------------------------------
        internal static List<string> LINE_1_CARTYPE_INDEX = new List<string>();
        internal static List<string> LINE_2_CARTYPE_INDEX = new List<string>();

        //Events
        //------------------------------------------------------------

        //Constructors
        //------------------------------------------------------------
        public DailySequenceCountRepository(OracleManager theOracleManager, DataManager theDataManager): base(theOracleManager, theDataManager)
        {

        }

         //Initializaiton
        //---------------------------------------------------------------------------------
        internal override void Initialize()
        {
            InitTable();
            InitIndex();
            SubscribeEvents();
        }
        
        
        internal void InitIndex()
        {
            requestIndexUpdate();
        }
        internal void InitTable()
        {
            dataTable = new DataTable();
            dataTable.TableName = "SEQUENCEINFO";
            dataTable.Columns.Add("TIMESTAMP", typeof(int)); //yyyyMMddHH
            //dataTable.Columns.Add("STATIONCODE", typeof(string));
            dataTable.Columns.Add("PRODDATE", typeof(DateTime));
            dataTable.Columns.Add("LINE_1_CARTYPE_1", typeof(int));
            dataTable.Columns.Add("LINE_1_CARTYPE_2", typeof(int));
            dataTable.Columns.Add("LINE_1_CARTYPE_3", typeof(int));
            dataTable.Columns.Add("LINE_1_TOTAL", typeof(int));
            dataTable.Columns.Add("LINE_2_CARTYPE_1", typeof(int));
            dataTable.Columns.Add("LINE_2_CARTYPE_2", typeof(int));
            dataTable.Columns.Add("LINE_2_CARTYPE_3", typeof(int));
            dataTable.Columns.Add("LINE_2_TOTAL", typeof(int));

            foreach(DataColumn column in dataTable.Columns)
            {
                if (column.DataType == typeof(int))
                {
                    column.DefaultValue = 0;
                }
            }

            dataTable.PrimaryKey = new DataColumn[]
            {
                dataTable.Columns["TIMESTAMP"],
                //dataTable.Columns["STATIONCODE"]
            };
        }
        internal void SubscribeEvents()
        {
            this.DataUpdated += dataManager.OnDataUpdated;
        }
        //Methods
        //---------------------------------------------------------------------------------------
        internal (string, OracleParameter[]) getIndexSQL()
        {
            OracleParameter[] parameters = new OracleParameter[0];
            return (@"SELECT CARTYPENO, CARTYPE FROM CARTYPEMAPPING ORDER BY CARTYPENO ASC", parameters);
        }
        internal void updateIndex(DataTable theTable)
        {
            if(theTable.Rows.Count > 6)
            {
                throw new Exception("too many cartypes");
            }

            foreach (DataRow row in theTable.Rows)
            {
                if (Convert.ToInt32(row["LINE"]) == 1)
                {
                    LINE_1_CARTYPE_INDEX.Add(row["CARTYPE"].ToString());
                }
                else
                {
                    LINE_2_CARTYPE_INDEX.Add(row["CARTYPE"].ToString());
                }
            }
        }
        internal void requestIndexUpdate()
        {
            string SQL;
            OracleParameter[] parameters;

            (SQL, parameters) = getIndexSQL();

            List<LINE> lines = new List<LINE>();
            lines.Add(LINE.FINAL_1);
            lines.Add(LINE.FINAL_2);

            oracleManager.getDataEnqueue(SQL, parameters, lines, updateIndex);
        }

        internal override void requestUpdate(Filter thefilter)
        {
            string SQL;
            OracleParameter[] parameters;
            (SQL, parameters) = getSQL(thefilter);

            List<LINE> lines = new List<LINE>();
            lines.Add(LINE.FINAL_1);
            lines.Add(LINE.FINAL_2);

            oracleManager.getDataEnqueue(SQL, parameters, lines, updateTable);
        }
        //returns string of sql for Oracle Querrying 
        internal override (string, OracleParameter[]) getSQL(Filter thefilter)
        {
            OracleParameter[] parameters = new OracleParameter[2];
            parameters[0] = new OracleParameter("startDate", convertDate(thefilter.startDate));
            parameters[1] = new OracleParameter("endDate", convertDate(thefilter.endDate));

            string DATE = "PRODDATE >= :startDate AND PRODDATE <= :endDate";

            string SQL = @"";

            string SELECT = " SELECT " + "PRODDATE, "+ "STATIONCODE, " + "CARTYPE, " + "Count( * ) as COUNT";
            string FROM = " FROM " + this.dataTable.TableName;
            string WHERE = " WHERE " + DATE;
            string GROUPBY = " GROUP BY " + "PRODDATE, " + "CARTYPE, " + "STATIONCODE";
            string ORDERBY = " ORDER BY " + "PRODDATE" + ", " + "CARTYPE " + "DESC";

            SQL += SELECT + FROM + WHERE + GROUPBY + ORDERBY;

            return (SQL, parameters);
        }

        internal override void updateTable(DataTable updateTable)
        {   
            if(updateTable == null)
            {
                throw new ArgumentNullException("Return DataTable is null");
            }

            lock (dataTable)
            {
                dataTable.Clear();

                try
                {
                    for (int i = 0; i < updateTable.Rows.Count; i++)
                    {
                        bool rowExist = false;

                        foreach (DataRow theRow in dataTable.Rows)
                        {
                            int? RowPK = Convert.ToInt32(theRow["TIMESTAMP"]);
                            int? dataPK = Convert.ToInt32(updateTable.Rows[i]["PRODDATE"]);

                            if (RowPK == dataPK)
                            {
                                rowExist = true;

                                if (updateTable.Rows[i]["STATIONCODE"].ToString() == "P370")
                                {
                                    if (LINE_1_CARTYPE_INDEX.ElementAtOrDefault(0) == updateTable.Rows[i]["CARTYPE"].ToString())
                                    {
                                        theRow["LINE_1_CARTYPE_1"] = updateTable.Rows[i]["COUNT"];
                                    }
                                    else if (LINE_1_CARTYPE_INDEX.ElementAtOrDefault(1) == updateTable.Rows[i]["CARTYPE"].ToString())
                                    {
                                        theRow["LINE_1_CARTYPE_2"] = updateTable.Rows[i]["COUNT"];
                                    }
                                    else if (LINE_1_CARTYPE_INDEX.ElementAtOrDefault(2) == updateTable.Rows[i]["CARTYPE"].ToString())
                                    {
                                        theRow["LINE_1_CARTYPE_3"] = updateTable.Rows[i]["COUNT"];
                                    }
                                }
                                else if (updateTable.Rows[i]["STATIONCODE"].ToString() == "P375")
                                {
                                    if (LINE_2_CARTYPE_INDEX.ElementAtOrDefault(0) == updateTable.Rows[i]["CARTYPE"].ToString())
                                    {
                                        theRow["LINE_2_CARTYPE_1"] = updateTable.Rows[i]["COUNT"];
                                    }
                                    else if (LINE_2_CARTYPE_INDEX.ElementAtOrDefault(1) == updateTable.Rows[i]["CARTYPE"].ToString())
                                    {
                                        theRow["LINE_2_CARTYPE_2"] = updateTable.Rows[i]["COUNT"];
                                    }
                                    else if (LINE_2_CARTYPE_INDEX.ElementAtOrDefault(2) == updateTable.Rows[i]["CARTYPE"].ToString())
                                    {
                                        theRow["LINE_2_CARTYPE_3"] = updateTable.Rows[i]["COUNT"];
                                    }
                                }
                                break;
                            }
                        }

                        if (!rowExist)
                        {
                            DataRow row = dataTable.NewRow();

                            row["TIMESTAMP"] = updateTable.Rows[i]["PRODDATE"];
                            row["PRODDATE"] = DateTime.ParseExact(updateTable.Rows[i]["PRODDATE"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                            if (updateTable.Rows[i]["STATIONCODE"].ToString() == "P370")
                            {
                                if (LINE_1_CARTYPE_INDEX.ElementAtOrDefault(0) == updateTable.Rows[i]["CARTYPE"].ToString())
                                {
                                    row["LINE_1_CARTYPE_1"] = updateTable.Rows[i]["COUNT"];
                                }
                                else if (LINE_1_CARTYPE_INDEX.ElementAtOrDefault(1) == updateTable.Rows[i]["CARTYPE"].ToString())
                                {
                                    row["LINE_1_CARTYPE_2"] = updateTable.Rows[i]["COUNT"];
                                }
                                else if (LINE_1_CARTYPE_INDEX.ElementAtOrDefault(2) == updateTable.Rows[i]["CARTYPE"].ToString())
                                {
                                    row["LINE_1_CARTYPE_3"] = updateTable.Rows[i]["COUNT"];
                                }
                            }
                            else if (updateTable.Rows[i]["STATIONCODE"].ToString() == "P375")
                            {
                                if (LINE_2_CARTYPE_INDEX.ElementAtOrDefault(0) == updateTable.Rows[i]["CARTYPE"].ToString())
                                {
                                    row["LINE_2_CARTYPE_1"] = updateTable.Rows[i]["COUNT"];
                                }
                                else if (LINE_2_CARTYPE_INDEX.ElementAtOrDefault(1) == updateTable.Rows[i]["CARTYPE"].ToString())
                                {
                                    row["LINE_2_CARTYPE_2"] = updateTable.Rows[i]["COUNT"];
                                }
                                else if (LINE_2_CARTYPE_INDEX.ElementAtOrDefault(2) == updateTable.Rows[i]["CARTYPE"].ToString())
                                {
                                    row["LINE_2_CARTYPE_3"] = updateTable.Rows[i]["COUNT"];
                                }
                            }

                            dataTable.Rows.Add(row);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.Source);
                }

                FinalizeTable();
            }

            if (DataManager.filter.AutoUpdate && !SearchBar.searchRequested)
            {
                mergeTable();
                OnAutoDataUpdated();
            }
            else if(SearchBar.searchRequested == true)
            {
                SearchBar.searchRequested = false;
                OnDataUpdated();
            }
        }

        internal void FinalizeTable()
        {
            foreach(DataRow row in dataTable.Rows)
            {
                row["LINE_1_TOTAL"] = Convert.ToInt32(row["LINE_1_CARTYPE_1"]) + Convert.ToInt32(row["LINE_1_CARTYPE_2"]) + Convert.ToInt32(row["LINE_1_CARTYPE_3"]);
                row["LINE_2_TOTAL"] = Convert.ToInt32(row["LINE_2_CARTYPE_1"]) + Convert.ToInt32(row["LINE_2_CARTYPE_2"]) + Convert.ToInt32(row["LINE_2_CARTYPE_3"]);
            }
           
        }

        internal void mergeTable()
        {
            if (DisplayPanel.DGV_OPEN)
            {
                dataManager.DGV_DataTable.Merge(dataTable);
            }
            if (DisplayPanel.BAR_GRAPH_OPEN)
            {
                dataManager.BAR_GRAPH_DataTable.Merge(dataTable);
            }
            if (DisplayPanel.TREND_GRAPH_OPEN)
            {
                dataManager.TREND_GRAPH_DataTable.Merge(dataTable);
            }
        }

        internal override void AutoUpdate()
        {   
            if(!DisplayPanel.DGV_OPEN && !DisplayPanel.BAR_GRAPH_OPEN && !DisplayPanel.TREND_GRAPH_OPEN)
            {
                return;
            }

            List<DateTime> maxDate = new List<DateTime>();
            Filter theFilter = new Filter();
            theFilter.setFilter(DataManager.filter);

            if (DisplayPanel.DGV_OPEN)
            {
                maxDate.Add((DateTime)dataManager.DGV_DataTable.Compute("MAX(PRODDATE)", ""));
            }
            if (DisplayPanel.BAR_GRAPH_OPEN)
            {
                maxDate.Add((DateTime)dataManager.BAR_GRAPH_DataTable.Compute("MAX(PRODDATE)", ""));
            }
            if (DisplayPanel.TREND_GRAPH_OPEN)
            {
                maxDate.Add((DateTime)dataManager.TREND_GRAPH_DataTable.Compute("MAX(PRODDATE)", ""));
            }
            maxDate.Add(DataManager.filter.endDate.AddDays(-1));

            theFilter.startDate = maxDate.Min();

            requestUpdate(theFilter);
        }

        internal void AutoUpdate_DGV()
        {
            
        }
        internal void AutoUpdate_BAR_GRAPH()
        {

        }
        internal void AutoUpdate_TREND_GRAPH()
        {

        }
    }
}
