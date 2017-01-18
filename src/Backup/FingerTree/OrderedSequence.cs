using System;
using System.Collections.Generic;
using System.Text;

namespace FingerTree
{
    public class Key<T, V> where V : IComparable
    {
        public delegate V getKey(T t);
        
        // maybe we shouldn't care for NoKey, as this is too theoretic
        public V NoKey;

        public getKey KeyAssign;

        public Key(V noKey, getKey KeyAssign)
        {
            this.KeyAssign = KeyAssign;
        }

    }

    public class KeyMonoid<T, V> where V : IComparable
    {
        public Key<T, V> KeyObj;

        public Monoid<V> theMonoid;

        public V aNextKeyOp(V v1, V v2)
        {
            return (v2.CompareTo(KeyObj.NoKey) == 0) ? v1 : v2;
        }

        //constructor
        public KeyMonoid(Key<T, V> KeyObj)
        {
            this.KeyObj = KeyObj;

            this.theMonoid = 
             new Monoid<V>(KeyObj.NoKey, new Monoid<V>.monOp(aNextKeyOp));
        }
    }

    public class OrdElem<T, V> : Elem<T, V> where V : IComparable
    {
        public Key<T, V> KeyObj;

        public OrdElem(T t, Key<T, V> KeyObj)
            : base(t)
        {
            this.KeyObj = KeyObj;
        }

        public override V Measure()
        {
            return KeyObj.KeyAssign(this.Element);
        }
    }

        public class OrderedSequence<T, V> where V : IComparable
        {
            private FTreeM<OrdElem<T, V>, V> treeRep;

            private Key<T, V> KeyObj;


            private static bool theLTMethod2(V v1, V v2)
            {
                return v1.CompareTo(v2) < 0;
            }

            private static bool theLEMethod2(V v1, V v2)
            {
                return v1.CompareTo(v2) <= 0;
            }

            private static bool theGTMethod2(V v1, V v2)
            {
                return v1.CompareTo(v2) > 0;
            }

            // Constructor 1
            public OrderedSequence(Key<T, V> KeyObj)
            {
                treeRep = new EmptyFTreeM<OrdElem<T, V>, V>(new KeyMonoid<T, V>(KeyObj).theMonoid);
                this.KeyObj = KeyObj;
            }

            // Constructor 2
            public OrderedSequence(Key<T, V> KeyObj, IEnumerable<T> aList)
            {
                this.KeyObj = KeyObj;

                OrderedSequence<T, V> tempSeq = new OrderedSequence<T, V>(KeyObj);
                treeRep = new EmptyFTreeM<OrdElem<T, V>, V>(new KeyMonoid<T, V>(KeyObj).theMonoid);

                foreach (T t in aList)
                    tempSeq = tempSeq.Push_Back(new OrdElem<T, V>(t, KeyObj));

                treeRep = tempSeq.treeRep;
            }
            
            // Constructor 3
            protected OrderedSequence(Key<T, V> KeyObj, FTreeM<OrdElem<T, V>, V> anOrdElTree)
            {
                this.KeyObj = KeyObj;
                treeRep = anOrdElTree;
            }


            OrderedSequence<T, V> Push_Back(OrdElem<T, V> ordEl)
            {
                var viewR = treeRep.RightView();

                if (viewR != null)
                {
                    if (viewR.last.Measure()
                                 .CompareTo(ordEl.Measure())
                        > 0)
                        throw new Exception(
                   "OrderedSequence Error: PushBack() of an element less than the biggest seq el."
                                            );
                }
                //else
                return new OrderedSequence<T,V>(KeyObj, treeRep.Push_Back(ordEl));
            }

            OrderedSequence<T, V> Push_Front(OrdElem<T, V> ordEl)
            {
                var viewL = treeRep.LeftView();

                if (viewL != null)
                {
                    if (treeRep.LeftView().head.Measure()
                                 .CompareTo(ordEl.Measure())
                        < 0)
                        throw new Exception(
                   "OrderedSequence Error: PushFront() of an element greater than the smallest seq el."
                                            );
                }
                //else
                return new OrderedSequence<T,V>(KeyObj, treeRep.Push_Front(ordEl));
            }

