using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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

        public BlockScope()
        { }

        public CrawlConstructedType AddClass(ClassTypeDeclerationNode node, string ns)
        {
            DeclaringScope declaringScope = DeclaringScope.MethodLike;

            if (node.Parent is ClassTypeDeclerationNode || node.Parent is TranslationUnitNode)
            {
                declaringScope = DeclaringScope.ClassLike; //Workingish, parrent == TranslationUnit probably going to require a little special handling
            }

            string name = node.Identifier.Value;
            CrawlConstructedType type = new CrawlConstructedType(name, ns);
            TypeInformation info = new TypeInformation(type, node.ProtectionLevel, node.Interval.a, new UniqueItem(),declaringScope, DeclaredAs.Class);

            //Update contents. If not existing add a new array (line 2)
            //Otherwise, linq to append to array
            _scopeDictionary.AddOrUpdate(
                name, 
                new[] {info},
                (s, informations) => informations.Concat(info.AsSingleIEnumerable()).ToArray()
            );
            _classes.Add(type);
            return type;
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

                        decleration.Identifier.UniqueItemTracker.Item = new UniqueItem();
                        string name = decleration.Identifier.Name;
                        scope.Add(name,
                            new TypeInformation(
                                variableNode.DeclerationType.ActualType,
                                variableNode.ProtectionLevel,
                                decleration.Interval.a, decleration.Identifier.UniqueItemTracker.Item,
                                declaringScope));

                    }
                }
                else if(child.Type == NodeType.MethodDecleration)
                {
                    MethodDeclerationNode methodNode = (MethodDeclerationNode) child;
                    methodNode.UniqueItemTracker.Item = new UniqueItem();
                    string name = methodNode.Identifier.Value;
                    scope.Add(name,
                        new TypeInformation(
                            methodNode.MethodSignature.ActualType,
                            methodNode.ProtectionLevel,
                            methodNode.Interval.a,
                            methodNode.UniqueItemTracker.Item,
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
