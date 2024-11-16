using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Data.DTO
{
    [DataContract]
    public class GameSessionCreatedResponse
    {
        [DataMember]
        public int GameId { get; set; }

        [DataMember]
        public OperationResult OperationResult { get; set; }
    }
}
