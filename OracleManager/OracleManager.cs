using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace SequenceTracker
{
    internal class OracleManager
    {


        //Managers
        //--------------------------------------------
        internal DataManager dataManager;
        internal MainWindow mainWindow;
        internal EventManager eventManager;

        //Fields
        private string ConnectionString_F1;
        private string ConnectionString_F2;
        private string ConnectionString_T1;
        private string ConnectionString_T2;
        private string ConnectionString_D1;
        private string ConnectionString_D2;


        private readonly ConcurrentQueue<Action> taskQueue = new ConcurrentQueue<Action>();
        //Thread
        //--------------------------------------------
        internal Thread workerThread;

        //Events
        //--------------------------------------------
        AutoResetEvent OrclSignal = new AutoResetEvent(false);

        //Flags
        //--------------------------------------------
        //internal bool isConnected;
        internal bool threadRunning = false;

        //Constructor
        //--------------------------------------------
        public OracleManager()
        {
        }

        //Initialization
        //---------------------------------------------
        internal void Init()
        {
            Start();
        }

        //Thread Control
        //-----------------------------------------------
        internal void Start()
        {
            workerThread = new Thread(threadWork);
            threadRunning = true;
            workerThread.Start();
        }
        internal void stop()
        {
            threadRunning = false;
            workerThread.Abort();
        }
        internal void threadWork()
        {
            Action task;

            while (threadRunning)
            {
                Console.WriteLine("Oracle Thread Running...");
                while (taskQueue.TryDequeue(out task))
                {
                    try
                    {
                        task();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[Oracle Exception]\n" + ex.Message);
                    }
                }

                OrclSignal.WaitOne();
            }
        }

        internal void Signal()
        {
            OrclSignal.Set();
        }

        public void addTask(Action task)
        {
            System.Diagnostics.Debug.WriteLine("addTask: task added");
            if (!threadRunning)
            {
                threadRunning = true;
            }
            taskQueue.Enqueue(task);
            Signal();
        }

        //Task Enqueue
        //------------------------------------------------
        /*
        internal void getSequenceCountInitEnqueue(string SQL, Action<List<string>, List<string>> callback)
        {   
            List<LINE> lines = new List<LINE>();

            lines.Add(LINE.FINAL_1);
            lines.Add(LINE.FINAL_2);

            Action task = () =>
            {
                (List<string>, List<string>) data = (new List<string>(), new List<string>());

                int i = 0;

                data.Item1 = getSequenceCountInit(SQL, lines[0]);
                data.Item2 = getSequenceCountInit(SQL, lines[1]);

                callback(data.Item1, data.Item2);
            };

            addTask(task);
        }
        internal void getOracleDataEnqueue(string SQL, List<LINE> lines, Action<List<(LINE, int, string, int)>> callback)
        {
            Action task = () =>
            {
                List<(LINE, int, string, int)> data = new List<(LINE, int, string, int)>();

                foreach(LINE line in lines)
                {   
                    data.AddRange(getOracleData(SQL, line));
                }

                callback(data);
            };

            addTask(task);
        }
        */
        internal void getDataEnqueue(string SQL, OracleParameter[] parameters, List<LINE> lines, Action<DataTable> callback)
        {
            Action task = () =>
            {
                DataTable callbackData = new DataTable();

                foreach (LINE line in lines)
                {
                    if(callbackData.Rows.Count == 0)
                    {
                        callbackData = getData(SQL,parameters, line);
                    }
                    else
                    {
                        callbackData.Merge(getData(SQL, parameters, line));
                    }
                }

                callback(callbackData);
            };

            addTask(task);
        }

        //Methods
        //-----------------------------------------------
        /*
        internal List<string> getSequenceCountInit(string SQL, LINE line)
        {
            List<string> data = new List<string>();

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings[line.ToString()].ConnectionString;

                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand(SQL, connection))
                    {
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                try
                                {   
                                    if(reader[OracleAttributeHelper.GetIndexColumns<SequenceCount>("Line_1_CARTYPE")[1]] == DBNull.Value)
                                    {
                                        data.Add("Not Configured");
                                        continue;
                                    }
                                    data.Add(reader[OracleAttributeHelper.GetIndexColumns<SequenceCount>("Line_1_CARTYPE")[1]].ToString());
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine("Unexpected Error While Initializing Data");
            }
            return data;
        }

        //Returns List<(LINE, PRODDATE | TIME, CARTYPE, COUNT)>
        internal List<(LINE, int, string, int)> getOracleData(string SQL, LINE line)
        {
            List<(LINE, int, string, int)> data = new List<(LINE, int, string, int)>();

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings[line.ToString()].ConnectionString;

                using(OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    connection.Open();

                    using(OracleCommand command = new OracleCommand(SQL, connection))
                    {
                        using(OracleDataReader reader = command.ExecuteReader())
                        {
                            int Count = 0;
                            string CarType = "";
                            int prodDate = 0;

                            while(reader.Read())
                            {   
                                Count = Convert.ToInt32(reader["COUNT"]);
                                CarType = reader["CARTYPE"].ToString();
                                prodDate = Convert.ToInt32(reader["PRODDATE"]);
                                data.Add((line, prodDate, CarType, Count));
                                CarType = "";
                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Unexpected Error While Querrying Data");
            }
            

            return data;
        }
        */
        internal DataTable getData(string SQL, OracleParameter[] parameters, LINE line)
        {
            DataTable table = new DataTable();

            try
            {
                string ConnectionString = ConfigurationManager.ConnectionStrings[line.ToString()].ConnectionString;

                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {

                    using (OracleCommand command = new OracleCommand(SQL, connection))
                    {
                        command.BindByName = true;
                        command.Parameters.AddRange(parameters);

                        connection.Open();

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            int rowIndex = 0;

                            while (reader.Read())
                            {   
                                if(rowIndex == 0)
                                {
                                    table.Columns.Add("LINE", typeof(LINE));
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        table.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                                    }
                                }

                                DataRow row = table.NewRow();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row["LINE"] = line;
                                    row[reader.GetName(i)] = reader.GetValue(i);
                                }

                                table.Rows.Add(row);

                                rowIndex++;
                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Unexpected Error While Querrying Data");
            }

            return table;
        }

    }
}
