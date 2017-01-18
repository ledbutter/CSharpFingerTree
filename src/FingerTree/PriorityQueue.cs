using System;
using System.Collections.Generic;
using System.Text;

namespace FingerTree
{
    public static class Prio
    {
        public static Monoid<double> theMonoid =
             new Monoid<double>(double.NegativeInfinity, new Monoid<double>.monOp(aMaxOp));

        public static double aMaxOp(double d1, double d2)
        {
            return (d1 > d2) ? d1 : d2;
        }

    }

    public class CompElem<T> : Elem<T, double>
    {
        private double dblRep;

        public CompElem(T t)
            : base(t)
        {
            dblRep = double.Parse(t.ToString());
        }

        public override double Measure()
        {
            return this.dblRep;
        }
    }

    public class PriorityQueue<T> : FTreeM<CompElem<T>, double> 
    {
        private FTreeM<CompElem<T>, double> treeRep =
            new EmptyFTreeM<CompElem<T>, double>(Prio.theMonoid);

        private static bool theLessOrEqMethod2(double d1, double d2)
        {
            return d1 <= d2;
        }

        public PriorityQueue(IEnumerable<T> aList)
        {
            foreach (T t in aList)
                treeRep = treeRep.Push_Back(new CompElem<T>(t));
        }

        public PriorityQueue(FTreeM<CompElem<T>, double> elemTree)
        {
            treeRep = elemTree;
        }

        public override Monoid<double> treeMonoid
        {
            get
            {
                return treeRep.treeMonoid;
            }
        }

        public override double Measure()
        {
            return treeRep.Measure();
        }

        public override FTreeM<CompElem<T>, double> Push_Front(CompElem<T> elemT)
        {
            return new PriorityQueue<T>
                    (treeRep.Push_Front(elemT));
        }


        public override FTreeM<CompElem<T>, double> Push_Back(CompElem<T> elemT)
        {
            return new PriorityQueue<T>
                    (treeRep.Push_Back(elemT));
        }

        public override IEnumerable<CompElem<T>> ToSequence()
        {
            return treeRep.ToSequence();
        }

        public override IEnumerable<CompElem<T>> ToSequenceR()
        {
            return treeRep.ToSequenceR();
        }


        public override ViewL<CompElem<T>, double> LeftView()
        {
            ViewL<CompElem<T>, double> internLView = treeRep.LeftView();

            internLView.ftTail = new PriorityQueue<T>(internLView.ftTail);

            return internLView;
        }

        public override ViewR<CompElem<T>, double> RightView()
        {
            ViewR<CompElem<T>, double> internRView = treeRep.RightView();
            internRView.ftInit = new PriorityQueue<T>(internRView.ftInit);

            return internRView;
        }

        public override FTreeM<CompElem<T>, double> Merge(FTreeM<CompElem<T>, double> rightFT)
        {
            if(!(rightFT is PriorityQueue<T>))
                throw new Exception("Error: PriQue merge with non-PriQue attempted!");
            //else
            return new PriorityQueue<T>
                    (
                     treeRep.Merge(((PriorityQueue<T>)rightFT).treeRep)
                     );
        }

        public override Split<FTreeM<CompElem<T>, double>, CompElem<T>, double>
            Split(MPredicate<double> predicate, double acc)
        {
            Split<FTreeM<CompElem<T>, double>, CompElem<T>, double> internSplit
                = treeRep.Split(predicate, acc);

            internSplit.left =
                new PriorityQueue<T>(internSplit.left);
            internSplit.right =
                new PriorityQueue<T>(internSplit.right);

            return internSplit;

        }

        public override Pair<FTreeM<CompElem<T>, double>, FTreeM<CompElem<T>, double>> 
            SeqSplit(MPredicate<double> predicate)
        {
            Pair<FTreeM<CompElem<T>, double>, FTreeM<CompElem<T>, double>> internPair
                = treeRep.SeqSplit(predicate);

            internPair.first = new PriorityQueue<T>(internPair.first);
            internPair.second = new PriorityQueue<T>(internPair.second);

            return internPair;
        }

        public override FTreeM<CompElem<T>, double>
            App2(List<CompElem<T>> ts, FTreeM<CompElem<T>, double> rightFT)
        {
            return treeRep.App2(ts, rightFT);
        }

        public Pair<T, PriorityQueue<T>> extractMax()
        {
            var trSplit =
                treeRep.Split(new MPredicate<double>
                                 (FP.Curry<double, double, bool>
                                          (theLessOrEqMethod2, treeRep.Measure())
                                  ),
                              Prio.theMonoid.zero
                              );
            return new Pair<T, PriorityQueue<T>>
                     (trSplit.splitItem.Element,
                      new PriorityQueue<T>(trSplit.left.Merge(trSplit.right))
                     );
        }
    }

}