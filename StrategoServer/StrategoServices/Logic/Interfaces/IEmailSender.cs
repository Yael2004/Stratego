using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Logic.Interfaces
{
    internal interface IEmailSender
    {
        void SendEmail(string destinationAddress, string message);
    }
}
