using System;
using System.Collections;
using System.Collections.Generic;
using libcompiler.Parser;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using libcompiler.SyntaxTree;
using System.Linq;

namespace libcompiler.TypeChecker
{
    /// <summary>
    /// ExpressionEvaluator class goal is to look at alle the expression types and return the right type of an expression.  
    /// </summary>
    public class ExpressionEvaluator
    {
        /// <summary>
        /// Check if given parameters those required by method signature.
        /// </summary>
        private static bool ParameterMatch(CrawlMethodType methodSignature, params CrawlType[] givenParameters)
        {
            bool result = true;
            var methodParameters = methodSignature.Parameters.ToArray();

            if (givenParameters.Length != methodParameters.Length)
                return false;

            for (int i = 0; i < givenParameters.Length; i++)
            {
                result = result && givenParameters[i].ImplicitlyCastableTo(methodParameters[i]);
            }

            return result;
        }

        public static CrawlType Call(CrawlMethodType methodSignature, IEnumerable<CrawlType> actualParameters)
        {
                if (!ParameterMatch(methodSignature, actualParameters.ToArray()))
                    return CrawlType.ErrorType;
                else
                    return methodSignature.ReturnType;
        }

        /// <summary>
        /// Counts the number of the actual parameters' types that are Equal to those of the formal parameters.
        /// </summary>
        public static int DirectParameterMatches(CrawlMethodType methodSignature, List<CrawlType> actualParameters)
        {
            if(actualParameters.Count != methodSignature.Parameters.Count)
                throw new ArgumentException("Number of actual parameters does not match number required for given method signature.");

            int result = 0;
            List<CrawlType> formalParameters = methodSignature.Parameters;

            for (var i = 0; i < actualParameters.Count; i++)
            {
                if (actualParameters[i].Equals(formalParameters[i]))
                    result++;
            }

            return result;
        }

        public static CrawlType BestParameterMatch(List<CrawlMethodType> candidates, List<CrawlType> actualParameters)
        {
            //Only candidate(s) where actual parameters are assignable to formal.
            candidates = candidates.Where(x =>
                ExpressionEvaluator.CallParametersAreValid(x, actualParameters)
            ).ToList();

            if (candidates.Count == 1)
                return candidates.First();

            if (candidates.Count > 1)
            {
                //Only candidate(s) with highest number of matches.
                int highestNumberOfMatches = candidates.Max(x =>
                    ExpressionEvaluator.DirectParameterMatches(x, actualParameters)
                );
                candidates = candidates.FindAll(x =>
                    ExpressionEvaluator.DirectParameterMatches(x, actualParameters)
                    == highestNumberOfMatches
                );

                if (candidates.Count == 1)
                    return candidates.First();
                if(candidates.Count > 1)
                    return CrawlType.ErrorType;
                //if(candidates.Count < 1)
                    return CrawlType.ErrorType;
            }

            //if(candidates.Count<1)
            return CrawlType.ErrorType;
        }

        /// <summary>
        /// Whether actual parameters can be used for calling method of given signature.
        /// </summary>
        public static bool CallParametersAreValid(CrawlMethodType methodSignature, List<CrawlType> actualParameters)
        {
            return ! Call(methodSignature, actualParameters)
                .Equals(CrawlType.ErrorType);
        }


        public static CrawlType UnaryNot(CrawlType operand)
        {
            if (operand.Equals(CrawlSimpleType.Bool))
            {
                return CrawlSimpleType.Bool;
            }

            return CrawlType.ErrorType;
        }

