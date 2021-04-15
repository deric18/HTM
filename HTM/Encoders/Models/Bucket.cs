using HTM.Models;

namespace HTM.Encoders.Models
{
    public class Bucket
    {
        uint _bucketIndex;
        Position2D _startIndex;
        Position2D _endX;
        Position2D _endY;
        Position2D _endXY;
        Position2D _center;

        public Bucket(uint bucketId, Position2D startIndex, uint radius, uint? buffer = 0)
        {
            _bucketIndex = bucketId;
            _endX = new Position2D(startIndex.X + radius, startIndex.Y);
            _endY = new Position2D(startIndex.X, startIndex.Y + radius);
            _endXY = new Position2D(startIndex.X + radius, startIndex.Y + radius);                        



        }

        private void ComputeCenter()
        {

        }
    }
}
