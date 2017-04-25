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
        readonly Dictionary<string, TypeInformation[]> _scopeDictionary = new Dictionary<string, TypeInformation[]>();



        //checks if the blockNode is a variable, class or method decleration and adds them to the _scopeDictionary
        public BlockScope(BlockNode node)
        {
            foreach (var child in node)
            {
                if(child.Type == NodeType.VariableDecleration)
                {
                    VariableDeclerationNode variableNode = (VariableDeclerationNode)child;
                    foreach (var decleration in variableNode.Declerations)
                    {
                        string name = decleration.Identifier.Name;
                        _scopeDictionary.Add(name, new TypeInformation[1]);
                    }
                }
                else if(child.Type == NodeType.ClassDecleration)
                {
                    ClassDeclerationNode classNode = (ClassDeclerationNode)child;
                    string name = classNode.Identifier.Value;
                    _scopeDictionary.Add(name, new TypeInformation[1]);
                }
                else if(child.Type == NodeType.MethodDecleration)
                {
                    MethodDeclerationNode methodNode = (MethodDeclerationNode)child;
                    TypeInformation info = null;
                    string name = methodNode.Identfier.Value;
                    if (_scopeDictionary.ContainsKey(name))
                    {
                        //TODO: Check if type is the exact same, if yes, bail
                        TypeInformation[] old = _scopeDictionary[name];
                        TypeInformation[] new_ = new TypeInformation[old.Length + 1];
                        Array.Copy(old, new_, old.Length);
                        new_[old.Length] = info;
                        _scopeDictionary[name] = new_;
                    }
                    else
                    {
                        _scopeDictionary.Add(name, new [] { info });
                    }

                }
                //TODO: Constructor?
            }
        }
        //checks if the symbol is in the _scopeDictionary else return null
        public TypeInformation[] FindSymbol(string symbol)
        {
            TypeInformation[] typeArray;
            if (_scopeDictionary.TryGetValue(symbol, out typeArray) == true)
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
