using System.ServiceModel;

namespace StrategoServices.Services.Interfaces
{
    [ServiceContract]
    public interface IChatServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(string username, string message);
    }
}