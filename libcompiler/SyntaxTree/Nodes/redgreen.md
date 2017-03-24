This is a short document detailing how a the internal datastructure for the ast(abstract syntax tree) works. 
The idea/data structure is stolen from the C# compiler platfom, [Roslyn](https://github.com/dotnet/roslyn)
This is mostly throught links, but it also contains a _brief_ description.

The advantages of red-green trees are that
* They are immutable.
* It is cheap to move both upwards and downwards in the tree, without requring housekeeping elsewhere.
* Takes form of a tree.
* Mostly reusable.

This is achieved by not using a single tree.
Instead 2 trees are used. There are the red public tree and the green private tree.

The *Green* tree is a true, immutable data structure that is build bottom up representing the ast. 
They are reusable, only one path up to the root needs to be changed to replace a node, while staying immutable.
Each green node keeps an immutable list (usually not actually a list) of their child nodes and thus provides a single source of truth on how the tree looks.

To archive the second part, easy moving upwards we hide the green tree behind a *Red* tree. The first red node is created with a refernce to the topmost green node.
Then, as the consumer of the red trees moves down the *red* tree, nodes are created as required. On this second stage, each *red* node is respobsible for creating its
children. Therefore, red nodes knows who their parrents are and can refernce them.

Thus, from the *green* nodes it is possible to move down the tree, and moving up is possible with the *red* nodes. 
Most of the green nodes can be reused when a change is made, and red nodes are generated as needed. 
It is not known how large a portion of red nodes are generated compared to green nodes in practice.

[Persistence, Facades and Roslyn’s Red-Green Trees](https://blogs.msdn.microsoft.com/ericlippert/2012/06/08/persistence-facades-and-roslyns-red-green-trees/)
[Roslyn's performance](https://roslyn.codeplex.com/discussions/541953)
[Stackoverflow question on Roslyn](http://stackoverflow.com/a/25981934)