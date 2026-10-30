using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SequenceTracker
{
    internal class OracleManager
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
                //isConnected = false;

                ConnectionString_F1 = ConfigurationManager.ConnectionStrings["Oracle_F1"].ConnectionString;
                ConnectionString_F2 = ConfigurationManager.ConnectionStrings["Oracle_F2"].ConnectionString;
                ConnectionString_T1 = ConfigurationManager.ConnectionStrings["Oracle_T1"].ConnectionString;
                ConnectionString_T2 = ConfigurationManager.ConnectionStrings["Oracle_T2"].ConnectionString;
                ConnectionString_D1 = ConfigurationManager.ConnectionStrings["Oracle_D1"].ConnectionString;
                ConnectionString_D2 = ConfigurationManager.ConnectionStrings["Oracle_D2"].ConnectionString;
            }
            internal OracleManager(MainWindow theWindow) : this()
            {
                mainWindow = theWindow;
            }

            //Initialization
            //---------------------------------------------
            internal void Init()
            {
                if (mainWindow == null)
                {
                    throw new Exception("MainWindow is null");
                }

                dataManager = mainWindow.dataManager;
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
            internal void getClassIndexEnqueue(string table, List<OracleIndexData> index, Action<List<OracleIndexData>> callback)
            {
                Action task = () =>
                {
                    List<OracleIndexData> data = new List<OracleIndexData>();

                    data.AddRange(getClassIndex(table, index, LINE.FINAL_1));
                    data.AddRange(getClassIndex(table, index, LINE.FINAL_2));
                    data.AddRange(getClassIndex(table, index, LINE.TRIM_1));
                    data.AddRange(getClassIndex(table, index, LINE.TRIM_2));
                    data.AddRange(getClassIndex(table, index, LINE.DOOR_1));
                    data.AddRange(getClassIndex(table, index, LINE.DOOR_2));

                    callback(data);
                };

                addTask(task);
            }

            internal void getOracleDataEnqueue(string table, List<OracleIndexData> index, List<PrimaryKey> primaryKey, Action<List<List<object>>> callback)
            {
                Action task = () =>
                {
                    List<List<object>> data = new List<List<object>>();

                    data.AddRange(getOracleData(table, index, LINE.FINAL_1, primaryKey));
                    data.AddRange(getOracleData(table, index, LINE.FINAL_2, primaryKey));
                    data.AddRange(getOracleData(table, index, LINE.TRIM_1, primaryKey));
                    data.AddRange(getOracleData(table, index, LINE.TRIM_2, primaryKey));
                    data.AddRange(getOracleData(table, index, LINE.DOOR_1, primaryKey));
                    data.AddRange(getOracleData(table, index, LINE.DOOR_2, primaryKey));

                    callback(data);
                };

                addTask(task);
            }

            //Methods
            //-----------------------------------------------
            internal List<OracleIndexData> getClassIndex(string table, List<OracleIndexData> index, LINE line)
            {
                List<OracleIndexData> returnData = new List<OracleIndexData>();

                string ConnectionString = getConnectionString(line);

                try
                {
                    using (OracleConnection connection = new OracleConnection(ConnectionString))
                    {
                        connection.Open();

                        using (OracleCommand command = connection.CreateCommand())
                        {
                            command.CommandText = getIndexSQL(table, index);

                            using (OracleDataReader reader = command.ExecuteReader())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    OracleIndexData newData = new OracleIndexData(reader.GetName(i), reader.GetFieldType(i));

                                    returnData.Add(newData);
                                }
                            }
                        }

                    }
                }
                catch (OracleException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return returnData;
            }

            internal List<List<object>> getOracleData(string table, List<OracleIndexData> index, LINE line, List<PrimaryKey> primaryKey)
            {
                List<List<object>> newData = new List<List<object>>();

                string ConnectionString = getConnectionString(line);

                try
                {
                    using (OracleConnection connection = new OracleConnection(ConnectionString))
                    {
                        connection.Open();

                        using (OracleCommand command = connection.CreateCommand())
                        {
                            command.CommandText = getCommandText(table, index, primaryKey);

                            using (OracleDataReader reader = command.ExecuteReader())
                            {
                                int j = 0;
                                while (reader.Read())
                                {
                                    newData.Add(new List<object>());
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        newData[j].Add(reader.GetValue(i));
                                    }
                                    j++;
                                }
                            }
                        }
                    }

                }
                catch (OracleException ex)
                {
                    Console.WriteLine(ex.Message);
                }


                return newData;
            }

            internal string getIndexSQL(string table, List<OracleIndexData> index)
            {
                string indexSQL = @"";

                string SELECT = "SELECT ";
                string FROM = "FROM " + table;

                int i = 0;

                foreach (OracleIndexData data in index)
                {
                    SELECT = SELECT + data.Column;
                    if (i < index.Count - 1)
                    {
                        SELECT += ", ";
                    }

                    i++;
                }

                indexSQL += SELECT + " " + FROM;

                return indexSQL;
            }

            internal string getCommandText(string table, List<OracleIndexData> index, List<PrimaryKey> primaryKey)
            {
                string CommandText = @"";
                string SELECT = "SELECT ";
                string FROM = "FROM " + table;
                string WHERE = "WHERE ";

                int i = 0;

                foreach (OracleIndexData data in index)
                {
                    SELECT = SELECT + data.Column;
                    if (i < index.Count - 1)
                    {
                        SELECT += ", ";
                    }

                    i++;
                }

                i = 0;

                foreach (PrimaryKey key in primaryKey)
                {
                    WHERE += key.Column + " in ('" + key.Value.ToString() + "')";

                    if (i < primaryKey.Count - 1)
                    {
                        WHERE += " OR ";
                    }

                    i++;
                }

                CommandText += SELECT + " " + FROM + " " + WHERE;

                return CommandText;
            }

            internal string getConnectionString(LINE line)
            {
                string ConnectionString = ";";

                switch (line)
                {
                    case LINE.FINAL_1:
                        ConnectionString = ConnectionString_F1;
                        break;
                    case LINE.FINAL_2:
                        ConnectionString = ConnectionString_F2;
                        break;
                    case LINE.TRIM_1:
                        ConnectionString = ConnectionString_T1;
                        break;
                    case LINE.TRIM_2:
                        ConnectionString = ConnectionString_T2;
                        break;
                    case LINE.DOOR_1:
                        ConnectionString = ConnectionString_D1;
                        break;
                    case LINE.DOOR_2:
                        ConnectionString = ConnectionString_D2;
                        break;
                    default:
                        throw new Exception("Invalid Line Inforamtion");
                }

                return ConnectionString;
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
}
