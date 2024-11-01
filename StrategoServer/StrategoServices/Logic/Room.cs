using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Logic
{
    public class Room
    {
        public string Code { get; set; }
        public string Player1Id { get; set; }
        public string Player2Id { get; set; }
        public bool IsFull { get; set; }
        public List<string> Messages { get; set; }

        public Room()
        {
            IsFull = false;
            Messages = new List<string>();
        }
    }
}
