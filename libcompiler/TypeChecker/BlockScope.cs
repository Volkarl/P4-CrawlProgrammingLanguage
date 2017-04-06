using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libcompiler.SyntaxTree;
using libcompiler.SyntaxTree.Nodes;

namespace libcompiler.TypeChecker
{
    class BlockScope : IScope
    {
        private readonly Dictionary<string, TypeInformation[]> _scopeDictionary =
            new Dictionary<string, TypeInformation[]>();

        //checks subnodes to see if they are variable-, class- or method-declerations and adds them to the _scopeDictionary
        public BlockScope(BlockNode node)
        {
            foreach (var child in node)
            {
                if(child.Type == NodeType.VariableDecleration)
                {
                    VariableDeclerationNode variableDeclerationNode = (VariableDeclerationNode)child;

                    foreach (var decleration in variableDeclerationNode.Declerations)
                    {
                        string name = decleration.Identifier.Name;
                        TypeInformation[] typeInformation = new TypeInformation[1]
                            {new TypeInformation(variableDeclerationNode.DeclerationType.ExportedType)};

                        _scopeDictionary.Add(name, typeInformation);
                    }
                }
                else if(child.Type == NodeType.ClassDecleration)
                {
                    ClassDeclerationNode classNode = (ClassDeclerationNode) child;
                    string name = classNode.Identifier.Value;
                    _scopeDictionary.Add(name, new TypeInformation[1]);
                }
                else if(child.Type == NodeType.MethodDecleration)
                {
                    MethodDeclerationNode methodNode = (MethodDeclerationNode) child;
                    string name = methodNode.Identfier.Name;
                    _scopeDictionary.Add(name, new TypeInformation[1]);
                }
            }
        }
        //checks if the identifier is in the _scopeDictionary else return null
        public TypeInformation[] GetScope(string identifier)
        {
            TypeInformation[] typeArray;
            if (_scopeDictionary.TryGetValue(identifier, out typeArray) == true)
            {
                return typeArray;
            }
            else
            {
                return null;
            }
        }
    }
}
