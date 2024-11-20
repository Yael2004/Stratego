using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class Log<T>
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(T));

        static Log()
        {
            var logRepository = LogManager.GetRepository();
            var configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");

            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException($"El archivo de configuración 'log4net.config' no fue encontrado en {configFile}");
            }

            XmlConfigurator.Configure(logRepository, new FileInfo(configFile));
        }

        public static ILog GetLogger() => _logger;
    }
}
