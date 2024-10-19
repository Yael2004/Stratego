using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Services.Interfaces
{
    [ServiceContract(CallbackContract = typeof(IChatServiceCallback))]
    public interface IChatService
    {
        [OperationContract]
        void Connect(int userId, string username);

        [OperationContract(IsOneWay = true)]
        void SendMessage(int userId, string username, string message);

        [OperationContract(IsOneWay = true)]
        void Disconnect(int userId);
    }
}
