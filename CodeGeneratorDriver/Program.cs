using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;

namespace CodeGeneratorDriver
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string path = Path.Combine(
                // ReSharper disable once AssignNullToNotNullAttribute
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "SyntaxNodeDefinitions.xml");

            SyntaxGeneration syntaxGeneration = ReadDefinition(path);

            Workspace workspace = new AdhocWorkspace();
            ;
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(workspace, LanguageNames.CSharp);

            foreach (Node node in syntaxGeneration.Node)
            {
                SyntaxNode file = Formatter.Format(
                    GreenNodeGenerator.CreateGreenNode(generator, node, syntaxGeneration.Options),
                    workspace);

                Console.WriteLine(file);
            }

            Console.ReadLine();

        }

        private static SyntaxGeneration ReadDefinition(string path)
        {
            var nodes = (SyntaxGeneration)new XmlSerializer(typeof(SyntaxGeneration)).Deserialize(XmlReader.Create(File.OpenRead(path)));

            var dictionary = nodes.Node.ToDictionary(x => x.Name);

            foreach (Node node in nodes.Node)
            {
                if(node.BaseClass == null && node.Name == nodes.Options.BaseName) continue;

                node.BaseNode = dictionary[node.BaseClass];
            }


            return nodes;
        }
    }
}