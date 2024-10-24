using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Data.DTO
{
    [DataContract]
    public class PlayerInfoShownDTO
    {
        public PlayerInfoShownDTO()
        { 
            Name = string.Empty;
            PicturePath = "picture1";
            LabelPath = "label1";
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string PicturePath { get; set; }

        [DataMember]
        public string LabelPath { get; set; }
    }
}
