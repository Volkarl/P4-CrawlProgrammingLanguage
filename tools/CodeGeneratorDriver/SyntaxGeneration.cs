﻿using System;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGeneratorDriver
{
    [XmlRoot("syntaxnodes")]
    public class SyntaxGeneration
    {
        [XmlElement("options")]
        public SyntaxGenerationOptions Options { get; set; }


        [XmlArray("nodes")]
        [XmlArrayItem("node")]
        public Node[] Node { get; set; }

    }

    public class SyntaxGenerationOptions
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
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("baseclass")]
        public string BaseClass { get; set; }

        public Node BaseNode { get; set; }


        [XmlArray("children")]
        [XmlArrayItem("child")]
        public Child[] Children { get; set; } = new Child[0];

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public Property[] Properties { get; set; } = new Property[0];

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
    }
}