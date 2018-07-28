# P4-CrawlProgrammingLanguage

## Cräwl
A programming language meant as an intermediary language with low complexity to help bridge the gap between introductory languages, such as Scratch block programming, to professional object-oriented languages. 
It is a strongly typed, statement-based, general purpose language, that supports constructed types and inheritance. The language uses Danish keywords and is designed such that features can be introduced only as needed in the learning curve. 
Additionally, Cräwl allows for importing libraries from the .NET framework, which can be used so long as the Cräwl syntax is sufficient to interact with them.

## The compiler
Compiles Cräwl to C#, at which point the Roslyn compiler then takes over. Lexing and scanning is done using the compiler generator tool ANTLR, while type checking, code generation, code optimization, etc, is done by us. A big feature of this project is that it uses code generation to create the code for the visitor pattern, which is later used for creating the abstract syntax tree. This visitor pattern is created using nodes defined in the file [SyntaxNodeDefinitions.xml](P4-CrawlProgrammingLanguage/src/libcompiler/SyntaxTree/SyntaxNodeDefinitions.xml), which allowed us to quickly change the nodes without having to re-do much manual work regarding the syntax tree. 

------------------------------------------------------

Created as part of the 4th semester project of software group sw409f17, Comp. Sci, Aalborg University, during spring/summer of 2017.

Group members:
- Andreas L. Hald 
- Asger H. Brorholt
- Johannes Elgaard
- Jonathan Karlsson
- M. Ibrahim Kohistani

