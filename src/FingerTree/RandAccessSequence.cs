using System;
using System.Collections.Generic;
using System.Text;

namespace FingerTree
{

    public static class Size
    {
        public static Monoid<uint> theMonoid =
             new Monoid<uint>(0, new Monoid<uint>.monOp(anAddOp));
        
        public static uint anAddOp(uint s1, uint s2)
        {
            return s1 + s2;
        }

    }

    public static class FP
    {
        public static Func<Y, Z> Curry<X, Y, Z>(this Func<X, Y, Z> func, X x)
        {
            return (y) => func(x, y);
        }

    }

    public abstract class Elem<T, V> : IMeasured<V> 
    {
        protected T theElem;

        public Elem(T t)
        {
            theElem = t;
        }

        public T Element 
        {
            get { return theElem; }
        }

        public abstract V Measure();
    }

    public class SizedElem<T> : Elem<T, uint>
    {
        public SizedElem(T t) : base(t)
        {
        }

        public override uint Measure()
        {
            return 1;
        }
    }

    public class Seq<T> : FTreeM<SizedElem<T>, uint>
    {
        public FTreeM<SizedElem<T>, uint> treeRep =
            new EmptyFTreeM<SizedElem<T>, uint>(Size.theMonoid);

        private static  bool theLessThanIMethod2(uint n, uint i)
        {
            return n < i;
        }

        public Seq(IEnumerable<T> aList)
        {
            foreach (T t in aList)
                treeRep = treeRep.Push_Back(new SizedElem<T>(t));
        }

        public Seq(FTreeM<SizedElem<T>, uint> elemTree)
        {
            treeRep = elemTree;
        }

        public override Monoid<uint> treeMonoid
        {
            get
            {
                return treeRep.treeMonoid;
            }
        }

        public override uint Measure()
        {
            return treeRep.Measure();
        }

        public override FTreeM<SizedElem<T>, uint> Push_Front(SizedElem<T> t)
        {
            return new Seq<T>(treeRep.Push_Front(t));
        }

        public override FTreeM<SizedElem<T>, uint> Push_Back(SizedElem<T> t)
        {
            return new Seq<T>(treeRep.Push_Back(t));
        }

        public override IEnumerable<SizedElem<T>> ToSequence()
        {
            return treeRep.ToSequence();
        }

        public override IEnumerable<SizedElem<T>> ToSequenceR()
        {
            return treeRep.ToSequenceR();
        }

        public override ViewL<SizedElem<T>, uint> LeftView()
        {
            ViewL<SizedElem<T>, uint> internLView = treeRep.LeftView();

            internLView.ftTail = new Seq<T>(internLView.ftTail);

            return internLView;
        }

        public override ViewR<SizedElem<T>, uint> RightView()
        {
            ViewR<SizedElem<T>, uint> internRView = treeRep.RightView();
            internRView.ftInit = new Seq<T>(internRView.ftInit);

            return internRView;
        }

        public override FTreeM<SizedElem<T>, uint> Reverse()
        {
            return new Seq<T>(treeRep.Reverse());
        }

        public override FTreeM<SizedElem<T>, uint> Merge(FTreeM<SizedElem<T>, uint> rightFT)
        {
            //if (!(rightFT is Seq<T>))
            //    throw new Exception("Error: Seq merge with non-Seq attempted!");
            ////else
            return treeRep.Merge(rightFT);
        }

        public override Split<FTreeM<SizedElem<T>, uint>, SizedElem<T>, uint>
            Split(MPredicate<uint> predicate, uint acc)
        {
            Split<FTreeM<SizedElem<T>, uint>, SizedElem<T>, uint> internSplit
                = treeRep.Split(predicate, acc);

            internSplit.left =
                new Seq<T>(internSplit.left);
            internSplit.right =
                new Seq<T>(internSplit.right);

            return internSplit;
        }

        public override Pair<FTreeM<SizedElem<T>, uint>, FTreeM<SizedElem<T>, uint>>
            SeqSplit(MPredicate<uint> predicate)
        {
            Pair<FTreeM<SizedElem<T>, uint>, FTreeM<SizedElem<T>, uint>> internPair
                = treeRep.SeqSplit(predicate);

            internPair.first = new Seq<T>(internPair.first);
            internPair.second = new Seq<T>(internPair.second);

            return internPair;
        }

        public override FTreeM<SizedElem<T>, uint>
            App2(List<SizedElem<T>> ts, FTreeM<SizedElem<T>, uint> rightFT)
        {
            return treeRep.App2(ts, rightFT);
        }



        public uint length
        {
            get { return treeRep.Measure(); }
        }

        public Pair<Seq<T>, Seq<T>> SplitAt(uint ind)
        {
            var treeSplit = 
                treeRep.SeqSplit(new MPredicate<uint>
                                   (FP.Curry<uint, uint, bool>(theLessThanIMethod2, ind))
                                 );
            return new Pair<Seq<T>, Seq<T>>
                     (new Seq<T>(treeSplit.first),
                      new Seq<T>(treeSplit.second)
                     );
        }

        public T ElemAt(uint ind)
        {
            return treeRep.Split(new MPredicate<uint>
                                   (FP.Curry<uint, uint, bool>(theLessThanIMethod2, ind)),
                                 0
                                 ).splitItem.Element;
        }

        public T this[uint index]
        {
            get
            {
                return ElemAt(index);
            }
        }

        public Seq<T> InsertAt(uint index, T t)
        {
            if (index > length)
                throw new IndexOutOfRangeException
                    (string.Format("Error: Attempt to insert at position: {0} "
                                  + "exceeding the length: {1} of this sequence.",
                                  index,
                                  length
                                  )
                     );
            //else
            Pair<Seq<T>, Seq<T>> theSplit = this.SplitAt(index);

            return new Seq<T>
               (
                theSplit.first.Merge(theSplit.second.Push_Front(new SizedElem<T>(t)))
                );
        }

        public Seq<T> RemoveAt(uint index)
        {
            if (index > length)
                throw new IndexOutOfRangeException
                    (string.Format("Error: Attempt to remove at position: {0} "
                                  + "exceeding the length: {1} of this sequence.",
                                  index,
                                  length
                                  )
                     );
            //else
            Pair<Seq<T>, Seq<T>> theSplit = this.SplitAt(index);

            return new Seq<T>
              (
                theSplit.first.treeRep.Merge(theSplit.second.treeRep.LeftView().ftTail)
               );
        }

    }
}
