using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerTree.UnitTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var t1 = new OrderedSequenceUnitTests();
            t1.TestCharOrderedSequence();
            t1.TestStringOrderedSequence();

            Console.ReadKey();
        }
    }
}
