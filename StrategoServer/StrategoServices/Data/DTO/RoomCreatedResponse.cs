using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Data.DTO
{
    [DataContract]
    public class RoomCreatedResponse
    {
        [DataMember]
        public string RoomCode { get; set; }
        [DataMember]
        public OperationResult Result { get; set; }

        public RoomCreatedResponse()
        {
            RoomCode = string.Empty;
        }
    }
}
