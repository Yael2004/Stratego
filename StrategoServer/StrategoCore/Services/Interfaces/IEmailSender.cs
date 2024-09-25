using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoCore.Services.Interfaces
{
    internal interface IEmailSender
    {
        void SendEmail(string destinationAddress);
    }
}
