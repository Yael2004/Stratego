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
        public int PieceId { get; set; }

        [DataMember]
        public string MoveType { get; set; }

        public PositionDTO(int initialX, int initialY, int finalX, int finalY, int pieceId, string moveType)
        {
            InitialX = initialX;
            InitialY = initialY;
            FinalX = finalX;
            FinalY = finalY;
            PieceId = pieceId;
            MoveType = moveType;
        }

        public PositionDTO() { }
    }
}
