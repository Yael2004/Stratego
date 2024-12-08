using StrategoServices.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class PingService : IPingService
    {
        public bool Ping()
        {
            return true;
        }
    }
}
