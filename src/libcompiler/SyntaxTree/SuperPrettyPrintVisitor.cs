using System;
using System.Text;
using libcompiler.SyntaxTree;

namespace libcompiler.SyntaxTree
{
    public class SuperPrettyPrintVisitor : SyntaxVisitor
    {
        private StringBuilder _result;
        private StringBuilder _indentation;
        private readonly bool _singleLine;
        private readonly Func<CrawlSyntaxNode, string> _details;
        private readonly string[] _slimStrings = { "╾" , "├" , "└" };
        private readonly string[] _slimParts =  { "╾", "│", " " };
        private readonly string[] _fatStrings = { "╾┤" , "├┤" , "└┤" };
        private readonly string[] _fatParts = { "╾┤", "││", " │" };

        /// <summary>
        /// A class to pretty print a syntax tree
        /// </summary>
        /// <param name="singleLine">Print slim or fat nodes</param>
        /// <param name="details">A function to provide an additional line of details</param>
        public SuperPrettyPrintVisitor(bool singleLine, Func<CrawlSyntaxNode, string> details = null)
        {
            _singleLine = singleLine;
            _details = details;
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


            string detail = _details?.Invoke(node);
            if (_singleLine)
            {
                addSingleLine(node);
                if (detail != null)
                    addDetails(node, detail, detail.Length);

            }
            else
            {
                string nodeString = node.ToString();
                int width = Math.Max(Math.Max(nodeString.Length, detail?.Length ?? 0), 2);
                addUpperPart(node, width);

                addMiddlePart(node, nodeString, width);
                if (detail != null)
                    addDetails(node, detail, width);
                addLowerPart(node, width);
            }
            base.Visit(node);

            removeIndentation();
        }

        private void addDetails(CrawlSyntaxNode node, string detail, int width)
        {
            string[] prefixpart = _singleLine ? _slimParts : _fatParts;
            _result.Append(_indentation);
            _result.Append(GetParentBranch(node, prefixpart));
            _result.Append(detail);

            for (int i = detail.Length; i < width; i++)
                _result.Append(' ');


            if (!_singleLine)
                _result.Append('│');


            _result.Append('\n');

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
            _result.Append(GetParentBranch(node, _slimStrings));

            _result.Append(node);

            _result.Append('\n');
        }

        private string GetParentBranch(CrawlSyntaxNode node, string[] choices)
        {
            if (node.Parent == null)
            {
                return choices[0];
            }
            //If this is not the last sibling, a branch continues downwards
            else if (node.Parent.ChildCount - 1 > node.IndexInParent)
            {
                return choices[1];
            }
            else
            {
                return choices[2];
            }
        }

        private void addMiddlePart(CrawlSyntaxNode node, string text, int width)
        {
            _result.Append(_indentation);
            //└┤foo  │

            _result.Append(GetParentBranch(node, _fatStrings));

            _result.Append(text);
            for (int i = text.Length; i < width; i++)
                _result.Append(' ');
            
            _result.Append("│\n");
        }

        private void addUpperPart(CrawlSyntaxNode node, int width)
        {
            _result.Append(_indentation);
            //┌──────────────┐

            //If this is root, no branch goes down to it.
            if(node.Parent==null)
                _result.Append(" ┌──");
            else
                _result.Append("│┌──");

            for (int i = 0; i < width - 2; i++)
                _result.Append("─");

            _result.Append("┐\n");
        }

        private void addLowerPart(CrawlSyntaxNode node, int width)
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


            for (int i = 0; i < width - 2; i++)
                _result.Append("─");

            _result.Append("┘\n");
        }
    }
}