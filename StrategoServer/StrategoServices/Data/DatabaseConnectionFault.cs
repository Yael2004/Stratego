using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Data
{
    [DataContract]
    public class DatabaseConnectionFault
    {
        [DataMember]
        public string Message { get; set; }
    }

}
