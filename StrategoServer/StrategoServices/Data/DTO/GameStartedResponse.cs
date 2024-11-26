using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Data.DTO
{
    [DataContract]
    public class GameStartedResponse
    {
        [DataMember]
        public bool IsStarter { get; set; }

        [DataMember]
        public OperationResult OperationResult { get; set; }

        public GameStartedResponse(bool isStarter, OperationResult operationResult)
        {
            IsStarter = isStarter;
            OperationResult = operationResult;
        }
    }
}
