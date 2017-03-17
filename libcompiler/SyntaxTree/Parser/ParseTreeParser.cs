using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using libcompiler.ExtensionMethods;
using libcompiler.Parser;
using libcompiler.SyntaxTree.Nodes;

namespace libcompiler.SyntaxTree.Parser
{
    public static class ParseTreeParser
    {
        public static FlowNode ParseFlow(RuleContext rule)
        {
            if (rule.RuleIndex == CrawlParser.RULE_for_loop)
            {
                return ParseFor(rule);
            }
            else if (rule.RuleIndex == CrawlParser.RULE_if_selection)
            {
                return ParseIf(rule);
            }
            else if (rule.RuleIndex == CrawlParser.RULE_while_loop)
            {
                return ParseWhile(rule);
            }
            throw new NotImplementedException("while loop not implemented");
        }

        private static FlowNode ParseWhile(RuleContext rule)
        {
            ExpressionNode condition = ExpressionParser.ParseExpression((RuleContext) rule.GetChild(1));
            BlockNode block = ParseBlockNode((RuleContext) rule.GetChild(3));

            return NodeFactory.WhileLoop(rule.SourceInterval, condition, block);
            throw new NotImplementedException();
        }

        private static FlowNode ParseIf(RuleContext rule)
        {
            //IF expression INDENT statements DEDENT (ELSE INDENT statements DEDENT)?;
            ExpressionNode expression = ExpressionParser.ParseExpression((RuleContext)rule.GetChild(1));
            BlockNode trueBlock = ParseBlockNode((RuleContext) rule.GetChild(3));

            //Pure if
            if (rule.ChildCount == 5)
            {
                return NodeFactory.If(rule.SourceInterval, expression, trueBlock);
            }
            //If Else
            else if(rule.ChildCount == 9)
            {
                BlockNode falseBlock = ParseBlockNode((RuleContext) rule.GetChild(7));
                return NodeFactory.IfElse(rule.SourceInterval, expression, trueBlock, falseBlock);
            }
            //Else if
            else if (rule.ChildCount == 7)
            {
                RuleContext chain = (RuleContext) rule.GetChild(6);
                List<CrawlSyntaxNode> blockContents = new List<CrawlSyntaxNode>(){ParseIf(chain)};
                BlockNode falseBlock = NodeFactory.Block(chain.SourceInterval, blockContents);
                return NodeFactory.IfElse(rule.SourceInterval, expression, trueBlock, falseBlock);
            }

            throw new NotImplementedException("if statement with strange argument counts (this is probably else if)");
        }

        private static FlowNode ParseFor(RuleContext rule)
        {
            //FOR type IDENTIFIER FOR_LOOP_SEPERATOR expression INDENT statements DEDENT
            ITerminalNode forNode = (ITerminalNode)rule.GetChild(0);
            CrawlParser.TypeContext typeNode = (CrawlParser.TypeContext) rule.GetChild(1);
            ITerminalNode identifierNode = (ITerminalNode)rule.GetChild(2);
            ITerminalNode serperatorNode = (ITerminalNode)rule.GetChild(3);
            RuleContext expression = (RuleContext)rule.GetChild(4);
            ITerminalNode indent = (ITerminalNode)rule.GetChild(5);
            RuleContext blockCtx = (RuleContext)rule.GetChild(6);
            ITerminalNode dedent = (ITerminalNode)rule.GetChild(7);

            TypeNode type = ParseType(typeNode);
            ExpressionNode iteratior = ExpressionParser.ParseExpression(expression);
            BlockNode block = ParseBlockNode(blockCtx);

            return NodeFactory.Forloop(rule.SourceInterval, type, ParseVariableNode(identifierNode), iteratior, block);
        }

        public static DeclerationNode ParseDeclerationNode(RuleContext rule)
        {
            ProtectionLevel protectionLevel = ProtectionLevel.None;
            RuleContext declpart;
            if (rule.GetChild(0) is CrawlParser.Protection_levelContext)
            {
                protectionLevel = ParseProtectionLevel((CrawlParser.Protection_levelContext) rule.GetChild(0));

                declpart = (RuleContext)rule.GetChild(1);
            }
            else
            {
                declpart = (RuleContext)rule.GetChild(0);
            }


            if (declpart.RuleIndex == CrawlParser.RULE_class_declaration)
            {
                return ParseClassDecleration(declpart, protectionLevel, rule.SourceInterval);
            }
            if (declpart.RuleIndex == CrawlParser.RULE_function_decleration)
            {
                return ParseFunctionDecleration(declpart, protectionLevel, rule.SourceInterval);
            }
            else if (declpart.RuleIndex == CrawlParser.RULE_variable_declerations)
            {
                return ParseVariableDecleration(declpart, protectionLevel, rule.SourceInterval);
            }
            
            throw new NotImplementedException("unknown decleration type");
        }

