This document _briefly_ gives an overview on how the Cräwlpiler (Crawl compiler) is put together.

#Overview

The compiler (Structurally speaking) is made up of 2 different categories. Transformations and synchronization points.

A transformation is a function^1 that takes in _something_ and outputs _something_. This could be the same thing, [AST] in, [AST] out. Some don't even modify their data, but just performs some kind of analysis on it.
A transformation don't have any dependencies except for the stage before it, and can be run at any time.

A synchronization point on the other hand is a point where all transformation's before it is finished and some kind of computation is done across all files.
For an idea how this works, the figure below should give an idea.

    /-------\ [filename] > Read parse tree -> Create AST -> Collect visible symbols > [AST] /---------\ [AST] -> find all visible symbols -> decorate TypeNode's 
	|       | [filename] > Read parse tree -> Create AST -> Collect visible symbols > [AST] |  Merge  | [AST] -> find all visible symbols -> decorate TypeNode's 
	| Input | [filename] > Read parse tree -> Create AST -> Collect visible symbols > [AST] | visible | [AST] -> find all visible symbols -> decorate TypeNode's 
	|       | [filename] > Read parse tree -> Create AST -> Collect visible symbols > [AST] | Symbols | [AST] -> find all visible symbols -> decorate TypeNode's 
	| Files | [filename] > Read parse tree -> Create AST -> Collect visible symbols > [AST] |   in    | [AST] -> find all visible symbols -> decorate TypeNode's 
	|       | [filename] > Read parse tree -> Create AST -> Collect visible symbols > [AST] | files   | [AST] -> find all visible symbols -> decorate TypeNode's 
	\_______/ [filename] > Read parse tree -> Create AST -> Collect visible symbols > [AST] \_________/ [AST] -> find all visible symbols -> decorate TypeNode's 
	
As screens generally scroll downwards and because it would take forever, above figure only contains the start of the compiler. 
The actual stages is described below.

>1: there was supposed to be a note here, keept in place in case i remember it...

#Stages

##*synchronization point*
##_transformation_

###*Start*
This is the start of compilation. Other than a few options, only thing present is the filenames that make up the program and referenced .dll files (Assemblies).

###_Create parse tree_
A file is read into memory and fed to Antlr which produces a parse tree.

###_Create AST_
The parse trees are transformed from concrete syntax trees into abstract syntax trees.

###_Collect visible symbols_
Scope checker does its first pass on the file and collects everything in the top level scope.
At time of writing, this is actually done by ParseTreeParser.TranslationUnit but conceptually it is a different stage
It's just that nobody have bothered to split it out in its own method.

###*Merge visible symbols*
Here all files that share a namespace is joined together to create a Namespace scope. Said data is also merged with namespace's from referenced assemblies.

###_Find all visible symbols_
All imported modules are merged into one big scope that contains everything visible that isn't defined inside the file.

###_Find scope information_
The AST is traversed and all(not quite yet, but in theory) defined variables gets saved in scopes attached to relevant Syntax Nodes

###*Currently unused synchronization point*
At time of writing, constructed types aren't handled. 
When scope information is collected, if it encounters a class, it just notes its name and puts an entry in the type table that basically says "There will be a type of this name in the future, pinkie swear" 
Maybe this will require actual single threaded handling. 
Maybe later stages will depend on the _real_ types existing.
This synchronization point exists for that.

###_Order check_
Every symbol is checked against every other symbol. If a collision is detected, a warning is emitted.
An enterprising soul could optimize this to keep running track of visible symbols instead of rebuilding the table of _ALL_ visible symbols in each scope, as current code is O(lots)

###_Decorate TypeNode_
The Tree is traversed (again...) and all TypeNodes has the actual type they refer to inserted. (Well, some of them just gets the pinkie swear type)
This _could_ be done inside the typechecker, but it is probably going to complicate it, depending on how variables and pinkie swear types are handled. (I haven't given this much thought, I might need to look in a book and see how other compilers handle this)

#Implementation
The implementation has some requirements and some other wishes.
Among those are

* Thread safe
* Reasonable amounts of repeated boiler plate code
* Switchable between multithreaded and single theaded
* A way to get diagnostic information (error messages out)

This is archived in large by *function composition* (Technically method composition)
Every transformation is implemented as a static function^2, that takes input for the transformation and returns the result.
Then by the magic of generics and lambda functions, 2 transformations can be joined into one big transformation that takes the input of the first transformation and returns the output of the second on.
Said process can be repeated until only big stage exists. 
This transformation is then finished with a method that instead of returning, puts its input into a thread safe collection

    List<foo> list = new List<foo>();  //Not actually a list in the code
	Action<bar> = input => list.Add(third(second(first(input, helper), helper), helper))

This stage is then executed on every file, but a method that either runs it one by one or in parallel. 

>2: This is a lie. There are places where the function is placed on an object to give them access to shared (read only) data 





	




