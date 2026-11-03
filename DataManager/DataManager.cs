using System;
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

        internal static Filter filter = new Filter();

        //Events
        //---------------------------------------------------------------
        public event EventHandler DataUpdated;

        //Event Handlers
        //---------------------------------------------------------------
        internal void OnSerchRequested(object sender, EventArgs e)
        {
            requestUpdate();
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
        }

        internal void SubscribeEvents()
        {
            mainWindow.SearchRequested += OnSerchRequested;
        }
        //Methods
        //---------------------------------------------------------------
        internal void Clear()
        {
            Sequences.Clear();
        }



        internal void requestUpdate()
        {
            Clear();

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

        internal void updateData(List<(int, int, LINE)> data)
        {
            string theStationCode = "";
            DateTime theDate;

            foreach (var item in data)
            {
                if (item.Item3 == LINE.FINAL_1 || item.Item3 == LINE.TRIM_1 || item.Item3 == LINE.DOOR_1)
                {
                    theStationCode = "P370";
                }
                else if (item.Item3 == LINE.FINAL_2 || item.Item3 == LINE.TRIM_2 || item.Item3 == LINE.DOOR_2)
                {
                    theStationCode = "P375";
                }
                else
                {
                    throw new Exception("Invalid line information");
                }
                    theDate = DateTime.ParseExact(item.Item2.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                Sequences.Add(new SequenceCount(theStationCode, theDate, item.Item1));
            }

            DataUpdated?.Invoke(this, EventArgs.Empty);
        }

        internal string getSQL()
        {   
            string START_DATE = convertDate(filter.startDate);
            string END_DATE = convertDate(filter.endDate); 
            string DATE = OracleAttributeHelper.GetColumnName<SequenceCount>("Date") + " BETWEEN " + START_DATE + " AND " + END_DATE;

            string SQL = @"";

            string SELECT = " SELECT Count(" + OracleAttributeHelper.GetColumnName<SequenceCount>("Count") + ") as Count, " + OracleAttributeHelper.GetColumnName<SequenceCount>("Count");
            string FROM = " FROM " + OracleAttributeHelper.GetTableName<SequenceCount>();
            string WHERE = " WHERE " + DATE;
            string GROUPBY = " GROUP BY " + OracleAttributeHelper.GetColumnName<SequenceCount>("Count");

            SQL += SELECT + FROM + WHERE + GROUPBY;

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

        internal List<SequenceCountResult> GetFilteredData()
        {
            var line1 = Sequences.Where(seq => seq.StationCode == "P370")
                                .OrderByDescending(seq => seq.Date)
                                .ToList();
            var line2 = Sequences.Where(seq => seq.StationCode == "P375")
                                .OrderByDescending(seq => seq.Date)
                                .ToList();

            var result = line1

            .Select(x => new
            {
                Date = x.Date,
                Line1 = (int?)x.Count,
                Line2 = (int?)null
            })
            .Concat(line2.Select(x => new
            {
                Date = x.Date,
                Line1 = (int?)null,
                Line2 = (int?)x.Count
            }))
            .GroupBy(x => x.Date)
            .Select(g => new SequenceCountResult
            {
                Date = g.Key,
                Line1 = g.FirstOrDefault(x => x.Line1.HasValue)?.Line1,
                Line2 = g.FirstOrDefault(x => x.Line2.HasValue)?.Line2
            })
            .OrderByDescending(x => x.Date)
            .ToList();

            return result;
        }
    }
}