        #region DeclerationSubs


        private static DeclerationNode ParseFunctionDecleration(RuleContext classPart, ProtectionLevel protectionLevel, Interval interval)
        {
            TypeNode type = ParseType((CrawlParser.TypeContext) classPart.GetChild(0));
            ITerminalNode identifier = (ITerminalNode) classPart.GetChild(1);
            ITerminalNode assignment = (ITerminalNode) classPart.GetChild(2);
            RuleContext body = (RuleContext) classPart.GetChild(3).GetChild(1);

            //Not sure if this can ever happen, but if it happens something went really wrong
            if (identifier.Symbol.Type != CrawlLexer.IDENTIFIER) throw new CrawlImpossibleStateException("Unexpected symbol", interval);
            if (assignment.Symbol.Type != CrawlLexer.ASSIGNMENT_SYMBOL) throw new CrawlImpossibleStateException("Unexpected symbol", interval);

            return NodeFactory.Function(interval, protectionLevel, type, ParseVariableNode(identifier), ParseBlockNode(body));
        }

        private static VariableNode ParseVariableNode(ITerminalNode node)
        {
            return NodeFactory.VariableNode(node.SourceInterval, node.GetText());
        }

        private static TokenNode ParseTokenNode(ITerminalNode node)
        {
            return NodeFactory.TokenNode(node.SourceInterval, node.GetText());
        }

        private static DeclerationNode ParseVariableDecleration(RuleContext classPart, ProtectionLevel protectionLevel, Interval interval)
        {
            //This is the whole variable decleration. First the type, then individual variables of that type, with an optional initialization value
            //Each individual identifier is parsed in ParseSingleVariable
            ITerminalNode eos =  classPart.LastChild() as ITerminalNode;
            if(eos == null || eos.Symbol.Type != CrawlLexer.END_OF_STATEMENT) throw new NotImplementedException("Something strange happened");

            TypeNode type = ParseType((CrawlParser.TypeContext) classPart.GetChild(0));

            return NodeFactory.VariableDecleration(
                interval,
                protectionLevel,
                type,
                classPart
                    .AsEdgeTrimmedIEnumerable()
                    .OfType<CrawlParser.Variable_declContext>()
                    .Select(ParseSingleVariable));
        }

        private static SingleVariableDecleration ParseSingleVariable(CrawlParser.Variable_declContext variable)
        {
            ITerminalNode identifier = (ITerminalNode) variable.GetChild(0);
            if(identifier.Symbol.Type != CrawlLexer.IDENTIFIER) throw new NotImplementedException();

            //unitialized
            if (variable.ChildCount == 1)
            {
                return NodeFactory.SingleVariable(variable.SourceInterval, ParseVariableNode(identifier));
            }
            //initialized
            else if (variable.ChildCount == 3)
            {
                return NodeFactory.SingleVariable(variable.SourceInterval, ParseVariableNode(identifier),
                    ExpressionParser.ParseExpression((RuleContext) variable.GetChild(2)));
            }

            throw new NotImplementedException("Variable declared in strange way");
        }


        private static DeclerationNode ParseClassDecleration(RuleContext classPart, ProtectionLevel protectionLevel, Interval interval)
        {
            ITerminalNode tn1 = (ITerminalNode)classPart.GetChild(0);
            ITerminalNode tn2 = (ITerminalNode)classPart.GetChild(1);
            RuleContext body = (RuleContext) classPart.GetChild(3);


            if(classPart.ChildCount != 4) throw new NotImplementedException("No class inheritance");
            if(tn1.Symbol.Type != CrawlLexer.CLASS) throw new CrawlImpossibleStateException("Trying to parse a class that is not a class", interval);

            BlockNode bodyBlock = ParseBlockNode(body);

            return NodeFactory.ClassDecleration(interval, protectionLevel, ParseTokenNode(tn2), bodyBlock);
        }

        #endregion

        private static TypeNode ParseType(CrawlParser.TypeContext type)
        {
           return NodeFactory.Type(type.SourceInterval, new CrawlType(type.GetText()));
        }

