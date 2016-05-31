using NUnitLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using NUnit.Common;

namespace Orient.Nunit.Test
{
    public class Program
    {
        public static int Main(string[] args)
        {
#if DNX451
            return new AutoRun().Execute(args);
#else
            return new AutoRun(typeof(Program).GetTypeInfo().Assembly).Execute(args, new ExtendedTextWrapper(Console.Out), Console.In);
#endif
        }
    }
}
