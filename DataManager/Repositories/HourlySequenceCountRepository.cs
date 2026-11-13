using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequenceTracker
{
    internal class HourlySequenceCountRepository : iDataRepository
    {
        //Constructors
        //--------------------------------------------------------------------------
        internal HourlySequenceCountRepository(OracleManager theOracleManager, DataManager theDataManager): base(theOracleManager, theDataManager)
        {

        }

        //Initialization
        //-----------------------------------------------------------------------
        internal override void Initialize()
        {
            InitTable();
            SubscribeEvents();
        }

        internal void InitTable()
        {
            dataTable = new DataTable();
            dataTable.TableName = "SEQUENCEINFO";
            dataTable.Columns.Add("TIMESTAMP", typeof(int)); //yyyyMMddHH
            dataTable.Columns.Add("PRODDATE", typeof(int)); //yyyyMMddHH
            dataTable.Columns.Add("TIME", typeof(string));
            dataTable.Columns.Add("LINE_1_CARTYPE_1", typeof(int));
            dataTable.Columns.Add("LINE_1_CARTYPE_2", typeof(int));
            dataTable.Columns.Add("LINE_1_CARTYPE_3", typeof(int));
            dataTable.Columns.Add("LINE_1_TOTAL", typeof(int));
            dataTable.Columns.Add("LINE_2_CARTYPE_1", typeof(int));
            dataTable.Columns.Add("LINE_2_CARTYPE_2", typeof(int));
            dataTable.Columns.Add("LINE_2_CARTYPE_3", typeof(int));
            dataTable.Columns.Add("LINE_2_TOTAL", typeof(int));

            foreach (DataColumn column in dataTable.Columns)
            {
                if (column.DataType == typeof(int))
                {
                    column.DefaultValue = 0;
                }
            }

            dataTable.PrimaryKey = new DataColumn[]
            {
                dataTable.Columns["TIMESTAMP"],
            };
        }
        internal void SubscribeEvents()
        {
            this.RowDataUpdated += dataManager.OnRowDataUpdated;
        }

        //Methods
        //------------------------------------------------------------------
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

        //WHERE: Date passed as a parameter
        internal override (string, OracleParameter[]) getSQL(Filter filter)
        {
            OracleParameter[] parameters = new OracleParameter[1];
            parameters[0] = new OracleParameter("ProdDate", convertDate(filter.startDate));

            string SQL = @"";
            string SELECT = "SELECT " + "SUBSTR(REPORTDATE, 0, 10) as PRODTIME, PRODDATE, STATIONCODE, " + "CARTYPE" + ", Count(*) as COUNT"; //PRODDATE 'yyyyMMddHH'
            string FROM = " FROM " + dataTable.TableName;
            string WHERE = " WHERE " + "PRODDATE = :ProdDate";
            string GROUPBY = " GROUP BY " + "SUBSTR(REPORTDATE, 0, 10), PRODDATE, STATIONCODE, CARTYPE";

            SQL += SELECT + FROM + WHERE + GROUPBY;

            Console.WriteLine(SQL);

            return (SQL, parameters);
        }
        internal override void updateTable(DataTable updateTable)
        {
            if (updateTable == null)
            {
                throw new ArgumentNullException("Return DataTable is null");
            }

            dataTable.Clear();

            try
            {
                for (int i = 0; i < updateTable.Rows.Count; i++)
                {
                    DataRow row = dataTable.NewRow();

                    row["TIMESTAMP"] = updateTable.Rows[i]["PRODTIME"];
                    row["PRODDATE"] = updateTable.Rows[i]["PRODDATE"];

                    if (updateTable.Rows[i]["STATIONCODE"].ToString() == "P370")
                    {
                        if (DailySequenceCountRepository.LINE_1_CARTYPE_INDEX.ElementAtOrDefault(0) == updateTable.Rows[i]["CARTYPE"].ToString())
                        {
                            row["LINE_1_CARTYPE_1"] = updateTable.Rows[i]["COUNT"];
                        }
                        else if (DailySequenceCountRepository.LINE_1_CARTYPE_INDEX.ElementAtOrDefault(1) == updateTable.Rows[i]["CARTYPE"].ToString())
                        {
                            row["LINE_1_CARTYPE_2"] = updateTable.Rows[i]["COUNT"];
                        }
                        else if (DailySequenceCountRepository.LINE_1_CARTYPE_INDEX.ElementAtOrDefault(2) == updateTable.Rows[i]["CARTYPE"].ToString())
                        {
                            row["LINE_1_CARTYPE_3"] = updateTable.Rows[i]["COUNT"];
                        }
                    }
                    else if (updateTable.Rows[i]["STATIONCODE"].ToString() == "P375")
                    {
                        if (DailySequenceCountRepository.LINE_2_CARTYPE_INDEX.ElementAtOrDefault(0) == updateTable.Rows[i]["CARTYPE"].ToString())
                        {
                            row["LINE_2_CARTYPE_1"] = updateTable.Rows[i]["COUNT"];
                        }
                        else if (DailySequenceCountRepository.LINE_2_CARTYPE_INDEX.ElementAtOrDefault(1) == updateTable.Rows[i]["CARTYPE"].ToString())
                        {
                            row["LINE_2_CARTYPE_2"] = updateTable.Rows[i]["COUNT"];
                        }
                        else if (DailySequenceCountRepository.LINE_2_CARTYPE_INDEX.ElementAtOrDefault(2) == updateTable.Rows[i]["CARTYPE"].ToString())
                        {
                            row["LINE_2_CARTYPE_3"] = updateTable.Rows[i]["COUNT"];
                        }
                    }

                    row["LINE_1_TOTAL"] = Convert.ToInt32(row["LINE_1_CARTYPE_1"]) + Convert.ToInt32(row["LINE_1_CARTYPE_2"]) + Convert.ToInt32(row["LINE_1_CARTYPE_3"]);
                    row["LINE_2_TOTAL"] = Convert.ToInt32(row["LINE_2_CARTYPE_1"]) + Convert.ToInt32(row["LINE_2_CARTYPE_2"]) + Convert.ToInt32(row["LINE_2_CARTYPE_3"]);

                    dataTable.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            FinalizeTable();


            if (DataManager.filter.AutoUpdate && DisplayPanel.detailWindow)
            {
                OnAutoDataUpdated();
            }
            else
            {
                OnRowDataUpdated();
            }
                
        }

        internal void FinalizeTable()
        {
            foreach (DataRow row in dataTable.Rows)
            {
                row["LINE_1_TOTAL"] = Convert.ToInt32(row["LINE_1_CARTYPE_1"]) + Convert.ToInt32(row["LINE_1_CARTYPE_2"]) + Convert.ToInt32(row["LINE_1_CARTYPE_3"]);
                row["LINE_2_TOTAL"] = Convert.ToInt32(row["LINE_2_CARTYPE_1"]) + Convert.ToInt32(row["LINE_2_CARTYPE_2"]) + Convert.ToInt32(row["LINE_2_CARTYPE_3"]);
            }
        }

        internal override void AutoUpdate()
        {
            if (!DisplayPanel.detailWindow)
            {
                return;
            }

            Filter theFilter = new Filter();
            theFilter.setFilter(DataManager.filter);

            try
            {   
                int ProdDate = dataManager.mainWindow.displayPanel.dailyDetailWindow.getPRODDATE();
                theFilter.startDate = DateTime.ParseExact(ProdDate.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                requestUpdate(theFilter);
            }
            catch(Exception ex)
            {
                Console.WriteLine (ex.Message);
            }
        }
    }
}