        private static ProtectionLevel ParseProtectionLevel(CrawlParser.Protection_levelContext protectionLevel)
        {
            TerminalNodeImpl tnode = (TerminalNodeImpl) protectionLevel.GetChild(0);

            switch (tnode.Payload.Type)
            {
                case CrawlLexer.PUBLIC:
                    return ProtectionLevel.Public;
                case CrawlLexer.PRIVATE:
                    return ProtectionLevel.Private;
                case CrawlLexer.PROTECTED:
                    return ProtectionLevel.Protected;
                case CrawlLexer.INTERNAL:
                    return ProtectionLevel.Internal;
                case CrawlLexer.PROTECTED_INTERNAL:
                    return ProtectionLevel.ProtectedInternal;
                default:
                    throw new CrawlImpossibleStateException("Unknown protection level", protectionLevel.SourceInterval);
            }   
        }

        public static BlockNode ParseBlockNode(RuleContext rule)
        {
            System.Collections.IEnumerable meaningfullContent;

            if (rule.RuleIndex == CrawlParser.RULE_statements)
            {
                meaningfullContent = rule.AsIEnumerable();
            }
            else if (rule.RuleIndex == CrawlParser.RULE_class_body)
            {
                meaningfullContent = rule.AsEdgeTrimmedIEnumerable();
            }
            else throw new NotImplementedException("Probably broken");

            IEnumerable<CrawlSyntaxNode> contents =
                meaningfullContent
                    .OfType<RuleContext>() //FIXME! This can contain raw NEWLINE and END_OF_STATEMENT tokens which is TerminalNodeImpl not RuleContext. Not happy about discarding that trivia
                    .Select(ParseStatement).ToList();

            return NodeFactory.Block(rule.SourceInterval, contents);
        }

        public static ImportNode ParseImportNode(RuleContext rule)
        {
            throw new NotImplementedException();
        }

        public static CrawlSyntaxNode ParseStatement(RuleContext rule)
        {
            switch (rule.RuleIndex)
            {
                case CrawlParser.RULE_declaration:
                    return ParseDeclerationNode(rule);
                case CrawlParser.RULE_side_effect_stmt:
                    return ExpressionParser.ParseSideEffectStatement(rule);
                case CrawlParser.RULE_for_loop:
                case CrawlParser.RULE_if_selection:
                case CrawlParser.RULE_while_loop:
                    return ParseFlow(rule);
                case CrawlParser.RULE_assignment:
                    return ParseAssignment(rule);
                case CrawlParser.RULE_return_statement:
                    return ParseReturn(rule);
                default:
                    throw new NotImplementedException();
            }
        }

        private static CrawlSyntaxNode ParseReturn(RuleContext rule)
        {
            
            if (rule.ChildCount == 3)
            {
                ExpressionNode retvalue = ExpressionParser.ParseExpression((RuleContext) rule.GetChild(1));
                return NodeFactory.Return(rule.SourceInterval, retvalue);
            }
            else
            {
                return NodeFactory.Return(rule.SourceInterval);
            }
        }

        private static CrawlSyntaxNode ParseAssignment(RuleContext rule)
        {

            ExpressionNode target = ExpressionParser.ParseExpression((RuleContext)rule.GetChild(0));
            if(rule.GetChild(1) is CrawlParser.Subfield_expressionContext)
            {
                CrawlParser.Subfield_expressionContext subfield = (CrawlParser.Subfield_expressionContext) rule.GetChild(1);

                VariableNode sub = NodeFactory.VariableAccess(subfield.GetChild(1).SourceInterval, subfield.GetChild(1).GetText());
                target = NodeFactory.MemberAccess(subfield.SourceInterval, target, sub);
            }
            else if(rule.GetChild(1) is CrawlParser.Index_expressionContext)
            {
                RuleContext idx = (RuleContext) rule.GetChild(1);
                target = NodeFactory.Index(idx.SourceInterval, target, ExpressionParser.ParseCallTail(idx));
            }

            ExpressionNode value = ExpressionParser.ParseExpression((RuleContext) rule.GetChild(rule.ChildCount - 2));
            return NodeFactory.Assignment(rule.SourceInterval, target, value);
        }

        public static CompiliationUnitNode ParseTranslationUnit(CrawlParser.Translation_unitContext translationUnit)
        {
            CrawlParser.Import_directivesContext imports =
                (CrawlParser.Import_directivesContext)translationUnit.GetChild(0);

            CrawlParser.StatementsContext statements =
                (CrawlParser.StatementsContext)translationUnit.GetChild(1);


            BlockNode rootBlock = ParseBlockNode(statements);

            return NodeFactory.CompilationUnit(translationUnit.SourceInterval, new List<ImportNode>(), rootBlock);
        }
    }
}