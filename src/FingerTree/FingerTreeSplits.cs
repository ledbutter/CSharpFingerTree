using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace FingerTree
{
    // Types:
    //     U -- the type of Containers that can be split
    //     T -- the type of elements in a container of type U
    //     V -- the type of the Measure-value when an element is measured
    public class Split<U, T, V> where U : ISplittable<T, V> where T : IMeasured<V>
    {
        public U left;

        public T splitItem;

        public U right;

        public Split(U left, T splitItem, U right)
        {
            this.left = left;
            this.splitItem = splitItem;
            this.right = right;
        }
    }

    public delegate bool MPredicate<V> (V v); 


    public class Pair<T, V>
    {
        public T first;
        public V second;

        public Pair(T first, V second)
        {
            this.first = first;
            this.second = second;
        }
    }

    public abstract partial class FTreeM<T, M> where T : IMeasured<M>
    { 
        public abstract Split<FTreeM<T, M>, T, M> Split(MPredicate<M> predicate, M acc);

        public abstract Pair<FTreeM<T, M>, FTreeM<T, M>> SeqSplit(MPredicate<M> predicate);

        public FTreeM<T, M> takeUntil(MPredicate<M> predicate)
        {
            return SeqSplit(predicate).first;
        }

        public FTreeM<T, M> dropUntil(MPredicate<M> predicate)
        {
            return SeqSplit(predicate).second;
        }

        public T Lookup(MPredicate<M> predicate, M acc)
        {
            return dropUntil(predicate).LeftView().head;
        }

        public partial class Digit<U, V> : ISplittable<U, V>
              where U : IMeasured<V>
        {
            // Assumption: predicate is false on the left end
            //             and true on the right end of the container
            public Split<Digit<U, V>, U, V> Split(MPredicate<V> predicate, V acc)
            {
                int cnt = digNodes.Count;

                if (cnt == 0)
                    throw new Exception("Error: Split of an empty Digit attempted!");
                //else
                U headItem = digNodes[0];
                if(cnt == 1)
                    return new Split<Digit<U,V>,U,V>
                                  (new Digit<U, V>(theMonoid, new List<U>()),
                                   headItem,
                                   new Digit<U, V>(theMonoid, new List<U>())
                                  );
                //else
                List<U> digNodesTail = new List<U>(digNodes.GetRange(1, cnt - 1));
                Digit<U, V> digitTail = new Digit<U, V>(theMonoid, digNodesTail);

                V acc1 = theMonoid.theOp(acc, headItem.Measure());
                if (predicate(acc1))
                    return new Split<Digit<U, V>, U, V>
                                  (new Digit<U, V>(theMonoid, new List<U>()),
                                   headItem,
                                   digitTail
                                  );
                //else
                Split<Digit<U,V>,U,V> tailSplit = digitTail.Split(predicate, acc1);

                tailSplit.left.digNodes.Insert(0, headItem);

                return tailSplit;
            }
        }
    }

    public partial class EmptyFTreeM<T, M> : FTreeM<T, M> where T : IMeasured<M>
    {
        public override Split<FTreeM<T, M>, T, M> Split(MPredicate<M> predicate, M acc)
        {
            throw new Exception("Error: Split attempted on an EmptyFTreeM !");
        }

        public override Pair<FTreeM<T, M>, FTreeM<T, M>> SeqSplit(MPredicate<M> predicate)
        {
            return new Pair<FTreeM<T,M>,FTreeM<T,M>>
                     (new EmptyFTreeM<T, M>(theMonoid),
                      new EmptyFTreeM<T, M>(theMonoid)
                      );
        }
    }

    public partial class SingleFTreeM<T, M> : FTreeM<T, M> where T : IMeasured<M>
    {
        public override Split<FTreeM<T, M>, T, M> Split(MPredicate<M> predicate, M acc)
        {
            return new Split<FTreeM<T,M>,T,M>
                         (new EmptyFTreeM<T, M>(theMonoid),
                          theSingle,
                          new EmptyFTreeM<T, M>(theMonoid)
                          );
        }

        public override Pair<FTreeM<T, M>, FTreeM<T, M>> SeqSplit(MPredicate<M> predicate)
        {
            M theMeasure = theSingle.Measure();

            if(predicate(theMeasure))
                return new Pair<FTreeM<T,M>,FTreeM<T,M>>
                         (new EmptyFTreeM<T, M>(theMonoid),
                          this
                          );
            //else
            return new Pair<FTreeM<T, M>, FTreeM<T, M>>
                     (this,
                      new EmptyFTreeM<T, M>(theMonoid)
                      );


        }

    }

    public partial class DeepFTreeM<T, M> : FTreeM<T, M> where T : IMeasured<M>
    {
        public override Split<FTreeM<T, M>, T, M> Split(MPredicate<M> predicate, M acc)
        {
            M vPr = theMonoid.theOp(acc, frontDig.Measure());

            if(predicate(vPr))
            {
                Split<Digit<T, M>, T, M> 
                    frontSplit = frontDig.Split(predicate, acc);

                return new Split<FTreeM<T, M>, T, M>
                            (FTreeM<T, M>.FromSequence(frontSplit.left.digNodes, theMonoid),
                             frontSplit.splitItem,
                             FTreeM<T, M>.Create(frontSplit.right.digNodes, innerFT, backDig)
                            );
            }
            //else
            M vM = theMonoid.theOp(vPr, innerFT.Measure());

            if (predicate(vM))
            {
                var midSplit = innerFT.Split(predicate, vPr);

                var midLeft = midSplit.left;
                var midItem = midSplit.splitItem;

                var splitMidLeft =
                    (new Digit<T, M>(theMonoid, midItem.theNodes)).Split
                                                                (predicate, 
                                                                 theMonoid.theOp(vPr, midLeft.Measure())
                                                                );

                T finalsplitItem = splitMidLeft.splitItem;

                FTreeM<T, M> finalLeftTree =
                    FTreeM<T, M>.CreateR(frontDig, midLeft, splitMidLeft.left.digNodes);

                FTreeM<T, M> finalRightTree =
                    FTreeM<T, M>.Create(splitMidLeft.right.digNodes, midSplit.right, backDig);

                return new Split<FTreeM<T, M>, T, M>
                             (finalLeftTree, finalsplitItem, finalRightTree);

            }
            //else
            Split<Digit<T, M>, T, M>
                backSplit = backDig.Split(predicate, vM);

            return new Split<FTreeM<T, M>, T, M>
                        (FTreeM<T, M>.CreateR(frontDig,  innerFT, backSplit.left.digNodes),
                         backSplit.splitItem,
                         FTreeM<T, M>.FromSequence(backSplit.right.digNodes, theMonoid)
                        );
        }

        public override Pair<FTreeM<T, M>, FTreeM<T, M>> SeqSplit(MPredicate<M> predicate)
        {
            if(!predicate(Measure()))
                return new Pair<FTreeM<T, M>, FTreeM<T, M>>
                         (this,
                          new EmptyFTreeM<T, M>(theMonoid)
                          );
            //else
            Split<FTreeM<T, M>, T, M> theSplit = Split(predicate, theMonoid.zero);

            return new Pair<FTreeM<T, M>, FTreeM<T, M>>
                    (theSplit.left, theSplit.right.Push_Front(theSplit.splitItem)
                     );
        }

    }
}