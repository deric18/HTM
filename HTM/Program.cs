using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTM
{
    class Program
    {
        static void Main(string[] args)
        {
            CPM cpm = CPM.Instance;

            Console.WriteLine("Enter X:");
            int x = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter Y:");
            int y = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter Z:");
            int z = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Initializing...");
            CPM.Initialize(x, y, z);
            Console.WriteLine("Done.");

            //Set Up Spatial Input pattern Source

            //Set Up Temporal Input Pattern Source

            //Set Up Apical Input Pattern Source
        }
    }
}