        public static CrawlType UnaryNegate(CrawlType operand)
        {
            //If operand is tal, return tal; if operand is kommatal, return kommatal
            if (operand.Equals(CrawlSimpleType.Tal) || operand.Equals(CrawlSimpleType.Kommatal))
            {
                return operand;
            }
            return CrawlType.ErrorType;
        }
        /// <summary>
        /// A dictionary that maps a triple(typeA, typeB, binary_operator) to the type of the result
        /// of performing either typeA binary_operator typeB or typeB binary_operator typeA
        /// If the two types are different, they are written in no particular order, so be sure
        /// to check both combinations when looking them up.
        /// </summary>
        private static Dictionary<Tuple<CrawlType, CrawlType, ExpressionType>, CrawlType> BinaryExpressionDict
             = new Dictionary<Tuple<CrawlType, CrawlType, ExpressionType>, CrawlType>
             {
#region Greater
                {
                    _talTal(ExpressionType.Greater),
                    CrawlSimpleType.Bool
                },
                {
                    _kommaKomma(ExpressionType.Greater),
                    CrawlSimpleType.Bool
                },
                 {
                  _talKomma(ExpressionType.Greater),
                  CrawlSimpleType.Bool
                 },
#endregion
#region GreatEqual
                 {
                     _talTal(ExpressionType.GreaterEqual),
                     CrawlSimpleType.Bool
                 },
                 {
                    _kommaKomma(ExpressionType.GreaterEqual),
                    CrawlSimpleType.Bool
                },
                 {
                  _talKomma(ExpressionType.GreaterEqual),
                  CrawlSimpleType.Bool
                 },
#endregion
#region Less

                 {
                     _talTal(ExpressionType.Less),
                     CrawlSimpleType.Bool
                 },
                 {
                    _kommaKomma(ExpressionType.Less),
                    CrawlSimpleType.Bool
                 },
                 {
                  _talKomma(ExpressionType.Less),
                  CrawlSimpleType.Bool
                 },
#endregion
#region LessEqual
                 {
                     _talTal(ExpressionType.LessEqual),
                     CrawlSimpleType.Bool
                 },
                 {
                    _kommaKomma(ExpressionType.LessEqual),
                    CrawlSimpleType.Bool
                 },
                 {
                  _talKomma(ExpressionType.LessEqual),
                  CrawlSimpleType.Bool
                 },
#endregion
#region Equal
                 {
                     _talTal(ExpressionType.Equal),
                     CrawlSimpleType.Bool
                 },
                 {
                    _kommaKomma(ExpressionType.Equal),
                    CrawlSimpleType.Bool
                 },
                 {
                      _talKomma(ExpressionType.Equal),
                      CrawlSimpleType.Bool
                 },
                 {
                     _tekstTekst(ExpressionType.Equal),
                     CrawlSimpleType.Bool
                 },
                 {
                     _tegnTegn(ExpressionType.Equal),
                     CrawlSimpleType.Bool
                 },
#endregion
#region NotEqual

                 {
                     _talTal(ExpressionType.NotEqual),
                     CrawlSimpleType.Bool
                 },
                 {
                    _kommaKomma(ExpressionType.NotEqual),
                    CrawlSimpleType.Bool
                 },
                 {
                  _talKomma(ExpressionType.NotEqual),
                  CrawlSimpleType.Bool
                 },
                 #endregion
#region Add
                 {
                     _talTal(ExpressionType.Add),
                       CrawlSimpleType.Tal
                 },
                 {
                     _talKomma(ExpressionType.Add),
                        CrawlSimpleType.Kommatal
                 },
                 {
                     _tekstTekst(ExpressionType.Add),
                        CrawlSimpleType.Tekst
                 },
                 {
                     _kommaKomma(ExpressionType.Add),
                        CrawlSimpleType.Kommatal
                 },
                 {
                     _tegnTegn(ExpressionType.Add),
                        CrawlSimpleType.ErrorType
                 },
#endregion
#region Subtract
                 {
                     _talTal(ExpressionType.Subtract),
                        CrawlSimpleType.Tal
                 },

                 {
                    _kommaKomma(ExpressionType.Subtract),
                        CrawlSimpleType.Kommatal
                 },

                 {
                     _talKomma(ExpressionType.Subtract),
                        CrawlSimpleType.Kommatal
                 },
                 {
                    _tekstTekst(ExpressionType.Subtract),
                    CrawlSimpleType.ErrorType 
                 }
#endregion Subtract

             };
    

