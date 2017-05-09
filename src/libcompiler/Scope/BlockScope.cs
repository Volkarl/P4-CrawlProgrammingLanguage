using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using libcompiler.Datatypes;
using libcompiler.ExtensionMethods;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;

namespace libcompiler.Scope
{
    public class BlockScope : IScope
    {
        readonly ConcurrentDictionary<string, TypeInformation[]> _scopeDictionary = new ConcurrentDictionary<string, TypeInformation[]>();
        readonly List<CrawlConstructedType> _classes = new List<CrawlConstructedType>();

        //checks if the blockNode is a variable, class or method decleration and adds them to the scopeDictionary    
        public BlockScope(BlockNode node)
        {

            string ns = node.FindNameSpace()?.Module ?? "";

            DeclaringScope declaringScope = DeclaringScope.MethodLike;
            
            if (node.Parent is ClassTypeDeclerationNode)
            {
                declaringScope = DeclaringScope.ClassLike;

            }
            ListDictionary<string, TypeInformation> scope = new ListDictionary<string, TypeInformation>();
            foreach (var child in node)
            {
                if(child.Type == NodeType.ClassTypeDecleration)
                {
                    ClassTypeDeclerationNode classNode = (ClassTypeDeclerationNode) child;
                    string name = classNode.Identifier.Value;
                    CrawlConstructedType type = new CrawlConstructedType(name, ns);
                    scope.Add(name, new TypeInformation(type, classNode.ProtectionLevel, classNode.Interval.a, declaringScope, NeedsABetterNameType.Class));
                    _classes.Add(type);

                }
            }

            foreach (KeyValuePair<string,List<TypeInformation>> pair in scope)
            {
                //TODO: This will fail silently if 2 classes with same name exists (i think)
                _scopeDictionary.TryAdd(pair.Key, pair.Value.ToArray());
            }
        }

        private BlockScope(
            ConcurrentDictionary<string, TypeInformation[]> scopeDictionary,
            List<CrawlConstructedType> classes)
        {
            _scopeDictionary = scopeDictionary;
            _classes = classes;
        }

        public BlockScope CollectIdentifiers(BlockNode block)
        {
            DeclaringScope declaringScope = DeclaringScope.MethodLike;

            if (block.Parent is ClassTypeDeclerationNode)
            {
                declaringScope = DeclaringScope.ClassLike;
            }

            ListDictionary<string, TypeInformation> scope = new ListDictionary<string, TypeInformation>();
            foreach (var child in block)
            {
                if(child.Type == NodeType.VariableDecleration)
                {
                    VariableDeclerationNode variableNode = (VariableDeclerationNode)child;
                    foreach (var decleration in variableNode.Declerations)
                    {
                        string name = decleration.Identifier.Name;
                        scope.Add(name,
                            new TypeInformation(
                                variableNode.DeclerationType.ActualType,
                                variableNode.ProtectionLevel,
                                decleration.Interval.a,
                                declaringScope));
                    }
                }
                else if(child.Type == NodeType.MethodDecleration)
                {
                    MethodDeclerationNode methodNode = (MethodDeclerationNode) child;
                    string name = methodNode.Identifier.Value;
                    scope.Add(name,
                        new TypeInformation(
                            methodNode.MethodSignature.ActualType,
                            methodNode.ProtectionLevel,
                            methodNode.Interval.a,
                            declaringScope));
                }
            }

            ConcurrentDictionary<string, TypeInformation[]> newscope = new ConcurrentDictionary<string, TypeInformation[]>(_scopeDictionary);
            foreach (KeyValuePair<string,List<TypeInformation>> pair in scope)
            {
                newscope.TryAdd(pair.Key, pair.Value.ToArray());
            }
            return new BlockScope(newscope, _classes);
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

        public IEnumerable<CrawlConstructedType> Classes() => _classes;
    }
}
