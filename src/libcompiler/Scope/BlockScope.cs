using System.Collections.Generic;
using System.Linq;
using libcompiler.Datatypes;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;

namespace libcompiler.Scope
{
    public class BlockScope : IScope
    {
        readonly Dictionary<string, TypeInformation[]> _scopeDictionary;
        //checks if the blockNode is a variable, class or method decleration and adds them to the scopeDictionary    
        public BlockScope(BlockNode node)
        {
            DeclaringScope declaringScope = DeclaringScope.MethodLike;
            
            if (node.Parent is ClassTypeDeclerationNode)
            {
                declaringScope = DeclaringScope.ClassLike;

            }
            ListDictionary<string, TypeInformation> scope = new ListDictionary<string, TypeInformation>();
            foreach (var child in node)
            {
                if(child.Type == NodeType.VariableDecleration)
                {
                    VariableDeclerationNode variableNode = (VariableDeclerationNode)child;
                    foreach (var decleration in variableNode.Declerations)
                    {
                        string name = decleration.Identifier.Name;
                        scope.Add(name, new TypeInformation(new FutureType(name, ""), variableNode.ProtectionLevel, decleration.Interval.a, declaringScope));
                    }
                }
                else if(child.Type == NodeType.ClassTypeDecleration)
                {
                    ClassTypeDeclerationNode classNode = (ClassTypeDeclerationNode) child;
                    string name = classNode.Identifier.Value;
                    scope.Add(name, new TypeInformation(new FutureType(name, ""), classNode.ProtectionLevel, classNode.Interval.a, declaringScope));
                }
                else if(child.Type == NodeType.MethodDecleration)
                {
                    MethodDeclerationNode methodNode = (MethodDeclerationNode) child;
                    string name = methodNode.Identifier.Value;
                    scope.Add(name, new TypeInformation(new FutureType(name, ""), methodNode.ProtectionLevel, methodNode.Interval.a, declaringScope));;
                }
            }

            _scopeDictionary = scope.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
        }
        //checks if the symbol is in the scopeDictionary else return null
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

        public IEnumerable<string> LocalSymbols() => _scopeDictionary.Keys;
    }
}
