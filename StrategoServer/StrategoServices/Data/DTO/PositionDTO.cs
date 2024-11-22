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

        [DataMember]
        public string MoveType { get; set; }

        public PositionDTO(int initialX, int initialY, int finalX, int finalY, int powerLevel, string pieceName, string moveType)
        {
            InitialX = initialX;
            InitialY = initialY;
            FinalX = finalX;
            FinalY = finalY;
            PowerLevel = powerLevel;
            PieceName = pieceName;
            MoveType = moveType;
        }

        public PositionDTO()
        {
            InitialX = -1;
            InitialY = -1;
            FinalX = -1;
            FinalY = -1;
            PowerLevel = -1;
            PieceName = string.Empty;
            MoveType = string.Empty;
        }
    }
}
