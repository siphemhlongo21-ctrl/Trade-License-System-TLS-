using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Timers;
using TradeLicenseWinService;

namespace C8.Windows.Service
{
    public partial class TradeLicenseService : ServiceBase
    {
        static Timer _timer;
        static string _ScheduledRunningTime = "07:00 AM";

        public TradeLicenseService()
        {
            try
            {
                InitializeComponent();
                ServiceName = "TradeLicense";
            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
            }

        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _eventLog.WriteEntry("Service Started at: " + String.Format("{0:t}", DateTime.Now));

                // Initialises the event log for the system.
                InitialiseEventLog();

                _timer = new Timer();
                _timer.Interval = 60000;
                _timer.Elapsed += OnTimer;
                _timer.Start();
            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
            }
        }




        protected override void OnStop()
        {
            _eventLog.WriteEntry("Service Stopped at: " + String.Format("{0:t}", DateTime.Now));
            //NotificationCore.NotifyAdmin();
        }

        /// <summary>
        /// LM.20150224a - Handles the timer event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.
            //_eventLog.WriteEntry("Monitoring the System...", EventLogEntryType.Information);

            string _CurrentTime = String.Format("{0:t}", DateTime.Now);
            if (_CurrentTime == _ScheduledRunningTime)
            {
                _eventLog.WriteEntry("Processing...");
                Process();
            }
        }

        #region Service State
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public long dwServiceType;
            public ServiceState dwCurrentState;
            public long dwControlsAccepted;
            public long dwWin32ExitCode;
            public long dwServiceSpecificExitCode;
            public long dwCheckPoint;
            public long dwWaitHint;
        };
        #endregion

        /// <summary>
        /// LM.20150224a - Initialises the event log.
        /// </summary>
        private void InitialiseEventLog()
        {
            try
            {
                // Configure loggin of service.
                this.AutoLog = false;
                if (!EventLog.SourceExists("TradeLicense"))
                {
                    EventLog.CreateEventSource("TradeLicense", "Application");
                }
                _eventLog.Source = "TradeLicense";
            }
            catch (Exception x)
            {
                _eventLog.WriteEntry(x.ToString());
                // LogEvent(LogTypeKeys.Error, x.ToString());
            }
        }

        /// <summary>
        ///LM.20150224a- Process all background tasks.
        /// </summary>
        public void Process()
        {
            try
            {
                new NotificationCore().GenerateDailyReminders();
                new FunctionCore().ProcessBackgroundTasks();
            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
            }
        }
    }
}
