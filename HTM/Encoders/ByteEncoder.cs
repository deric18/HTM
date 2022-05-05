using HTM.Encoders.Models;
using HTM.Models;
using System;
using System.Collections.Generic;

namespace HTM.Encoders
{
    public class ByteEncoder
    {
        private uint _n;
        private uint _w;        //(total number of buckets to be created)
        private uint _numBuckets;        
        private uint _numCellsPerBucket;
        private SDR _outputSdr;
        private double buffer;
        private List<Bucket> bucketList;
        private uint radius;

        public ByteEncoder(uint minVal, uint maxVal)
        {
            if(maxVal > 256 || minVal > maxVal)
            {
                throw new Exception("Invalid limits");
            }

            _n = CPM.GetInstance.NumX * CPM.GetInstance.NumY;
            _w = 8;     //because its a byteencoder


            if (_n < _w * 25)
                throw new Exception("Potential Space is too small for sample space _n < _w * 25 (Recommendation : _n > 50 * _w)!. ");

            buffer = 0;
            radius = 0;            

        }

        private void Compute()
        {

            buffer = (_n % _w) == 0 ? 0.0 : ((_n / _w) * _n);
            _numCellsPerBucket = _n / _w;
            radius = (uint)Math.Sqrt(_numCellsPerBucket);
            if(radius % 2 == 0)         //to find a better centre
            {
                //try to do rectangular adjustment

            }
            _numBuckets = _n / _numCellsPerBucket;
        }
        

        private void CreateBuckets()
        {
            Position2D startIndex = new Position2D(0, 0);
            
            for(uint i=0; i<_numBuckets; i++)
            {
                bucketList.Add(new Bucket(i, startIndex, radius, (uint)buffer));
            }
        }

        public SDR EncodeByte(byte byteVal)
        {
            return new SDR(10, 10);

        }

        public SDR SparsifyInput(uint inputs)
        {            
            throw new System.NotImplementedException();
        }
    }
}
