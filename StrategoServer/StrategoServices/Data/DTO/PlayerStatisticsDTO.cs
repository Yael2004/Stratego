using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Data.DTO
{
    [DataContract]
    public class PlayerStatisticsDTO
    {
        public PlayerStatisticsDTO()
        {
            WonGames = 0;
            LostGames = 0;
        }

        [DataMember]
        public int WonGames { get; set; }

        [DataMember]
        public int LostGames { get; set; }

        [DataMember]
        public int TotalGames => WonGames + LostGames;
    }

}
