using System;
using System.Collections.Generic;
using System.Threading;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using libcompiler.SyntaxTree.Internal;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public abstract class CrawlSyntaxNode
    {
        private readonly GreenNode _green;

        /// <summary>
        /// The <see cref="CrawlSyntaxTree"/> this Node belongs to. 
        /// </summary>
        public CrawlSyntaxTree OwningTree { get; }

        /// <summary>
        /// The parrent <see cref="CrawlSyntaxNode"/> of this node. It is null if this is a root node
        /// </summary>
        public CrawlSyntaxNode Parrent { get; }

        /// <summary>
        /// The position where this node appear in the parrent
        /// </summary>
        public int IndexInParrent { get; }
        
        /// <summary>
        /// The <see cref="NodeType"/> of this <see cref="CrawlSyntaxNode"/>. 
        /// It is unique to most <see cref="CrawlSyntaxNode"/> with the 
        /// exception being <see cref="ExpressionNode"/> which also contains
        /// an <see cref="ExpressionType"/>
        /// </summary>
        public NodeType Type { get; }

        /// <summary>
        /// The Interval this <see cref="CrawlSyntaxNode"/> covers in the source code
        /// <b>NOTICE: This API element is not stable and might change</b>
        /// </summary>
        public Interval Interval => _green.Interval;

        public int ChildCount => _green.ChildCount;

        protected internal CrawlSyntaxNode(CrawlSyntaxNode parrent, GreenNode self, int slot)
        {
            if(!(parrent is SyntaxNodeTreeInjector))
                Parrent = parrent;

            OwningTree = parrent?.OwningTree;
            if (OwningTree == null)
            {
                OwningTree = new CrawlSyntaxTree(this, "<Unknown>");

            }
            _green = self;
            //GreenNodes sometimes uses upper bits to encode extra information. Not allowed here
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            Type = self.Type & (NodeType) 0xff;
            IndexInParrent = slot;

        }

        protected internal CrawlSyntaxNode(CrawlSyntaxTree tree, GreenNode self, int slot)
        {
            OwningTree = tree;
            _green = self;
            Parrent = null;
            Type = self.Type;
            IndexInParrent = slot;
        }

        protected T GetRed<T>(ref T field, int slot) where T : CrawlSyntaxNode
        {
            T result = field;

            if (result == null)
            {
                GreenNode green = this._green.GetSlot(slot);
                if (green != null)
                {
                    Interlocked.CompareExchange(ref field, (T)green.CreateRed(this, slot), null);
                    result = field;
                }
            }

            return result;
        }

        internal static GreenNode ExtractGreenNode(CrawlSyntaxNode node) => node?._green;

        public abstract CrawlSyntaxNode GetChild(int index);

        public CrawlSyntaxNode Translplant(CrawlSyntaxNode replacement)
        {

            Stack<int> parrentIndex = new Stack<int>();
            int count = 0;
            GreenNode toInsert = replacement._green;
            CrawlSyntaxNode self = this;
            while (self.Parrent != null)
            {
                parrentIndex.Push(self.IndexInParrent);
                toInsert = self.Parrent._green.WithReplacedChild(toInsert, self.IndexInParrent);
                self = self.Parrent;
                count++;
            }

            CrawlSyntaxNode newRoot = toInsert.CreateRed(null, 0);
            while (parrentIndex.Count != 0)
            {
                newRoot = newRoot.GetChild(parrentIndex.Pop());
                if(newRoot == null) throw new Exception();
            }

            return newRoot;
        }
    }
}