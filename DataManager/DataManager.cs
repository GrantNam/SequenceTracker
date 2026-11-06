using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;


namespace SequenceTracker
{
    internal class DataManager
    {
        //Managers
        //---------------------------------------------------------------
        internal OracleManager oracleManager;
        internal MainWindow mainWindow;

        internal List<SequenceCount> Sequences = new List<SequenceCount>();
        internal List<SequenceCount> DailySequences = new List<SequenceCount>();

        internal static Filter filter = new Filter();

        //Events
        //---------------------------------------------------------------
        public event EventHandler DataUpdated;
        public event EventHandler RowDataUpdated;
        //Event Handlers
        //---------------------------------------------------------------
        internal void OnSerchRequested(object sender, EventArgs e)
        {
            requestUpdate();
        }

        internal void OnRowHeaderClicked(object sender, SequenceCountResult result)
        {
            requestRowData(result);
        }
        //constructor
        //----------------------------------------------------------------
        public DataManager() { }
        internal DataManager(OracleManager theManager): this()
        {
            oracleManager = theManager;
        }

        //Initialization
        //---------------------------------------------------------------
        internal void Init()
        {
            SubscribeEvents();
            InitializeSequenceCount();
        }

        internal void InitializeSequenceCount()
        {
            string sql = getSeqCountInitSQL();

            oracleManager.getSequenceCountInitEnqueue(sql, SequenceCount.Init);
        }

        internal void SubscribeEvents()
        {
            mainWindow.SearchRequested += OnSerchRequested;
            mainWindow.RowHeaderClicked += OnRowHeaderClicked;
        }
        //Methods
        //---------------------------------------------------------------
        internal void Clear()
        {
            Sequences.Clear();
        }

        internal void requestRowData(SequenceCountResult result)
        {
            string sql = getRowSQL(result);
            List<LINE> lines = new List<LINE>();

            if (filter.LINE_1)
            {
                lines.Add(LINE.FINAL_1);
            }
            if (filter.LINE_2)
            {
                lines.Add(LINE.FINAL_2);
            }

            oracleManager.getOracleDataEnqueue(sql, lines, getRowData);
        }

