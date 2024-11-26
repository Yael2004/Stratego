using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Data.DTO
{
    [DataContract]
    public class MovementInstructionDTO
    {
        [DataMember]
        public int InitialX { get; set; }

        [DataMember]
        public int InitialY { get; set; }

        [DataMember]
        public int FinalX { get; set; }

        [DataMember]
        public int FinalY { get; set; }

        [DataMember]
        public int DefenderId { get; set; }
        [DataMember]
        public string Result { get; set; }

        public MovementInstructionDTO()
        {
            DefenderId = 0;
            Result = string.Empty;
        }
    }

}
