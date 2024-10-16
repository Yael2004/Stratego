using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoApp.Helpers
{
    public static class Log<T>
    {
        private static ILog _logger = LogManager.GetLogger(typeof(T));

        static Log()
        {
            var logRepository = LogManager.GetRepository();
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
         
        public static ILog GetLogger() { return _logger; }

        public static void LogInfo(string message)
        {
            _logger.Info(message);
        }

        public static void LogError(string message)
        {
            _logger.Error(message);
        }

        public static void LogWarning(Exception ex)
        {
            _logger.Warn(ex.Message, ex);
        }
    }
}