        internal void requestUpdate()
        {
            string SQL = getSQL();
            List<LINE> lines = new List<LINE>();

            if (filter.LINE_1)
            {
                lines.Add(LINE.FINAL_1);
            }
            if (filter.LINE_2)
            {
                lines.Add(LINE.FINAL_2);
            }

            oracleManager.getOracleDataEnqueue(SQL, lines, updateData);
        }
        internal void getRowData(List<(LINE, int, string, int)> data)
        {
            string theStationCode = "";
            DateTime theDate;

            DailySequences.Clear();

            List<SequenceCount> tempSeq = new List<SequenceCount>();

            foreach (var item in data)
            {
                try
                {
                    theStationCode = AppConstants.Lines[item.Item1];
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                theDate = DateTime.ParseExact(item.Item2.ToString(), "yyyyMMddHH", System.Globalization.CultureInfo.InvariantCulture);

                DailySequences.Add(new SequenceCount(theStationCode, theDate, item.Item3, item.Item4));
            }

            Console.WriteLine("RowData Updated -- Count: " + DailySequences.Count);
            RowDataUpdated?.Invoke(this, EventArgs.Empty);
        }
        internal void updateData(List<(LINE, int ,string, int)> data)
        {
            string theStationCode = "";
            DateTime theDate;

            Clear();



            foreach (var item in data)
            {
                try
                {
                    theStationCode = AppConstants.Lines[item.Item1];
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                theDate = DateTime.ParseExact(item.Item2.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                Sequences.Add(new SequenceCount(theStationCode, theDate, item.Item3, item.Item4));
            }

            DataUpdated?.Invoke(this, EventArgs.Empty);
        }

        internal string getRowSQL(SequenceCountResult result)
        {
 
            string SQL = @"";
            string SELECT = "SELECT " + "SUBSTR(REPORTDATE, 0, 10) as PRODDATE, " + OracleAttributeHelper.GetColumnName<SequenceCountResult>("Line1_CarType_1") + ", Count(*) as COUNT"; 
            string FROM = " FROM " + OracleAttributeHelper.GetTableName<SequenceCountResult>();
            string WHERE = " WHERE " + OracleAttributeHelper.GetColumnName<SequenceCountResult>("Date") + " IN " + "'" + convertDate(result.Date) + "'";
            string GROUPBY = " GROUP BY " + OracleAttributeHelper.GetColumnName<SequenceCountResult>("Date") + ", " + OracleAttributeHelper.GetColumnName<SequenceCountResult>("Line1_CarType_1") + ", SUBSTR(REPORTDATE, 0, 10)";

            SQL += SELECT + FROM + WHERE + GROUPBY;

            Console.WriteLine(SQL);

            return SQL;
        }
        internal string getSeqCountInitSQL()
        {

            string SQL = @"";
            string SELECT = "SELECT ";
            string FROM = " FROM " + OracleAttributeHelper.GetIndexTable<SequenceCount>("Line_1_CARTYPE");
            string ORDERBY = " ORDER BY " + OracleAttributeHelper.GetIndexColumns<SequenceCount>("Line_1_CARTYPE")[1] + " ASC";

            List<string> theStrings = OracleAttributeHelper.GetIndexColumns<SequenceCount>("Line_1_CARTYPE");

            foreach(string theString in theStrings)
            {
                if(theString != theStrings.First())
                {
                    SELECT += ", ";
                }

                SELECT += theString;
            }

            SQL += SELECT + FROM + ORDERBY;

            return SQL;
        }
        internal string getSQL()
        {

            string START_DATE = convertDate(filter.startDate);
            string END_DATE = convertDate(filter.endDate); 
            string DATE = OracleAttributeHelper.GetColumnName<SequenceCount>("Date") + " BETWEEN " + START_DATE + " AND " + END_DATE;

            string SQL = @"";

            string SELECT = " SELECT " + OracleAttributeHelper.GetColumnName<SequenceCount>("Date") + ", " + OracleAttributeHelper.GetColumnName<SequenceCount>("Count") + ", " + "Count( * ) as COUNT";
            string FROM = " FROM " + OracleAttributeHelper.GetTableName<SequenceCount>();
            string WHERE = " WHERE " + DATE;
            string GROUPBY = " GROUP BY " + OracleAttributeHelper.GetColumnName<SequenceCount>("Date") + ", " + OracleAttributeHelper.GetColumnName<SequenceCount>("Count");
            string ORDERBY = " ORDER BY " + OracleAttributeHelper.GetColumnName<SequenceCount>("Date") + " DESC";

            SQL += SELECT + FROM + WHERE + GROUPBY + ORDERBY;

            return SQL;
        }

        internal string convertDate(DateTime date)
        {   
            if(date == null)
            {
                throw new Exception("Date cannot be null");
            }
            string YYYYMMDD = date.ToString("yyyyMMdd");
            return YYYYMMDD;
        }

        internal List<SequenceCountResult> GetFilteredData(List<SequenceCount> sequences)
        {
            var line1 = sequences.Where(seq => seq.StationCode == "P370")
                                .OrderByDescending(seq => seq.Date)
                                .ToList();
            var line2 = sequences.Where(seq => seq.StationCode == "P375")
                                .OrderByDescending(seq => seq.Date)
                                .ToList();


            var line1_cartype1 = line1.Where(seq => seq.CarType == AppConstants.CarTypes_Line_1[1])
                                        .OrderByDescending(seq => seq.Date)
                                        .ToList();
            var line1_cartype2 = line1.Where(seq => seq.CarType == AppConstants.CarTypes_Line_1[2])
                                        .OrderByDescending(seq => seq.Date)
                                        .ToList();
            var line1_cartype3 = line1.Where(seq => seq.CarType == AppConstants.CarTypes_Line_1[3])
                                        .OrderByDescending(seq => seq.Date)
                                        .ToList();

            var line2_cartype1 = line2.Where(seq => seq.CarType == AppConstants.CarTypes_Line_2[1])
                                       .OrderByDescending(seq => seq.Date)
                                       .ToList();
            var line2_cartype2 = line2.Where(seq => seq.CarType == AppConstants.CarTypes_Line_2[2])
                                        .OrderByDescending(seq => seq.Date)
                                        .ToList();
            var line2_cartype3 = line2.Where(seq => seq.CarType == AppConstants.CarTypes_Line_2[3])
                                        .OrderByDescending(seq => seq.Date)
                                        .ToList();

            var result = line1_cartype1

            .Select(x => new
            {
                Date = x.Date,
                Line1_Car1 = (int?)x.Count,
                Line1_Car2 = (int?)null,
                Line1_Car3 = (int?)null,
                Line2_Car1 = (int?)null,
                Line2_Car2 = (int?)null,
                Line2_Car3 = (int?)null
            })
            .Concat(line1_cartype2.Select(x => new
            {
                Date = x.Date,
                Line1_Car1 = (int?)null,
                Line1_Car2 = (int?)x.Count,
                Line1_Car3 = (int?)null,
                Line2_Car1 = (int?)null,
                Line2_Car2 = (int?)null,
                Line2_Car3 = (int?)null
            }))
            .Concat(line1_cartype3.Select(x => new
            {
                Date = x.Date,
                Line1_Car1 = (int?)null,
                Line1_Car2 = (int?)null,
                Line1_Car3 = (int?)x.Count,
                Line2_Car1 = (int?)null,
                Line2_Car2 = (int?)null,
                Line2_Car3 = (int?)null
            }))
            .Concat(line2_cartype1.Select(x => new
            {
                Date = x.Date,
                Line1_Car1 = (int?)null,
                Line1_Car2 = (int?)null,
                Line1_Car3 = (int?)null,
                Line2_Car1 = (int?)x.Count,
                Line2_Car2 = (int?)null,
                Line2_Car3 = (int?)null
            }))
            .Concat(line2_cartype2.Select(x => new
            {
                Date = x.Date,
                Line1_Car1 = (int?)null,
                Line1_Car2 = (int?)null,
                Line1_Car3 = (int?)null,
                Line2_Car1 = (int?)null,
                Line2_Car2 = (int?)x.Count,
                Line2_Car3 = (int?)null
            }))
            .Concat(line2_cartype3.Select(x => new
            {
                Date = x.Date,
                Line1_Car1 = (int?)null,
                Line1_Car2 = (int?)null,
                Line1_Car3 = (int?)null,
                Line2_Car1 = (int?)null,
                Line2_Car2 = (int?)null,
                Line2_Car3 = (int?)x.Count
            }))
            .GroupBy(x => x.Date)
            .Select(g => new SequenceCountResult
            {
                Date = g.Key,
                Line1_CarType_1 = g.FirstOrDefault(x => x.Line1_Car1.HasValue)?.Line1_Car1,
                Line1_CarType_2 = g.FirstOrDefault(x => x.Line1_Car2.HasValue)?.Line1_Car2,
                Line1_CarType_3 = g.FirstOrDefault(x => x.Line1_Car3.HasValue)?.Line1_Car3,
                Line2_CarType_1 = g.FirstOrDefault(x => x.Line2_Car1.HasValue)?.Line2_Car1,
                Line2_CarType_2 = g.FirstOrDefault(x => x.Line2_Car2.HasValue)?.Line2_Car2,
                Line2_CarType_3 = g.FirstOrDefault(x => x.Line2_Car3.HasValue)?.Line2_Car3,
            })
            .OrderByDescending(x => x.Date)
            .ToList();

            return result;
        }

        internal bool hasProdDate(DateTime theDate)
        {
            bool hasDate = false;

            foreach(SequenceCount count in Sequences)
            {
                if(count.Date == theDate)
                {
                    hasDate = true;
                    break;
                }
            }
            return hasDate;
        }

        internal void addSequence(DateTime theDate, string theStationCode, string theCarType, int Count)
        {
            foreach (SequenceCount count in Sequences)
            {
                if (count.Date == theDate)
                {
                    break;
                }
            }
        }
    }
}
