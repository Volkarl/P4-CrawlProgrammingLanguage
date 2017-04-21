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
        Dictionary<string, TypeInformation[]> scopeDictionary = new Dictionary<string, TypeInformation[]>();
        //checks if the blockNode is a variable, class or method decleration and adds them to the scopeDictionary    
        public BlockScope(BlockNode node)
        {
            foreach (var child in node)
            {
                if(child.Type == NodeType.VariableDecleration)
                {
                    VariableDeclerationNode variableNode = (VariableDeclerationNode)child;
                    foreach (var Decleration in variableNode.Declerations)
                    {
                        string name = Decleration.Identifier.Name;
                        scopeDictionary.Add(name, new TypeInformation[1]);
                    }
                }
                else if(child.Type == NodeType.ClassDecleration)
                {
                    ClassDeclerationNode classNode = (ClassDeclerationNode)child;
                    string name = classNode.Identifier.Value;
                    scopeDictionary.Add(name, new TypeInformation[1]);
                }
                else if(child.Type == NodeType.MethodDecleration)
                {
                    MethodDeclerationNode methodNode = (MethodDeclerationNode)child;
                    TypeInformation info = null;
                    string name = methodNode.Identfier.Name;
                    if (scopeDictionary.ContainsKey(name))
                    {
                        //TODO: Check if type is the exact same, if yes, bail
                        TypeInformation[] old = scopeDictionary[name];
                        TypeInformation[] new_ = new TypeInformation[old.Length + 1];
                        Array.Copy(old, new_, old.Length);
                        new_[old.Length] = info;
                        scopeDictionary[name] = new_;
                    }
                    else
                    {
                        scopeDictionary.Add(name, new [] { info });
                    }
                    
                }
                //TODO: Constructor?
            }
        }
        //checks if the symbol is in the scopeDictionary else return null
        public TypeInformation[] GetScope(string symbol)
        {
            TypeInformation[] typeArray;
            if (scopeDictionary.TryGetValue(symbol, out typeArray) == true)
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
