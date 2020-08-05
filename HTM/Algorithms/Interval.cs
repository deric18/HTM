using HTM.Models;

namespace HTM.Algorithms
{
    internal class Interval
    {
        private int x1;
        private int x2;
        private int y1;
        private int y2;
        bool xDone, yDone, zDone;           //Done this way for points whose RSB is crossing over for all the 3 dimensions
        uint X, Y, Z;
        private int seed;

        public Interval(int x1, int x2, int y1, int y2)
        {            
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
            xDone = yDone = zDone = false;
            X = Y = Z = 0;
            seed = 7;
        }                               

        public void UpdateValues(int x1, int x2, int y1, int y2)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
        }

        public void PerformOperationX()
        {
            if (xDone)
                throw new System.Exception("Invalid Operation");
            X = SynapseGeneratorHelper.PredictRandomIntervalInteger(x1, x2, y1, y2);
            xDone = true;
        }

        public void PerformOpearationY(int x1, int x2, int y1, int y2)
        {
            UpdateValues(x1, x2, y1, y2);
            if (yDone)
                throw new System.Exception("Invalid Operation");
            Y = SynapseGeneratorHelper.PredictRandomIntervalInteger(x1, x2, y1, y2);
            yDone = true;
        }

        public void PerformOpearationZ(int x1, int x2, int y1, int y2)
        {
            UpdateValues(x1, x2, y1, y2);
            if (zDone)
                throw new System.Exception("Invalid Operation");
            Z = SynapseGeneratorHelper.PredictRandomIntervalInteger(x1, x2, y1, y2);
            zDone = true;
        }
        

        public Position3D GetNewPosition() =>        
            (xDone && yDone && zDone) ? new Position3D(X, Y, Z) : null;        
    }
}
