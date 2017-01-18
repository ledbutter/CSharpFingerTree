using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSeq;

namespace TestCollAndString
{
    class TestPerformance
    {
        static void Main(string[] args)
        {
            int nLength = 100000;
            uint nOperations = 100000;

            StringTest strTest = new StringTest(nLength, nOperations);
            FStringTest fstrTest = new FStringTest(nLength, nOperations);

            double msecs13 = fstrTest.fstringRemove();

            double msecs12 = fstrTest.fstringSubstring();

            double msecs11 = fstrTest.fstringInsert();

            double msecs10 = fstrTest.fstringConcat();

            double msecs9 = strTest.stringSubstring();

            double msecs8 = strTest.stringRemove();

            double msecs7 = strTest.stringInsert();

            double msecs6 = strTest.stringConcat();

            List<UInt32> intContent = new List<UInt32>();

            for (UInt32 i = 0; i < nLength; i++)
                intContent.Add(i);

            CollectionTest<UInt32> shortPerfTest = new
                  CollectionTest<UInt32>(nLength, nOperations, intContent);

            double msecs18 = shortPerfTest.fseqReverse();

            double msecs17 = shortPerfTest.fseqTake();

            double msecs16 = shortPerfTest.fseqSkip();

            double msecs15 = shortPerfTest.fseqElementAt();

            double msecs14 = shortPerfTest.fseqConcat();

            double msecs5 = shortPerfTest.colReverse();

            double msecs4 = shortPerfTest.colTake();

            double msecs3 = shortPerfTest.colSkip();

            double msecs2 = shortPerfTest.colElementAt();

            double msecs = shortPerfTest.colConcat();
        }
    }

    public class FStringTest
    {
        public UInt32 nOperations = 0;
        public int nStringLength = 100000;
        public FString theString = new FString();
        public FString theString2 = new FString();

        public FStringTest(int contLength, UInt32 nOperations)
        {
            this.nOperations = nOperations;
            this.nStringLength = contLength;

            FString str100Filler = new FString(
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789/!@#$%^*()-_+=|?.,`~абгдежзийклмнопрст"
                );

            for (int i = 0; i < contLength / str100Filler.Length(); i++)
            {
                theString = theString.Merge(str100Filler);
                theString2 = theString2.Merge(str100Filler);
            }
        }

        public double fstringConcat()
        {
            Double milliSeconds = 0D;
            char c;

            DateTime start = DateTime.Now;
            for (UInt32 i = 0; i < nOperations; i++)
            {
                {
                    FString strConcat = theString.concat(theString2);

                    c = strConcat.itemAt(2 * nStringLength - 1);
                }
            }
            DateTime end = DateTime.Now;

            milliSeconds += (end - start).TotalMilliseconds;

            return milliSeconds;
        }

        public double fstringInsert()
        {
            Double milliSeconds = 0D;
            char c;


            int halfLength = nStringLength / 2;

            DateTime start = DateTime.Now;
            for (UInt32 i = 0; i < nOperations; i++)
            {
                {
                    FString strInsert = theString.insert(nStringLength - 1000, theString);

                    c = strInsert.itemAt(2 * nStringLength - 1);
                }
            }
            DateTime end = DateTime.Now;

            milliSeconds += (end - start).TotalMilliseconds;

            return milliSeconds;
        }

        public double fstringRemove()
        {
            Double milliSeconds = 0D;

            char c = ' ';

            DateTime start = DateTime.Now;
            for (UInt32 i = 0; i < nOperations; i++)
            {
                {
                    FString fstrSubstr = theString.remove(1000, nStringLength - 100);

                    c = fstrSubstr.itemAt((int)fstrSubstr.Length() -1);
                }
            }
            DateTime end = DateTime.Now;

            milliSeconds += (end - start).TotalMilliseconds;

            return milliSeconds;

        }


        public double fstringSubstring()
        {
            Double milliSeconds = 0D;

            char c = ' ';

            DateTime start = DateTime.Now;
            for (UInt32 i = 0; i < nOperations; i++)
            {
                {
                    FString fstrSubstr = theString.substring(1000, nStringLength - 1000);

                    c = fstrSubstr.itemAt(nStringLength - 1005);
                }
            }
            DateTime end = DateTime.Now;

            milliSeconds += (end - start).TotalMilliseconds;

            return milliSeconds;
        }
    }

