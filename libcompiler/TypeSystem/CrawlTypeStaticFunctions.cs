using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace libcompiler.TypeSystem
{
    public abstract partial class CrawlType
    {
        public static CrawlType ParseDecleration(string name)
        {
            if (name.EndsWith("]"))
            {

            }


            BuildInType returnvalue;
            if (buildInTypes.TryGetValue(name, out returnvalue))
            {
                return returnvalue;
            }
            throw new NotImplementedException();
        }

        private static readonly ConcurrentDictionary<string, BuildInType> buildInTypes;

        static CrawlType()
        {
            buildInTypes = new ConcurrentDictionary<string, BuildInType>();


            buildInTypes.TryAdd("tekst", new BuildInType(typeof(string), "tekst", new[]
            {
                new KeyValuePair<string, string>("Længde", "Length"),
            }));

        }
    }
}