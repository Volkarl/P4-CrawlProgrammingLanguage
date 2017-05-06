using System;
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
        /// The output shall be a new Crawltype. 
        /// </summary>
        private static Dictionary<Tuple<CrawlType, CrawlType, ExpressionType>, CrawlType> BinaryExpressinDict
             = new Dictionary<Tuple<CrawlType, CrawlType, ExpressionType>, CrawlType>
             {
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
                 }

            };

        /// <summary>
        /// The differient combinations with the specifik binary Expression Greater.
        /// </summary>
        /// <param name="eType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// This methods purpose is to Look at the right and left side and the swith the left and right operand to to try the other 
        /// combination as well. And then return the type 
        /// </summary>
        /// <param name="leftOperand"></param>
        /// <param name="oprator"></param>
        /// <param name="rightOperand"></param>
        /// <returns></returns>
        public static CrawlType EvaluateBinaryType(CrawlType leftOperand, ExpressionType oprator, CrawlType rightOperand)
        {
            

            if (BinaryExpressinDict.ContainsKey(new Tuple<CrawlType,CrawlType,ExpressionType>(leftOperand, rightOperand, oprator)))
            {
                return BinaryExpressinDict[new Tuple<CrawlType, CrawlType, ExpressionType>(leftOperand, rightOperand, oprator)];
            }
            if (BinaryExpressinDict.ContainsKey(new Tuple<CrawlType, CrawlType, ExpressionType>(rightOperand, leftOperand, oprator)))
            {
                return BinaryExpressinDict[new Tuple<CrawlType, CrawlType, ExpressionType>(rightOperand, leftOperand, oprator)];
            }
            else
                return CrawlType.ErrorType;
            
        }
        
        

    }
}