    public class StringTest
    {
        public UInt32 nOperations = 0;
        public int nStringLength = 100000;
        public string theString = string.Empty;
        public string theString2 = string.Empty;

        public StringTest(int contLength, UInt32 nOperations)
        {
            this.nOperations = nOperations;
            this.nStringLength = contLength;

            string str100Filler =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789/!@#$%^*()-_+=|?.,`~абгдежзийклмнопрст";

            for (int i = 0; i < contLength / str100Filler.Length; i++)
            {
                theString += str100Filler;
                theString2 += str100Filler;
            }
        }

        public double stringConcat()
        {
            Double milliSeconds = 0D;

            char c;

            DateTime start = DateTime.Now;
            for (UInt32 i = 0; i < nOperations; i++)
            {
                {
                    string strConcat = theString + theString2;

                    c = strConcat[2 * nStringLength - 1];
                }
            }
            DateTime end = DateTime.Now;

            milliSeconds += (end - start).TotalMilliseconds;

            return milliSeconds;
        }

        public double stringInsert()
        {
            Double milliSeconds = 0D;

            char c;

            DateTime start = DateTime.Now;
            for (UInt32 i = 0; i < nOperations; i++)
            {
                {
                    string strInsert = theString.Insert(nStringLength - 1000, theString2);

                    c = strInsert[2 * nStringLength - 1005];
                }
            }
            DateTime end = DateTime.Now;

            milliSeconds += (end - start).TotalMilliseconds;

            return milliSeconds;
        }

        public double stringRemove()
        {
            Double milliSeconds = 0D;

            char c = ' ';

            DateTime start = DateTime.Now;
            for (UInt32 i = 0; i < nOperations; i++)
            {
                {
                    string strRemove = theString.Remove(1000, nStringLength - 1000);

                    c = strRemove[strRemove.Length - 1];
                }
            }
            DateTime end = DateTime.Now;

            milliSeconds += (end - start).TotalMilliseconds;

            return milliSeconds;
        }

        public double stringSubstring()
        {
            Double milliSeconds = 0D;

            char c = ' ';

            DateTime start = DateTime.Now;
            for (UInt32 i = 0; i < nOperations; i++)
            {
                {
                    string strSubstr = theString.Substring(1000, nStringLength - 1000);

                    c = strSubstr[nStringLength - 1005];
                }
            }
            DateTime end = DateTime.Now;

            milliSeconds += (end - start).TotalMilliseconds;

            return milliSeconds;
        }
    }
    
    public class CollectionTest<T> 
    {
        public UInt32 nOperations = 0;
        public int nContLength = 0;
        public IEnumerable<T> theContainer = null;
        public FNSeq<T> theFContainer = null;
        public FNSeq<T> theFContainer2 = null;

        List<T> theContainer2 = new List<T>();

        public CollectionTest(int contLength, UInt32 nOperations, IEnumerable<T> contents)
        {
            this.nContLength = contLength;

            this.nOperations = nOperations;

            theContainer = contents;
            foreach(T t in contents)
                theContainer2.Add(t);
            
            theFContainer = new FNSeq<T>(contents);
            theFContainer2 = new FNSeq<T>(contents);

        }

        public double fseqConcat()
        {
            Double milliSeconds = 0D;
            T myT = default(T);

            DateTime start = DateTime.Now;
            for (UInt32 i = 0; i < nOperations; i++)
            {
                {
                    FNSeq<T> theResult =
                          theFContainer.Merge(theFContainer2);
                    myT = theResult.itemAt(nContLength + nContLength - 1);
                }
            }
            DateTime end = DateTime.Now;

            milliSeconds += (end - start).TotalMilliseconds;

            return milliSeconds;
        }

        public double colConcat()
        {
            Double milliSeconds = 0D;
            T myT = default(T);

            DateTime start = DateTime.Now;
            for (UInt32 i = 0; i < nOperations; i++)
            {
                {
                    IEnumerable<T> theResult = 
                          theContainer.Concat(theContainer);
                    myT = theResult.ElementAt(nContLength + nContLength - 1);
                }
            }
            DateTime end = DateTime.Now;

            milliSeconds += (end - start).TotalMilliseconds;

            return milliSeconds;

        }

