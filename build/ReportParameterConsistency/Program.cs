// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Linq;
using System.Reflection;

namespace ReportParameterConsistency
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: ReportParameterConsistency <assembly>");
                return;
            }

            var asm = Assembly.LoadFrom(args[0]);
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

            /*
            var p2 = from type in asm.GetTypes()
                             where type.IsPublic
                             from parameter in (type.GetMethods(flags).SelectMany(m => m.GetParameters()))
                                .Concat(type.GetConstructors(flags).SelectMany(m => m.GetParameters()))
                     where parameter.Name == "result"
                     select type;
            Console.WriteLine(string.Join(",", p2));
            */

            // Note: could use indexers too...
            var parameters = from type in asm.GetTypes()
                             where type.IsPublic
                             from parameter in (type.GetMethods(flags).SelectMany(m => m.GetParameters()))
                                .Concat(type.GetConstructors(flags).SelectMany(m => m.GetParameters()))
                             where !parameter.ParameterType.ContainsGenericParameters
                             group parameter by parameter.ParameterType into g
                             orderby g.Key.FullName
                             select new { Type = g.Key.ToString(),
                                 Names = g.GroupBy(p => p.Name, (k, ps) => new { Name = k, Count = ps.Count() })
                                                   .OrderByDescending(x => x.Count) };

            foreach (var item in parameters)
            {
                Console.WriteLine(item.Type);
                foreach (var name in item.Names)
                {
                    Console.WriteLine($"  {name.Count,3}: {name.Name}");
                }
                Console.WriteLine();
            }
        }
    }
}
