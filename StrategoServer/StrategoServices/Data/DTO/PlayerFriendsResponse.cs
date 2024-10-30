using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Data.DTO
{
    [DataContract]
    public class PlayerFriendsResponse
    {
        public PlayerFriendsResponse()
        {
            Friends = new List<PlayerInfoShownDTO>();
        }

        [DataMember]
        public OperationResult Result { get; set; }

        [DataMember]
        public List<PlayerInfoShownDTO> Friends { get; set; }
    }
}
