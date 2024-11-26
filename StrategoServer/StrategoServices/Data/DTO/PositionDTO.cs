using System.Runtime.Serialization;

namespace StrategoServices.Data.DTO
{
    [DataContract]
    public class PositionDTO
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
        public int PowerLevel { get; set; }

        [DataMember]
        public string PieceName { get; set; }

        public PositionDTO(int initialX, int initialY, int finalX, int finalY, int powerLevel, string pieceName)
        {
            InitialX = initialX;
            InitialY = initialY;
            FinalX = finalX;
            FinalY = finalY;
            PowerLevel = powerLevel;
            PieceName = pieceName;
        }

        public PositionDTO()
        {
            InitialX = -1;
            InitialY = -1;
            FinalX = -1;
            FinalY = -1;
            PowerLevel = -1;
            PieceName = string.Empty;
        }
    }
}
