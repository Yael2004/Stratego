using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Data.DTO
{
    [DataContract]
    public class OtherPlayerInfoDTO
    {
        [DataMember]
        public PlayerInfoShownDTO PlayerInfo { get; set; }
        [DataMember]
        public PlayerStatisticsDTO PlayerStatistics { get; set; }
        [DataMember]
        public bool IsFriend { get; set; }
    }
}
