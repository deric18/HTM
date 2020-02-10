using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTM.Models
{
    public static class NumColumnsPerBlock
    {
        public static readonly Dictionary<uint, KeyValuePair<int, int>> ColumnConfigurationPerBlock;
        //Total Number of Points, <Number of Points per Line , Number of lines per dimension>
        static NumColumnsPerBlock()
        {
            ColumnConfigurationPerBlock = new Dictionary<uint, KeyValuePair<int, int>>()
            {
                {1000,      new KeyValuePair<int, int>(100, 10)   },
                {10000,     new KeyValuePair<int, int>(100, 100)  },
                {100000,    new KeyValuePair<int, int>(100, 1000) },
                {1000000,   new KeyValuePair<int, int>(1000,1000) },
                {10000000, new KeyValuePair<int, int>(1000, 10000) }
            };
        }
    }
}
