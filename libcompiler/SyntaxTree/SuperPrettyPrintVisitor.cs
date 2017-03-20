using System.Text;
using libcompiler.SyntaxTree.Nodes;

namespace libcompiler.SyntaxTree
{
    public class SuperPrettyPrintVisitor : SyntaxTreeVistitor
    {
        private StringBuilder _result;
        private StringBuilder _indentation;

        public string PrettyPrint(CrawlSyntaxNode node)
        {
            _result = new StringBuilder();
            _indentation = new StringBuilder();
            Visit(node);
            return _result.ToString();
        }

        public override void Visit(CrawlSyntaxNode node)
        {
            _addIndentation(node);

            _addMiddleBit(node);
            base.Visit(node);

            _removeIndentation();
        }

        private void _addIndentation(CrawlSyntaxNode node)
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

            _result.Append(_indentation);
        }

        private void _removeIndentation()
        {
            //Unless this is root...
            if(_indentation.Length>0)
                //...Indentation is always 2 characters
                _indentation.Remove(_indentation.Length - 2, 2);
        }

        private void _addMiddleBit(CrawlSyntaxNode node)
        {
            //If this is root, no branch goes down to it
            if (node.Parent == null)
            {
                _result.Append("│");
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
    }
}