            public Pair<OrderedSequence<T, V>, OrderedSequence<T, V>>
                     Partition(V vK)
            {
                Pair<FTreeM<OrdElem<T, V>, V>, FTreeM<OrdElem<T, V>, V>> baseSeqSplit
               =
                treeRep.SeqSplit(new MPredicate<V>
                                      (FP.Curry<V, V, bool>
                                        (theLEMethod2, vK)
                                       )
                                );

                OrderedSequence<T, V> left
                     = new OrderedSequence<T, V>(KeyObj, baseSeqSplit.first);

                OrderedSequence<T, V> right
                     = new OrderedSequence<T, V>(KeyObj, baseSeqSplit.second);

                return new
                      Pair<OrderedSequence<T, V>, 
                           OrderedSequence<T, V>
                           >
                            (left, right);
            }

            public OrderedSequence<T, V> Insert(T t)
            {
                Pair<OrderedSequence<T, V>, OrderedSequence<T, V>> tPart
                    = Partition(KeyObj.KeyAssign(t));

                OrdElem<T, V> tOrd = new OrdElem<T,V>(t, KeyObj);

                return new
                 OrderedSequence<T, V>
                    (
                     KeyObj,
                     tPart.first.treeRep.Merge(tPart.second.treeRep.Push_Front(tOrd))
                    );

            }

            public OrderedSequence<T, V> DeleteAll(T t)
            {
                V vK = KeyObj.KeyAssign(t);  // the Key of t
                
                Pair<OrderedSequence<T, V>, OrderedSequence<T, V>> 
                    tPart = Partition(vK);

                OrderedSequence<T, V> seqPrecedestheEl = tPart.first;
                OrderedSequence<T, V> seqStartsWiththeEl = tPart.second;

                Pair<FTreeM<OrdElem<T, V>, V>, FTreeM<OrdElem<T, V>, V>> lastTreeSplit
               =
                seqStartsWiththeEl.treeRep.SeqSplit
                               (new MPredicate<V>
                                      (FP.Curry<V, V, bool>
                                        (theLTMethod2, vK)
                                       )
                                );

                //OrderedSequence<T, V> seqBeyondtheEl =
                //    new OrderedSequence<T, V>(KeyObj, lastTreeSplit.second);

                return new OrderedSequence<T, V>
                        (KeyObj,
                         seqPrecedestheEl.treeRep.Merge(lastTreeSplit.second)
                         );
            }

            public OrderedSequence<T, V> Merge(OrderedSequence<T, V> ordSeq2)
            {
                FTreeM<OrdElem<T, V>, V> theMergedTree
                    = OrdMerge(treeRep, ordSeq2.treeRep);

                return new OrderedSequence<T, V>
                         (KeyObj, theMergedTree);
            }
            
            private static FTreeM<OrdElem<T, V>, V>
                OrdMerge(FTreeM<OrdElem<T, V>, V> ordTree1,
                         FTreeM<OrdElem<T, V>, V> ordTree2
                         )
            {
                ViewL<OrdElem<T, V>, V> lView2 = ordTree2.LeftView();

                if (lView2 == null)
                    return ordTree1;
                //else
                OrdElem<T, V> bHead = lView2.head;
                FTreeM<OrdElem<T, V>, V> bTail = lView2.ftTail;

                // Split ordTree1 on elems <= and then > bHead
                Pair<FTreeM<OrdElem<T, V>, V>, FTreeM<OrdElem<T, V>, V>> 
                tree1Split = ordTree1.SeqSplit
                                      (new MPredicate<V>
                                           (FP.Curry<V, V, bool> (theLEMethod2, bHead.Measure()))
                                       );

                FTreeM<OrdElem<T, V>, V> leftTree1 = tree1Split.first;
                FTreeM<OrdElem<T, V>, V> rightTree1 = tree1Split.second;

                // OrdMerge the tail of ordTree2 
                //          with the right-split part of ordTree1
                FTreeM<OrdElem<T, V>, V> 
                    mergedRightparts = OrdMerge(bTail, rightTree1);

                return leftTree1.Merge(mergedRightparts.Push_Front(bHead));
            }

            public IEnumerable<T> ToSequence()
            {
                foreach (OrdElem<T, V> ordEl in treeRep.ToSequence())
                    yield return ordEl.Element;
            }
        }
}