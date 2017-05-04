using System;
using libcompiler.TypeSystem;

namespace libcompiler.TypeChecker
{
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

        public static CrawlType BinaryGreater(CrawlType leftOperand, CrawlType rightOperand)
        {
            throw new NotImplementedException();
        }

        public static CrawlType BinaryGreaterEqual(CrawlType leftOperand, CrawlType rightOperand)
        {
            throw new NotImplementedException();
        }

        public static CrawlType BinaryEqual(CrawlType leftOperand, CrawlType rightOperand)
        {
            throw new NotImplementedException();
        }

        public static CrawlType BinaryNotEqual(CrawlType leftOperand, CrawlType rightOperand)
        {
            throw new NotImplementedException();
        }

        public static CrawlType BinaryLessEqual(CrawlType leftOperand, CrawlType rightOperand)
        {
            throw new NotImplementedException();
        }

        public static CrawlType BinaryLess(CrawlType leftOperand, CrawlType rightOperand)
        {
            throw new NotImplementedException();
        }

        public static CrawlType BinaryPower(CrawlType leftOperand, CrawlType rightOperand)
        {
            throw new NotImplementedException();
        }

        public static CrawlType BinaryRange(CrawlType leftOperand, CrawlType rightOperand)
        {
            throw new NotImplementedException();
        }

        public static CrawlType BinaryShortCircuitOr(CrawlType leftOperand, CrawlType rightOperand)
        {
            throw new NotImplementedException();
        }

        public static CrawlType BinaryShortCircuitAnd(CrawlType leftOperand, CrawlType rightOperand)
        {
            throw new NotImplementedException();
        }
    }
}