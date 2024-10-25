using StrategoServices.Data;
using System.ServiceModel;

namespace StrategoServices.Services.Interfaces
{
    [ServiceContract]
    public interface IChatServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(string username, string message);

        [OperationContract(IsOneWay = true)]
        void ChatResponse(OperationResult result);
    }
}