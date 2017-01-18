using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FingerTree;
namespace FSeq
{
    public class FString
    {
        private Seq<char> theSeq = null;

        public FString()
        {
            theSeq = new Seq<char>(new List<char>());
        }

        public FString(string aString)
        {
            theSeq = new Seq<char>(aString.ToCharArray());
        }

        protected FString(Seq<char> aSeq)
        {
            theSeq = aSeq;
        }

        public uint Length()
        {
            return this.theSeq.length;
        }

        public FString Merge(FString string2)
        {
            return new FString(new Seq<char>(theSeq.Merge(string2.theSeq.treeRep)));
        }

        public FString concat(FString afString)
        {
            return this.Merge(afString);
        }

        public static FString
            stringJoin(FString[] stringList,
                       FString strSeparator)
        {
            FString fStr = new FString();

            int numStrings = stringList.Length;

            if (numStrings == 0)
                return fStr;
            //else
            int i = 0;
            for (i = 0; i < numStrings - 1; i++)
            {
                fStr = fStr.Merge(stringList[i]);
                fStr = fStr.Merge(strSeparator);
            }
            fStr = fStr.Merge(stringList[i]);

            return fStr;
        }

        public FString insert(int startInd, FString fstr2)
        {
            Pair<FTreeM<SizedElem<char>, uint>, FTreeM<SizedElem<char>, uint>> theSplit =
                theSeq.SeqSplit(new MPredicate<uint>
                            (FP.Curry<uint, uint, bool>(theLTMethod, (uint)startInd)));

            return new FString(
                            new Seq<char>
                                (
                                     (
                                       theSplit.first.Merge(fstr2.theSeq.treeRep)
                                                     .Merge(((Seq<char>)(theSplit.second)).treeRep)
                                     )
                                 )
                              );

        }

        public FString remove(int startInd, int subLength)
        {
            uint theLength = theSeq.length;

            if (theLength == 0 || subLength <= 0)
                return this;
            //else
            if (startInd < 1)
                startInd = 0;

            if (startInd + subLength > theLength)
                subLength = (int)(theLength - startInd);

            // Now ready to do the real work
            Pair<FTreeM<SizedElem<char>, uint>, FTreeM<SizedElem<char>, uint>> split1 =
            theSeq.SeqSplit
                      (
                      new MPredicate<uint>
                          (FP.Curry<uint, uint, bool>(theLTMethod, (uint)startInd))
                      );

            Pair<FTreeM<SizedElem<char>, uint>, FTreeM<SizedElem<char>, uint>> split2 =
            split1.second.SeqSplit
                      (
                      new MPredicate<uint>
                          (FP.Curry<uint, uint, bool>(theLTMethod, (uint)subLength))
                      );

            FString fsResult =
              new FString(
                    new Seq<char>
                     (
                        split1.first.Merge(((Seq<char>)(split2.second)).treeRep)
                     )
             );
            return fsResult;
        }

        public FString substring(int startInd, int subLength)
        {
            uint theLength = theSeq.length;

            if (theLength == 0 || subLength <= 0)
                return this;
            //else
            if (startInd < 1)
                    startInd = 0;

            if (startInd + subLength > theLength)
                subLength = (int)(theLength - startInd);

            // Now ready to do the real work
            FString fsResult =
              new FString(
                    new Seq<char>
                     (
                        theSeq.SeqSplit
                          (
                          new MPredicate<uint>
                              (FP.Curry<uint, uint, bool>(theLTMethod, (uint)startInd))
                          ).second
                             .SeqSplit
                        (new MPredicate<uint>
                              (FP.Curry<uint, uint, bool>(theLTMethod, (uint)subLength))
                        ).first
                     )
             );
            return fsResult;
        }

            public char itemAt(int ind)
            {
                if (ind < 0 || ind >= Length())
                    throw new ArgumentOutOfRangeException();
                //else
                return theSeq.ElemAt(((uint)ind));
            }



        bool theLTMethod(uint i1, uint i2)
        {
            return i1 < i2;
        }

    }


    public class FNSeq<T> //where T : IMeasured<uint>
    {
        private Seq<T> theSeq = null;

