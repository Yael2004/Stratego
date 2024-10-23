using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace StrategoServices.Data.DTO
{
    [DataContract]
    public class PlayerDTO
    {
        public PlayerDTO()
        {
            Name = string.Empty;
            PicturePath = "picture1";
            LabelPath = "label1";
            AccountId = 0;

            Friends = new List<PlayerDTO>();
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string PicturePath { get; set; }

        [DataMember]
        public string LabelPath { get; set; }

        [DataMember]
        public int AccountId { get; set; }

        [DataMember]
        public List<PlayerDTO> Friends { get; set; }
    }
}

