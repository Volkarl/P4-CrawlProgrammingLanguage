using System;
using System.Collections;
using System.Collections.Generic;
using libcompiler.Parser;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;
using System.Collections.Generic;
using libcompiler.SyntaxTree;

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

        public static CrawlType Call(CrawlMethodType methodSignature, params CrawlType[] givenParameters)
        {
            CrawlType result = null;
                if (!ParameterMatch(methodSignature, givenParameters))
                    return CrawlType.ErrorType;
                else
                    return methodSignature.ReturnType;
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
        /// A dictionary containing a Tuplw With CrawlType, CrawlType and Expressiontype. 
        /// The output shall be a new CrawlSimpleType or somekind of error
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
             };


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