        private static Dictionary<Tuple<CrawlType, CrawlType, ExpressionType>, CrawlType> _MultipleOperator
            = new Dictionary<Tuple<CrawlType, CrawlType, ExpressionType>, CrawlType>
            {
         
                
            };
      


        public static CrawlType Evaluate(CrawlType leftSide, ExpressionType oprator, CrawlType rightSide)
        {
            var firstCombi = new Tuple<CrawlType, CrawlType, ExpressionType >(leftSide, rightSide, oprator);
            if (_MultipleOperator.ContainsKey(firstCombi))
                {
                    return _MultipleOperator[firstCombi];
                }

             var secondCombi = new Tuple<CrawlType, CrawlType, ExpressionType>(rightSide, leftSide, oprator);
            if (_MultipleOperator.ContainsKey(secondCombi))
                {
                     return _MultipleOperator[secondCombi];
                }
            else
                     return CrawlType.ErrorType;
            
        }

        public static CrawlType EvaluateMultiExpression(ExpressionType expressionType, List<CrawlType> operands)
        {
           
            #region 2 possible soulution 
            CrawlType resultingType = Evaluate(operands[0], expressionType, operands[1]);
            
            for (var i=2; i<operands.Count; i+=2)
             {
                 resultingType= Evaluate(resultingType, expressionType, operands[i]);

             }
            return resultingType;
            
            #endregion

        }



        // The different combinations with the specific binary Expression
        private static Tuple<CrawlType, CrawlType, ExpressionType> _talTal(ExpressionType eType)
        {
            return new Tuple<CrawlType, CrawlType, ExpressionType>(CrawlSimpleType.Tal, CrawlSimpleType.Tal, eType);
        }

        private static Tuple<CrawlType, CrawlType, ExpressionType> _kommaKomma(ExpressionType eType)
        {
            return new Tuple<CrawlType, CrawlType, ExpressionType>(CrawlSimpleType.Kommatal, CrawlSimpleType.Kommatal, eType);
        }

        private static Tuple<CrawlType, CrawlType, ExpressionType> _talKomma(ExpressionType eType)
        {
            return new Tuple<CrawlType, CrawlType, ExpressionType>(CrawlSimpleType.Tal, CrawlSimpleType.Kommatal, eType);
        }

        private static Tuple<CrawlType, CrawlType, ExpressionType> _tekstTekst(ExpressionType eType)
        {
            return new Tuple<CrawlType, CrawlType, ExpressionType>(CrawlSimpleType.Tekst, CrawlSimpleType.Tekst, eType);
        }

        private static Tuple<CrawlType, CrawlType, ExpressionType> _tegnTegn(ExpressionType eType)
        {
            return new Tuple<CrawlType, CrawlType, ExpressionType>(CrawlSimpleType.Tegn, CrawlSimpleType.Tegn, eType);
        }



        /// <summary>
        /// Returns result type of binary expression. This may be the error type.
        /// </summary>
        public static CrawlType EvaluateBinaryType(CrawlType operand1, ExpressionType oprator, CrawlType operand2)
        {
            //Valid binary expressions are stored in BinaryExpressionDict.
            //To avoid redundancy, {tal > kommatal => bool} is saved,
            //but {kommatal > tal => bool} is not.
            //Instead we check if either of the two possible combinations
            //of two operators are stored in the dictionary. 

            var firstPossibleMatch =
                new Tuple<CrawlType, CrawlType, ExpressionType>(operand1, operand2, oprator);

            if (BinaryExpressionDict.ContainsKey(firstPossibleMatch))
            {
                return BinaryExpressionDict[firstPossibleMatch];
            }

            var secondPossibleMatch =
                new Tuple<CrawlType, CrawlType, ExpressionType>(operand2, operand1, oprator);

            if (BinaryExpressionDict.ContainsKey(secondPossibleMatch))
            {
                return BinaryExpressionDict[secondPossibleMatch];
            }
            else
                return CrawlType.ErrorType;
        }

        




    }
}