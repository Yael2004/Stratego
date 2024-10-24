using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Data.DTO
{
    [DataContract]
    public class PlayerStatisticsResponse
    {
        [DataMember]
        public OperationResult Result { get; set; }

        [DataMember]
        public PlayerStatisticsDTO Statistics { get; set; }
    }

}
