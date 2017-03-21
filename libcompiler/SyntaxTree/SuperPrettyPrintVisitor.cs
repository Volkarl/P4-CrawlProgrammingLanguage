using System.Text;
using libcompiler.SyntaxTree.Nodes;

namespace libcompiler.SyntaxTree
{
    public class SuperPrettyPrintVisitor : SyntaxTreeVistitor
    {
        private StringBuilder _result;
        private StringBuilder _indentation;
        private readonly bool _singleLine;

        public SuperPrettyPrintVisitor(bool singleLine)
        {
            _singleLine = singleLine;
        }

        public string PrettyPrint(CrawlSyntaxNode node)
        {
            _result = new StringBuilder();
            _indentation = new StringBuilder();
            Visit(node);
            return _result.ToString();
        }

        public override void Visit(CrawlSyntaxNode node)
        {
            addIndentation(node);

            if (_singleLine)
            {
                addSingleLine(node);
            }
            else
            {
                addUpperPart(node);
                addMiddlePart(node);
                addLowerPart(node);
            }
            base.Visit(node);

            removeIndentation();
        }

        private void addIndentation(CrawlSyntaxNode node)
        {
            //If this is root, add no indentation
            if (node.Parent == null)
                ;
            //If parent is not the last sibling, a branch continues downwards
            else if(node.Parent.Parent?.ChildCount - 1 > node.Parent.IndexInParent)
            {
                _indentation.Append("│ ");
            }
            else
            {
                _indentation.Append("  ");
            }
        }

        private void removeIndentation()
        {
            //Unless this is root...
            if(_indentation.Length>0)
                //...Indentation is always 2 characters
                _indentation.Remove(_indentation.Length - 2, 2);
        }

        private void addSingleLine(CrawlSyntaxNode node)
        {
            _result.Append(_indentation);
            //├Foo

            //If this is root, no branch goes down to it
            if (node.Parent == null)
            {
                _result.Append("╾");
            }
            //If this is not the last sibling, a branch continues downwards
            else if (node.Parent.ChildCount - 1 > node.IndexInParent)
            {
                _result.Append("├");
            }
            else
            {
                _result.Append("└");
            }

            _result.Append(node);
            _result.Append('\n');
        }

        private void addUpperPart(CrawlSyntaxNode node)
        {
            _result.Append(_indentation);
            //┌──────────────┐

            //If this is root, no branch goes down to it.
            if(node.Parent==null)
                _result.Append(" ┌──");
            else
                _result.Append("│┌──");

            for (int i = 0; i < node.ToString().Length; i++)
                _result.Append("─");

            _result.Append("┐\n");
        }

        private void addMiddlePart(CrawlSyntaxNode node)
        {
            _result.Append(_indentation);
            //└┤foo  │

            //If this is root, no branch goes down to it.
            if (node.Parent == null)
            {
                _result.Append("╾┤");
            }
            //If this is not the last sibling, a branch continues downwards
            else if (node.Parent.ChildCount - 1 > node.IndexInParent)
            {
                _result.Append("├┤");
            }
            else
            {
                _result.Append("└┤");
            }

            _result.Append(node.ToString());

            _result.Append("  │\n");
        }


        private void addLowerPart(CrawlSyntaxNode node)
        {
            _result.Append(_indentation);
            //└─┬─────┘


            //If this is not the last sibling, a branch continues downwards
           if (node.Parent?.ChildCount - 1 > node.IndexInParent)
            {
                _result.Append("│");
            }
            else
            {
                _result.Append(" ");
            }
            //If this is a leaf no branch down is needed.
            if (node.ChildCount == 0)
                _result.Append("└──");
            else
                _result.Append("└┬─");


            for (int i = 0; i < node.ToString().Length; i++)
                _result.Append("─");

            _result.Append("┘\n");
        }
    }
}