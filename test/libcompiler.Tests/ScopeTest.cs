/*using System;
using libcompiler.SyntaxTree;
using NUnit.Framework;
using static libcompiler.Tests.Helpers;

namespace libcompiler.Tests
{
    [TestFixture]
    class ScopeTest
    {
        //TODO: Missing tests for
        //That the returned information is valid (its not atm)
        //Scope/Visibility of exported types/variables of multiple files
        //Use of stdlib types/methods are visible
        //Use of variable defined later fails.
        //Multiple method variables with same name but different type fails
        //Generics? (Probably another file really)
        //Test parameters visible
        //error when declaring variable that is also parameter


        /// <summary>
        /// Tests that a variable is visible
        /// </summary>
        [TestCase]
        public void TestScopeFoundVariable()
        {
            TranslationUnitNode tree = ReadTestFile("scopetest1.crawl");
            Assert.NotNull(tree.Code.GetScope("abe"));   
        }

        /// <summary>
        /// Tests that a method is visible
        /// </summary>
        [TestCase]
        public void TestScopeFoundMethod()
        {
            TranslationUnitNode tree = ReadTestFile("scopetest1.crawl");
            Assert.NotNull(tree.Code.GetScope("UdregnOrangutanger"));
        }

        /// <summary>
        /// Tests that an error occurs if a single decleration tries to define multiple variables
        /// </summary>
        [TestCase]
        public void TestScopeMultipleVariableSingleLineError()
        {
            Assert.That(() => ReadTestFile("scopetest2.notcrawl"), Throws.Exception);
        }

        //TODO: version with different types
        //TODO: version with 1 method type, 1 normal
        /// <summary>
        /// Tests that an error occours if multiple declerations tries to define the same variablen twice
        /// </summary>
        [TestCase]
        public void TestScopeMultipleVariableMultiLineError()
        {
            Assert.That(() => ReadTestFile("scopetest3.notcrawl"), Throws.Exception);
        }

        //TODO: Version with method defined in parrent
        //TODO: Version with class defined in parrent
        /// <summary>
        /// Tests that it raises an error if a method declares a variable already defined in parrent.
        /// </summary>
        [TestCase]
        public void TestScopeMultipleVariableParrentError()
        {
            Assert.That(() => ReadTestFile("scopetest4.notcrawl"), Throws.Exception);
        }

        /// <summary>
        /// Test that multiple methods in a class passes
        /// </summary>
        [TestCase]
        public void TestClassMultipleMethodDefinitions()
        {
            TranslationUnitNode tree = ReadTestFile("scopetest5.crawl");
            Assert.Inconclusive("No way to check classes for stuff");
        }

        /// <summary>
        /// Tests that multiple methods can be defined in a class
        /// </summary>
        [TestCase]
        public void TestClassMultipleMethodDefinitionsDiffernetParamterCount()
        {
            TranslationUnitNode tree = ReadTestFile("scopetest6.crawl");
            Assert.Inconclusive("No way to check classes for stuff");
        }

        //TODO: different visibility should also fail
        /// <summary>
        /// Tests that multiple methods with same signature cannot be declared in a class
        /// </summary>
        [TestCase]
        public void TestClassMultipleMethodDefinitionsExactSignature()
        {
            TranslationUnitNode tree = ReadTestFile("scopetest7.notcrawl");
            Assert.Inconclusive("No way to check classes for stuff");
        }

        /// <summary>
        /// Tests that multiple methods with the same signature but differing return types cannot see eachothers intern variables
        /// </summary>
        [TestCase]
        public void TestClassMultipleMethodDefinitionsExactSignatureButReturnType()
        {
            TranslationUnitNode tree = ReadTestFile("scopetest8.crawl");
            Assert.Inconclusive("No way to check classes for stuff");
        }

        [TestCase]
        public void TestFindingParametersInScope()
        {
            TranslationUnitNode tree = ReadTestFile("scopetest11.crawl");
            MethodDeclerationNode method = (MethodDeclerationNode) tree.Code[0];
            Assert.NotNull(method.FindFirstScope().GetScope("a"));

        }

        /// <summary>
        /// Test that a missing symbol returns null
        /// </summary>
        [TestCase]
        public void TestScopeMissingReturnsNull()
        {
            TranslationUnitNode tree = ReadTestFile("scopetest1.crawl");
            Assert.IsNull(tree.Code.GetScope("notdefined"));
        }

        /// <summary>
        /// Test that variables declared in children aren't visible
        /// </summary>
        [TestCase]
        public void TestScopeNotFindingChildren()
        {
            TranslationUnitNode tree = ReadTestFile("scopetest1.crawl");
            Assert.IsNull(tree.Code.GetScope("orangutanger"));
        }

        //TODO: Test duplicate method with changed signature cannot see eachothers variables
        /// <summary>
        /// Test that variables in adjacent scopes aren't visible
        /// </summary>
        [TestCase]
        public void TestScopeNotFindingAdjacent()
        {
            TranslationUnitNode tree = ReadTestFile("scopetest9.crawl");
            MethodDeclerationNode m = (MethodDeclerationNode) tree.Code[1];
            Assert.IsNull(m.BodyBlock.GetScope("z"));

        }

        //TODO: test variables
        /// <summary>
        /// Tests that inside a method, other methods in the same scope as method, is visible
        /// </summary>
        [TestCase]
        public void TestScopeFindingOther()
        {
            TranslationUnitNode tree = ReadTestFile("scopetest9.crawl");
            MethodDeclerationNode m = (MethodDeclerationNode)tree.Code[1];
            Assert.NotNull(m.BodyBlock.GetScope("a"));

        }

        /// <summary>
        /// Tests that symbols is defined parrent is visible
        /// </summary>
        [TestCase]
        public void TestScopeParrentDefined()
        {
            TranslationUnitNode tree = ReadTestFile("scopetest1.crawl");
            MethodDeclerationNode method = (MethodDeclerationNode) tree.Code[1];
            BlockNode block = method.BodyBlock;
            Assert.NotNull(block.GetScope("abe"));
        }

        /// <summary>
        /// Tests that a method is visible inside itself
        /// </summary>
        [TestCase]
        public void TestScopeMethodsFindSelf()
        {
            TranslationUnitNode tree = ReadTestFile("scopetest1.crawl");
            MethodDeclerationNode method = (MethodDeclerationNode)tree.Code[1];
            BlockNode block = method.BodyBlock;
            Assert.NotNull(block.GetScope("UdregnOrangutanger"));
        }

        //TODO: Test private isn't
        /// <summary>
        /// Tests that a parrent class is visible for child class
        /// </summary>
        [TestCase]
        public void TestScopeDerivedClass()
        {
            TranslationUnitNode tree = ReadTestFile("scopetest10.crawl");

            ClassDeclerationNode childClass = (ClassDeclerationNode)tree.Code[1];
            Assert.NotNull(childClass.BodyBlock.GetScope("inparrent"));
        }
    }
}
*/