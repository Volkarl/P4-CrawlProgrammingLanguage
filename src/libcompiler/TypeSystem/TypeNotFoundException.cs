using System;

namespace libcompiler.TypeSystem
{
    public class TypeNotFoundException : Exception
    {
        public string Type { get; }

        public TypeNotFoundException(string type)
        {
            Type = type;
        }
    }
}