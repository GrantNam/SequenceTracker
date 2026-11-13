using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace SequenceTracker
{
    internal partial class DataManager
    {
        //Fields
        //-------------------------------------------------------------
        internal ConcurrentQueue<List<SequenceCount>> AutoUpdateBuffer = new ConcurrentQueue<List<SequenceCount>>();
        internal ConcurrentQueue<Filter> AutoUpdateFilter = new ConcurrentQueue<Filter>();

        internal ConcurrentQueue<List<SequenceCount>> AutoUpdateDailyBuffer = new ConcurrentQueue<List<SequenceCount>>();

        //Flags
        //--------------------------------------------------------------
        internal bool threadRunning = false;
        internal bool AutoUpdating = false;

        //Thread
        //--------------------------------------------
        internal Thread AutoUpdateThread;

        //Signals
        //--------------------------------------------
        private static System.Threading.Timer updateTimer;

        internal AutoResetEvent AutoUpdateSignal = new AutoResetEvent(false);
        internal AutoResetEvent AutoUpdateCompleteSignal = new AutoResetEvent(false);

        //Thread Controls
        //---------------------------------------------------------------

        private void TimerCallback(object state)
        {
            AutoUpdateSignal.Set();
        }
        internal static void TimerStart()
        {
            int interval = int.Parse(ConfigurationManager.AppSettings["UpdateInterval"]);
            updateTimer.Change(0, interval);
        }
        internal static void TimerPause()
        {
            updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }
        internal void Start()
        {
            int interval = int.Parse(ConfigurationManager.AppSettings["UpdateInterval"]);

            updateTimer = new System.Threading.Timer(TimerCallback, null, interval, interval);

            AutoUpdateThread = new Thread(threadWork);
            threadRunning = true;
            AutoUpdateThread.Start();
        }
        internal void stop()
        {
            threadRunning = false;
            AutoUpdateThread.Abort();
        }
        internal void threadWork()
        {
            TimerPause();
            while (threadRunning)
            {
                if (DataManager.filter.AutoUpdate)
                {
                    SyncEndDate();
                    AutoUpdate();
                }
                AutoUpdateSignal.WaitOne();
            }
        }

        internal void Signal()
        {
            AutoUpdateSignal.Set();
        }
        //Methods
        //-------------------------------------------------------------------
        
        internal void Clear<T>(ConcurrentQueue<T> someQueue)
        {
            while (someQueue.TryDequeue(out _))
            {
            }
        }
        internal void SyncEndDate()
        {
            DataManager.filter.endDate = mainWindow.searchBar1.EndDate.Value;
        }

        internal void AutoUpdate()
        {
            DailySequenceCountRepo.AutoUpdate();
            HourlySequenceCountRepo.AutoUpdate();
        }
      
    }  
}