        public double fseqElementAt()
        {
            Double milliSeconds = 0D;

            int indMid = nContLength / 2;

            T elem;

            for (UInt32 i = 0; i < nOperations; i++)
            {
                DateTime start = DateTime.Now;
                {
                    elem = theFContainer2.itemAt(indMid);
                }
                DateTime end = DateTime.Now;

                milliSeconds += (end - start).TotalMilliseconds;

            }
            return milliSeconds;
        }

        public double colElementAt()
        {
            Double milliSeconds = 0D;

            int indMid = nContLength / 2;

            T elem;

            for (UInt32 i = 0; i < nOperations; i++)
            {
                DateTime start = DateTime.Now;
                {
                    elem = theContainer2[indMid];
                }
                DateTime end = DateTime.Now;

                milliSeconds += (end - start).TotalMilliseconds;

            }
            return milliSeconds;

        }

        public double fseqSkip()
        {
            Double milliSeconds = 0D;
            T myT = default(T);

            int halfLength = nContLength / 2;

            DateTime start = DateTime.Now;
            for (UInt32 i = 0; i < nOperations; i++)
            {
                {
                    FNSeq<T> skippedFSeq = theFContainer.skip(halfLength);
                    myT = skippedFSeq.itemAt(halfLength - 1);
                }
            }
            DateTime end = DateTime.Now;

            milliSeconds += (end - start).TotalMilliseconds;

            return milliSeconds;
        }

        public double fseqTake()
        {
            Double milliSeconds = 0D;
            T myT = default(T);

            int halfLength = nContLength / 2;

            DateTime start = DateTime.Now;
            for (UInt32 i = 0; i < nOperations; i++)
            {
                {
                    FNSeq<T> skippedFSeq = theFContainer.take(halfLength);
                    myT = skippedFSeq.itemAt(halfLength - 1);
                }
            }
            DateTime end = DateTime.Now;

            milliSeconds += (end - start).TotalMilliseconds;

            return milliSeconds;
        }

        public double colSkip()
        {
            Double milliSeconds = 0D;
            T myT = default(T);

            int halfLength = nContLength / 2;

            DateTime start = DateTime.Now;
            for (UInt32 i = 0; i < nOperations; i++)
            {
                {
                    IEnumerable<T> skippedCol = theContainer.Skip(halfLength);
                    myT = skippedCol.ElementAt(halfLength - 1);
                }
            }
            DateTime end = DateTime.Now;

            milliSeconds += (end - start).TotalMilliseconds;

            return milliSeconds;

        }

        public double colTake()
        {
            Double milliSeconds = 0D;
            T myT = default(T);

            int halfLength = nContLength / 2;

            DateTime start = DateTime.Now;
            for (UInt32 i = 0; i < nOperations; i++)
            {
                {
                    IEnumerable<T> takenCol = theContainer.Take(halfLength);
                    myT = takenCol.ElementAt(halfLength - 1);
                }
            }
            DateTime end = DateTime.Now;

            milliSeconds += (end - start).TotalMilliseconds;

            return milliSeconds;

        }

        public double fseqReverse()
        {
            Double milliSeconds = 0D;
            T myT = default(T);

            DateTime start = DateTime.Now;
            for (UInt32 i = 0; i < nOperations; i++)
            {
                {
                    FNSeq<T> fsReversed = theFContainer.reverse();
                    myT = fsReversed.itemAt(nContLength - 1);
                }
            }
            DateTime end = DateTime.Now;

            milliSeconds += (end - start).TotalMilliseconds;

            return milliSeconds;
        }

        public double colReverse()
        {
            Double milliSeconds = 0D;
            T myT = default(T);

            DateTime start = DateTime.Now;
            for (UInt32 i = 0; i < nOperations; i++)
            {
                {
                    IEnumerable<T> takenCol = theContainer.Reverse();
                    myT = takenCol.ElementAt(nContLength - 1);
                }
            }
            DateTime end = DateTime.Now;

            milliSeconds += (end - start).TotalMilliseconds;

            return milliSeconds;

        }
    }
}
