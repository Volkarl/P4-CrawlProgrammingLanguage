using System.Linq;
using System.Text;
using libcompiler.SyntaxTree;
using libcompiler.SyntaxTree.Nodes;

namespace libcompiler
{
    
    public class SyntaxTreePrinter : SyntaxTreeVistitor
    {
        public StringBuilder BuildString = new StringBuilder();

        private void AddSpaces(int count)
        {
            while (count >= 5)
            {
                BuildString.Append(' ', 4);
                BuildString.Append('|');
                count -= 5;
            }
            BuildString.Append(' ', count);
        } 
        
        private string spaces =
            "                                                                                       ";
        private int indent = 0;
        private int Lenght = -1;
        public override void Visit(CrawlSyntaxNode node)
        {

            Indent();
            Lenght = BuildString.Length;
            //AddSpaces(indent);
            BuildString.Append(node.ToString());

            
            base.Visit(node);
            Dedent();
        }

        private bool last = false;
        private int suppressCount = 0;

        private void Dedent()
        {
            indent--;
            if(last)
            {
                BuildString.Append("\n");
                AddSpaces(indent * 2);
            }
            if (suppressCount > 0)
                suppressCount--;
            else
                BuildString.Append(")");

            last = true;
        }

        private void Indent()
        {
            BuildString.Append('\n');
            AddSpaces(indent * 2);
            BuildString.Append('(');
            indent++;
            last = false;
        }

        protected override void VisitLiteral(LiteralNode node)
        {
            SupressParens();
            base.VisitLiteral(node);
        }

        private void SupressParens()
        {
            for (int i = BuildString.Length - 1; i >= 0; i--)
            {
                if (BuildString[i] == '(')
                {
                    BuildString.Remove(i, 1);
                    break;
                }
            }
            //BuildString.Length--;
            suppressCount++;
        }

        protected override void VisitVariableDecleration(VariableDeclerationNode node)
        {
            BuildString.Append(' ');
            BuildString.Append(node.DeclerationType.ExportedType.Textdef);

            if (node.Declerations.ChildCount == 1)
            {
                SingleVariableDecleration decleration = node.Declerations.First();
                BuildString.Append(' ');
                BuildString.Append(decleration.Identifier);
                if (decleration.DefaultValue != null)
                    Visit(decleration.DefaultValue);
            } else 
            foreach (SingleVariableDecleration decleration in node.Declerations)
            {
                Indent();
                BuildString.Append(decleration.Identifier);
                
                if(decleration.DefaultValue != null)
                    Visit(decleration.DefaultValue);
                Dedent();
            }
        }
    }
}
