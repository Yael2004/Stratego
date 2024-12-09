using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Logic.Interfaces
{
    internal interface IEmailSender
    {
        bool SendVerificationEmail(string destinationAddress, string code);
        bool SendInvitationEmail(string destinationAddress, string code);
    }
}
