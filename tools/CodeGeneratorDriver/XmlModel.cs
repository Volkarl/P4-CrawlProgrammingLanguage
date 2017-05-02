using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGeneratorDriver
{
    [XmlRoot("syntaxnodes")]
    public class Model
    {
        [XmlElement("options")]
        public Options Options { get; set; }


        [XmlArray("nodes")]
        [XmlArrayItem("node")]
        public Node[] Nodes { get; set; }

    }

    public class Options
    {
        [XmlElement("basename")]
        public string BaseName { get; set; }

        [XmlElement("namespace")]
        public string NameSpace { get; set; }
        
        //s/<([^>]*)>.*/[XmlElement("$1")] public string $1 {get; set;}
        
        [XmlElement("parent")]
        public string Parent {get; set;}

        [XmlElement("createred")]
        public string CreateRed {get; set;}

        [XmlElement("getred")]
        public string GetRed { get; set; }

        [XmlElement("index")]
        public string Index {get; set;}
        
        [XmlElement("indexinparent")]
        public string IndexInParent {get; set;}

        [XmlElement("getchildat")]
        public string GetChildAt { get; set; }

        [XmlElement("invalidchildreplace")]
        public string InvalidChildReplace { get; set; }

        [XmlElement("with_replaced_child")]
        public string WithReplacedChild { get; set; }

        [XmlElement("new_child")]
        public string NewChild { get; set; }

        [XmlElement("self")]
        public string Self { get; set; }

        [XmlElement("visit")]
        public string Visit { get; set; }

        [XmlElement("node")]
        public string Node { get; set; }


        [XmlElement("combine")]
        public string Combine { get; set; }

        [XmlElement("update")]
        public string Update { get; set; }
    }

    public class Node
    {
        public Node BaseNode { get; set; }


        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("baseclass")]
        public string BaseClass { get; set; }

        [XmlElement("manual")]
        public bool Manual { get; set; } = false;

        [XmlElement("abstract")]
        public bool Abstract { get; set; } = false;


        [XmlIgnore]
        private List<Member> _allMembers;

        [XmlIgnore]
        public List<Member> AllMembers => _allMembers ?? (_allMembers =
                                              BaseNode  ?.  AllMembers.Concat(Members)
                                                  .ToList()
                                              ?? Members
                                          );

        [XmlIgnore]
        private List<Member> _members;

        [XmlIgnore]
        public List<Member> Members => _members ?? (_members =
                                           Properties
                                               .Select(p => new Member(p, this))
                                               .Concat(Children.Select(c => new Member(c, this)))
                                               .ToList());

        public TypeSyntax GetRepresentation(TypeClassContext context = TypeClassContext.None)
        {
            if ((context & TypeClassContext.NotList) != 0 && Name.StartsWith("List'"))
                return SyntaxFactory.ParseTypeName($"IEnumerable<{Name.Split('\'')[1]}Node>");


            return SyntaxFactory.ParseTypeName($"{Name}Node");
            throw new NotImplementedException();
        }

        [XmlArray("children")]
        [XmlArrayItem("child")]
        public Child[] Children { get; set; } = new Child[0];

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public Property[] Properties { get; set; } = new Property[0];

        [XmlElement("ctor")]
        public string ExtraConstructorCode { get; set; }

        [XmlElement("class")]
        public string ExtraClassCode { get; set; }

    }

    public class Property
    {
        [XmlElement("type")]
        public string Type { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

    }

    public class Child
    {
        [XmlElement("type")]
        public string Type { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("nulldefault")]
        public bool NullDefault { get; set; }
    }
}