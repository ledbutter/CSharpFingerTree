using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerTree.UnitTests
{
    public class OrderedSequenceUnitTests
    {
        public void TestCharOrderedSequence()
        {
            var os = new OrderedSequence<char, uint>(
                new Key<char, uint>(uint.MinValue, (c) => { return c; }));

            var src = new List<char>
            {
                'z',
                'b',
                'h',
                'a',
                'a',
                'c',
                'e',
                'j'
            };

            foreach (var s in src)
                os = os.Insert(s);

            var expected = new List<char>
            {
                'a',
                'a',
                'b',
                'c',
                'e',
                'h',
                'j',
                'z',
            };

            var tseq = os.ToSequence();

            var tseqEnum = tseq.GetEnumerator();
            tseqEnum.MoveNext();

            foreach (var es in expected)
            {
                if (0 != es.CompareTo(tseqEnum.Current))
                    throw new Exception("TestCharOrderedSequence failed.");
                tseqEnum.MoveNext();
            }
        }

        public void TestStringOrderedSequence()
        {
            var os = new OrderedSequence<string, string>(
                new Key<string, string>(string.Empty, (s) => { return s; }));

            var src = new List<string>
            {
                "zaz",
                "zab",
                "zah",
                "aaz",
                "abz",
                "acd",
                "eeh",
                "heh"
            };

            foreach (var s in src)
                os = os.Insert(s);

            var expected = new List<string>
            {
                "aaz",
                "abz",
                "acd",
                "eeh",
                "heh",
                "zab",
                "zah",
                "zaz",
            };

            var tseq = os.ToSequence();

            var tseqEnum = tseq.GetEnumerator();
            tseqEnum.MoveNext();

            foreach (var es in expected)
            {
                if(0 != es.CompareTo(tseqEnum.Current))
                    throw new Exception("TestStringOrderedSequence failed.");
                tseqEnum.MoveNext();
            }

        }
    }
}
