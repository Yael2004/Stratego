using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Data.DTO
{
    [DataContract]
    public class FinalStatsDTO
    {
        [DataMember]
        public int AccountId { get; set; }

        [DataMember]
        public int PlayerId { get; set; }

        [DataMember]
        public bool HasWon { get; set; }

        [DataMember]
        public int GameId { get; set; }

        public FinalStatsDTO()
        {
            AccountId = 0;
            PlayerId = 0;
            this.HasWon = false;
            GameId = 0;
        }
    }
}
