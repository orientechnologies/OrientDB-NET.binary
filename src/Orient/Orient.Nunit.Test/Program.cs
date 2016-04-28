﻿using System;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using NUnitLite;

namespace Orient.Nunit.Test
{
    public class Program
    {
        public static int Main(string[] args)
        {
#if DNX451
            return new AutoRun().Execute(args);
#else
            return new AutoRun().Execute(typeof(Program).GetTypeInfo().Assembly, Console.Out, Console.In, args);
#endif
        }
    }
    }
