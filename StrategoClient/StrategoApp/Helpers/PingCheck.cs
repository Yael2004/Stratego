using log4net;
using StrategoApp.PingService;
using StrategoApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoApp.Helpers
{
    public class PingCheck
    {
        private static readonly ILog Log = Log<PingCheck>.GetLogger();
        private static bool _isMonitoring;
        private readonly MainWindowViewModel _mainWindowViewModel;

        public PingCheck(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public async Task StartPingMonitoringAsync()
        {
            if (_isMonitoring)
            {
                Log.Warn("Ping monitoring is already running.");
                return;
            }

            _isMonitoring = true;

            while (_isMonitoring)
            {
                try
                {
                    bool isServerAlive = CheckPing();

                    if (!isServerAlive)
                    {
                        Log.Warn("Ping failed. Server might be down.");
                        _mainWindowViewModel.ChangeViewModel(new LogInViewModel(_mainWindowViewModel, true));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error during ping check", ex);
                    _mainWindowViewModel.ChangeViewModel(new LogInViewModel(_mainWindowViewModel, true));
                }

                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }

        public bool CheckPing()
        {
            PingServiceClient pingService = null;
            try
            {
                pingService = new PingServiceClient();
                var result = pingService.Ping();
                return result;
            }
            catch (CommunicationException cex)
            {
                Log.Error("CommunicationException in PingCheck", cex);
                return false;
            }
            catch (TimeoutException tex)
            {
                Log.Error("TimeoutException in PingCheck", tex);
                return false;
            }
            catch (Exception ex)
            {
                Log.Error("Exception in PingCheck", ex);
                return false;
            }
            finally
            {
                if (pingService != null)
                {
                    try
                    {
                        if (pingService.State == CommunicationState.Faulted)
                        {
                            Log.Warn("PingService is faulted. Aborting the client.");
                            pingService.Abort();
                        }
                        else
                        {
                            pingService.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error while closing or aborting PingService client", ex);
                        pingService.Abort();
                    }
                }
            }
        }

        public void StopPingMonitoring()
        {
            _isMonitoring = false;
            Log.Info("Ping monitoring stopped.");
        }
    }
}
