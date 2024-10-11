using System.ServiceModel;

namespace StrategoServices.Interfaces.StrategoServices
{
    [ServiceContract]
    public interface IChatServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(string username, string message);
    }
}