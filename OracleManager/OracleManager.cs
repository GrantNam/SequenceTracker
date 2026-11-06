using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
                Console.WriteLine("Thread Running...");
                while (taskQueue.TryDequeue(out task))
                {
                    try
                    {
                        task();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
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

        //Methods
        //-----------------------------------------------

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

        
    }
    internal enum LINE
    {
        FINAL_1 = 1,
        FINAL_2 = 2,
        TRIM_1 = 3,
        TRIM_2 = 4,
        DOOR_1 = 5,
        DOOR_2 = 6
    }
}
