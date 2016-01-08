﻿using NUnitLite;

namespace Invenietis.DependencySolver.Core.Tests
{
    public class Program
    {
        public static int Main(string[] args)
        {
#if DNX451 || DNX46
            return new AutoRun().Execute( args );
#else
            return new AutoRun().Execute( typeof( Program ).GetTypeInfo().Assembly, Console.Out, Console.In, args );
#endif
        }
    }
}
