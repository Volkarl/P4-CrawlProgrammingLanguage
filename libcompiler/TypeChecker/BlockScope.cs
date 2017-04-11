using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libcompiler.SyntaxTree;
using libcompiler.SyntaxTree.Nodes;
using libcompiler.TypeSystem;

namespace libcompiler.TypeChecker
{
    class BlockScope : IScope
    {
        private readonly Dictionary<string, TypeInformation[]> _scopeDictionary;
        private readonly BlockNode _node;

        private readonly object _lock = new object();
        private volatile bool _variablesRead;

        public BlockScope(BlockNode node)
        {
            /*
             * Scope is constructed in 2 steps. First all types are read in,
             * then on demand (first access) variables are also resolved. It 
             * is done this way to ensure that all types exist before trying
             * to resolve them (to know what type the variables are). 
             */
            _node = node;
            _scopeDictionary = ReadAllTypeDefinitions(node);
        }
        //checks if the identifier is in the _scopeDictionary else return null
        public TypeInformation[] GetScope(string identifier)
        {
            //If first time getting scope, read every variable. Structure to only lock on generation
            if (_variablesRead == false)
            {
                lock (_lock)
                {
                    if (_variablesRead == false)
                    {
                        _variablesRead = true;
                        ArgumentWithVariables();
                    }
                }
            }

            //Do the actual lookup
            TypeInformation[] typeArray;
            if (_scopeDictionary.TryGetValue(identifier, out typeArray))
            {
                return typeArray;
            }
            else
            {
                return null;
            }
        }

        public Dictionary<string, TypeInformation[]> ReadAllTypeDefinitions(BlockNode node)
        {
            Dictionary<string, TypeInformation[]> dictionary = new Dictionary<string, TypeInformation[]>();
            foreach (var child in node)
            {
                if (child.Type == NodeType.ClassDecleration)
                {
                    ClassDeclerationNode classNode = (ClassDeclerationNode) child;
                    string name = classNode.Identifier.Value;
                    dictionary.Add(name, new TypeInformation[1]);
                }
            }

            return dictionary;
        }

        public void ArgumentWithVariables()
        {
            List<KeyValuePair<string, TypeInformation[]>> all = new List<KeyValuePair<string, TypeInformation[]>>();
            foreach (CrawlSyntaxNode child in _node)
            {
                if (child.Type == NodeType.VariableDecleration)
                {
                    VariableDeclerationNode variableDeclerationNode = (VariableDeclerationNode)child;

                    foreach (var decleration in variableDeclerationNode.Declerations)
                    {
                        string name = decleration.Identifier.Name;
                        TypeInformation[] typeInformation = new TypeInformation[1]
                            {new TypeInformation(CrawlType.ParseDecleration(_node, name))};

                        all.Add(new KeyValuePair<string, TypeInformation[]>(name, typeInformation));
                    }
                }
                else if (child.Type == NodeType.MethodDecleration)
                {
                    MethodDeclerationNode methodNode = (MethodDeclerationNode)child;
                    string name = methodNode.Identfier.Name;
                    all.Add(new KeyValuePair<string, TypeInformation[]>(name, new TypeInformation[1]));
                }
            }

            foreach (var valuePair in all)
            {
                //TODO: Handle duplication gracefully.
                //Best is (probably) to record all and save them in a structure containing where it is defined
                //then throw AggregateException or a custom CrawlLotsOfErrorsException.
                _scopeDictionary.Add(valuePair.Key, valuePair.Value);
            }
        }
    }
}
