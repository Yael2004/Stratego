using StrategoApp.LogInService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StrategoApp.Model
{
    public class Player
    {
        public Player()
        {
            Name = string.Empty;
            PicturePath = "picture1";
            LabelPath = "label1";
            AccountId = 0;

            Friends = new List<Player>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string PicturePath { get; set; }

        public string LabelPath { get; set; }

        public int AccountId { get; set; }

        public List<Player> Friends { get; set; }
    }
}
