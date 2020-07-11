using Codeable.Foundation.Common;
using Codeable.Foundation.Common.Aspect;
using Codeable.Foundation.Common.Daemons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Codeable.Foundation.Core.Unity
{
    public class ExpireStaticLifetimeDaemon : ChokeableClass, IDaemonTask
    {
        public ExpireStaticLifetimeDaemon(IFoundation iFoundation)
            : base(iFoundation)
        {
        }

        #region Statics

        private static object _DaemonRoot = new object();
        private static IDaemonTask _DaemonInstance;

        public static void EnsureDaemon()
        {
            try
            {
                if (_DaemonInstance == null)
                {
                    lock (_DaemonRoot)
                    {
                        if (_DaemonInstance == null)
                        {
                            IFoundation iFoundation = CoreFoundation.Current;
                            IDaemonManager daemonManager = iFoundation.GetDaemonManager();
                            IDaemonTask daemonTask = daemonManager.GetRegisteredDaemonTask(DAEMON_NAME);

                            if (daemonTask == null)
                            {
                                ExpireStaticLifetimeDaemon daemon = new ExpireStaticLifetimeDaemon(iFoundation);
                                DaemonConfig config = new DaemonConfig()
                                {
                                    InstanceName = DAEMON_NAME,
                                    ContinueOnError = true,
                                    IntervalMilliSeconds = 15000, // clean up every 15 seconds
                                    StartDelayMilliSeconds = 15000,
                                    TaskConfiguration = string.Empty
                                };

                                daemonManager.RegisterDaemon(config, daemon, true);
                                _DaemonInstance = daemon;
                            }
                            else
                            {
                                // should be impossible [but other bad-actors]
                                _DaemonInstance = daemonTask;
                            }
                        }
                    }
                }
            }
            catch
            {
                // gulp
            }
        }

        #endregion

        #region IDaemonTask Members

        public const string DAEMON_NAME = "ExpireStaticLifetimeDaemon";

        protected static bool _executing;

        public string DaemonName
        {
            get
            {
                return DAEMON_NAME;
            }
            protected set
            {
            }
        }
        public void Dispose()
        {
        }
        public void Execute(Codeable.Foundation.Common.IFoundation iFoundation, CancellationToken token)
        {
            base.ExecuteMethod("Execute", delegate ()
            {
                if (_executing) { return; } // safety

                try
                {
                    _executing = true;
                    this.CleanLifetime();
                }
                finally
                {
                    _executing = false;
                }
            });
        }

        public DaemonSynchronizationPolicy SynchronizationPolicy
        {
            get { return DaemonSynchronizationPolicy.SingleAppDomain; }
        }


        #endregion

        #region Protected Methods

        protected void CleanLifetime()
        {
            base.ExecuteMethod("CleanLifetime", delegate ()
            {
                try
                {
                    ExpireStaticLifetimeManager.CleanExpiredValues();
                }
                catch
                {
                    // gulp
                }
            });
        }

        #endregion

    }

}
