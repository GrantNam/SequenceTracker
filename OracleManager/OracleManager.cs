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
        }

        //Thread Control
        //-----------------------------------------------
        internal void start()
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

        internal void getOracleDataEnqueue(string SQL, List<LINE> lines, Action<List<(int, int, LINE)>> callback)
        {
            Action task = () =>
            {
                List<(int, int, LINE)> data = new List<(int, int, LINE)>();

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
        internal List<(int, int, LINE)> getOracleData(string SQL, LINE line)
        {
            List<(int, int, LINE)> data = new List<(int, int, LINE)>();

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
                            int seqCount = 0;
                            int prodDate = 0;

                            while(reader.Read())
                            {
                                seqCount = Convert.ToInt32(reader["Count"]);
                                prodDate = Convert.ToInt32(reader["PRODDATE"]);
                                data.Add((seqCount, prodDate, line));
                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                Console.WriteLine("Unexpected Error While Querrying Data");
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
