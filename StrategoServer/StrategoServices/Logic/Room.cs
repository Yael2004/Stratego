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
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public bool IsFull { get; set; }
        public List<string> Messages { get; set; }

        public Room()
        {
            Code = string.Empty;
            Player1Id = 0;
            Player2Id = 0;
            IsFull = false;
            Messages = new List<string>();
        }
    }
}
