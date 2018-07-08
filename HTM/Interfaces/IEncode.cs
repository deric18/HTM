using HTM.Models;
using System.Collections.Generic;

namespace HTM.Interfaces
{
    public interface IEncode
    {
        SDR SparsifyInput(List<int> inputs);
    }
}