        public FNSeq()
        {
            theSeq = new Seq<T>(new List<T>());
        }

        public FNSeq(IEnumerable<T> seqIterator)
        {
            theSeq = new Seq<T>(new List<T>());

            foreach(T t in seqIterator)
                theSeq = (Seq<T>)(theSeq.Push_Back(new SizedElem<T>(t)));
        }

        protected FNSeq(Seq<T> aSeq)
        {
            theSeq = aSeq;
        }

        public uint Length()
        {
            return this.theSeq.length;
        }

        public List<T> ToSequence()
        {
            List<T> lstResult = new List<T>();

            foreach (SizedElem<T> elem in theSeq.ToSequence())
                lstResult.Add(elem.Element);

            return lstResult;//.ToArray();
        }

        public T itemAt(int ind)
        {
            if (ind < 0 || ind >= Length())
                throw new ArgumentOutOfRangeException();
            //else
            return theSeq.ElemAt(((uint)ind));
        }

        public FNSeq<T> reverse()
        {
            return new FNSeq<T>((Seq<T>)(theSeq.Reverse()));
        }

        public FNSeq<T> Merge(FNSeq<T> seq2)
        {
            return new FNSeq<T>(new Seq<T>(theSeq.Merge(seq2.theSeq.treeRep)));
        }

        public FNSeq<T> skip(int length)
        {
            return new FNSeq<T>
                (
                 new Seq<T>
                   (
            this.theSeq.dropUntil(new MPredicate<uint>
                         (FP.Curry<uint, uint, bool>(theLTMethod, (uint)length))
                                  )
                    )
                );
        }

        public FNSeq<T> take(int length)
        {
            return new FNSeq<T>
                (
                 new Seq<T>
                   (
            this.theSeq.takeUntil(new MPredicate<uint>
                         (FP.Curry<uint, uint, bool>(theLTMethod, (uint)length))
                                  )
                    )
                );
        }

        public FNSeq<T> subsequence(int startInd, int subLength)
        {
            uint theLength = theSeq.length;

            if (theLength == 0 || subLength <= 0)
                return this;
            //else
            if (startInd < 0)
                startInd = 0;

            if (startInd + subLength > theLength)
                subLength = (int)(theLength - startInd);

            // Now ready to do the real work
            FNSeq<T> fsResult =
              new FNSeq<T>(
                (Seq<T>)
                 (
                    ((Seq<T>)
                        (theSeq.SeqSplit
                          (
                          new MPredicate<uint>
                              (FP.Curry<uint, uint, bool>(theLTMethod, (uint)startInd))
                          ).second
                         )
                     ).SeqSplit
                        (new MPredicate<uint>
                              (FP.Curry<uint, uint, bool>(theLTMethod, (uint)subLength))
                        ).first
                 )
             );
            return fsResult;
        }

        public FNSeq<T> remove(int ind)
        {
            if (ind < 0 || ind >= Length())
                throw new ArgumentOutOfRangeException();
            //else
            return new FNSeq<T>(theSeq.RemoveAt((uint)(ind)));
        }

        // this inserts a whole sequence, so we cannot just use Seq.snsertAt()
        public FNSeq<T> insert_before(int ind, FNSeq<T> fSeq2)
        {
            if (ind < 0 || ind >= this.Length())
                throw new ArgumentOutOfRangeException();
            //else
            Pair<FTreeM<SizedElem<T>, uint>, FTreeM<SizedElem<T>, uint>> theSplit =
                theSeq.SeqSplit
                     (new MPredicate<uint>
                             (
                               FP.Curry<uint, uint, bool>(theLTMethod, (uint)ind - 1)
                             )
                      );

            FNSeq<T> fs1 = new FNSeq<T>((Seq<T>)(theSplit.first));
            FNSeq<T> fs3 = new FNSeq<T>((Seq<T>)(theSplit.second));

            return fs1.Merge(fSeq2).Merge(fs3);
        }

        bool theLTMethod(uint i1, uint i2)
        {
            return i1 < i2;
        }

    }
}

