﻿using System;
using System.Collections.Generic;
using System.Threading;
using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    /// <summary>
    /// The root class for all Red nodes (See redgreen.md). Most generic 
    /// functionality is located here. Rest is (at time of writing) in 
    /// <see cref="ListNode{T}"/> and <see cref="ExpressionNode"/>
    /// </summary>
    public abstract class CrawlSyntaxNode
    {
        private readonly GreenNode _green;
        private CrawlSyntaxTree _owningTree;

        /// <summary>
        /// The <see cref="CrawlSyntaxTree"/> this Node belongs to. 
        /// </summary>
        public CrawlSyntaxTree OwningTree
        {
            get
            {
                //If we already know what tree owns this node, great
                if (_owningTree != null) return _owningTree;

                //otherwise we to find it.
                //If ask our parent for it. If we don't have a parent, make one up.
                CrawlSyntaxTree newTree = Parent == null ? new CrawlSyntaxTree(this, "<Unknown>") : Parent.OwningTree;

                //Save it thread safe
                Interlocked.CompareExchange(ref _owningTree, newTree, null);

                //Return what actually got saved
                return _owningTree;
            }
        }

        /// <summary>
        /// The parent <see cref="CrawlSyntaxNode"/> of this node. It is null if this is a root node
        /// </summary>
        public CrawlSyntaxNode Parent { get; }

        /// <summary>
        /// The position where this node appear in the parent
        /// </summary>
        public int IndexInParent { get; }
        
        /// <summary>
        /// The <see cref="NodeType"/> of this <see cref="CrawlSyntaxNode"/>. 
        /// It is unique to most types of <see cref="CrawlSyntaxNode"/> and in some
        /// cases one <see cref="CrawlSyntaxNode"/> will represent different 
        /// on what value <see cref="Type"/> has.
        /// </summary>
        public NodeType Type { get; }

        /// <summary>
        /// The Interval this <see cref="CrawlSyntaxNode"/> covers in the source code
        /// <b>NOTICE: This API element is not stable and might change</b>
        /// </summary>
        public Interval Interval => _green.Interval;

        /// <summary>
        /// The number of children this node has.
        /// </summary>
        public int ChildCount => _green.ChildCount;

        protected internal CrawlSyntaxNode(CrawlSyntaxNode parent, GreenNode self, int slot)
        {
            Parent = parent;

            _green = self;
            //GreenNodes sometimes uses upper bits to encode extra information. Not allowed here
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            //TODO: I don't think this is used anymore
            Type = self.Type & (NodeType) 0xff;
            IndexInParent = slot;

        }

        /// <summary>
        /// This is one of the big pieces of the puzzle of how the Red side of the tree.
        /// It tries to take the <paramref name="slot"/> child and either return the
        /// coresponding child or create it. This value is cached in <paramref name="field"/>.
        /// This would be rather simple to do, except that this needs to be done
        /// <b>thread safe</b>.
        /// </summary>
        /// <typeparam name="T">The type of child</typeparam>
        /// <param name="field">A field to cache the value in.</param>
        /// <param name="slot">The index of the child</param>
        /// <returns>A child node, either a new or from cache.</returns>
        protected T GetRed<T>(ref T field, int slot) where T : CrawlSyntaxNode
        {
            T result = field;

            if (result == null)
            {
                GreenNode green = this._green.GetChildAt(slot);
                if (green != null)
                {
                    Interlocked.CompareExchange(ref field, (T)green.CreateRed(this, slot), null);
                    result = field;
                }
            }

            return result;
        }

        internal static GreenNode ExtractGreenNode(CrawlSyntaxNode node) => node?._green;

        public abstract CrawlSyntaxNode GetChildAt(int index);

        public CrawlSyntaxNode Translplant(CrawlSyntaxNode replacement)
        {

            Stack<int> parrentIndex = new Stack<int>();
            int count = 0;
            GreenNode toInsert = replacement._green;
            CrawlSyntaxNode self = this;
            while (self.Parent != null)
            {
                parrentIndex.Push(self.IndexInParent);
                toInsert = self.Parent._green.WithReplacedChild(toInsert, self.IndexInParent);
                self = self.Parent;
                count++;
            }

            CrawlSyntaxNode newRoot = toInsert.CreateRed(null, 0);
            while (parrentIndex.Count != 0)
            {
                newRoot = newRoot.GetChildAt(parrentIndex.Pop());
                if(newRoot == null) throw new Exception();
            }

            return newRoot;
        }
    }
}