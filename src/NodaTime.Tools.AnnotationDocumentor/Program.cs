// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace NodaTime.Tools.AnnotationDocumentor
{
    /// <summary>
    /// Tool to annotate XML documentation based on the attributes discovered in the corresponding assembly.
    /// This is run as a post-build step before running Sandcastle.
    /// </summary>
    internal sealed class Program
    {
        private readonly ProcessorBase rootProcessor;

        private Program()
        {
            rootProcessor = new DelegatingProcessor
            {
                { "NotNullAttribute", new NotNullProcessor() }
            };
        }

        private static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: NodaTime.Tools.AnnotationDocumentor <assembly> <XML input> <XML output>");
                return 1;
            }
            Assembly assembly = Assembly.LoadFrom(args[0]);
            
            XDocument doc = XDocument.Load(args[1]);
            new Program().Process(assembly, doc);
            doc.Save(args[2]);
            return 0;
        }

        private void Process(Assembly assembly, XDocument doc)
        {
            var publicTypes = assembly.GetTypes().Where(t => t.IsPublic).ToList();
            ProcessAll(publicTypes, ProcessType, doc);
            ProcessAll(publicTypes.SelectMany(t => t.GetProperties()), ProcessProperty, doc);
            ProcessAll(publicTypes.SelectMany(t => t.GetMethods()), ProcessMethod, doc);
        }

        private void ProcessAll<T>(IEnumerable<T> members, Action<T, XDocument> processor, XDocument doc)
        {
            foreach (var member in members)
            {
                processor(member, doc);
            }
        }

        private void ProcessType(Type type, XDocument doc)
        {
            var element = FindMember(doc, "T:" + type.FullName);
            if (element == null)
            {
                return;
            }
            foreach (var annotation in GetAnnotations(type))
            {
                rootProcessor.ProcessType(element, type, annotation);
            }
        }

        private void ProcessProperty(PropertyInfo property, XDocument doc)
        {
            var parameters = property.GetIndexParameters();
            string name = "P:" + property.ReflectedType.FullName + "." + property.Name
                + (parameters.Length == 0 ? "" : GetParametersSignature(parameters));
            var element = FindMember(doc, name);
            if (element == null)
            {
                return;
            }
            foreach (var parameter in parameters)
            {
                foreach (var annotation in GetAnnotations(parameter))
                {
                    rootProcessor.ProcessParameter(element, parameter, annotation);
                }
            }

            foreach (var annotation in GetAnnotations(property))
            {
                rootProcessor.ProcessProperty(element, property, annotation);
            }
        }

        private void ProcessMethod(MethodInfo method, XDocument doc)
        {
            string name = "M:" + method.ReflectedType.FullName + "." + method.Name + GetParametersSignature(method.GetParameters());
            var element =  FindMember(doc, name);
            if (element == null)
            {
                return;
            }
            foreach (var parameter in method.GetParameters())
            {
                foreach (var annotation in GetAnnotations(parameter))
                {
                    rootProcessor.ProcessParameter(element, parameter, annotation);
                }
            }
            foreach (var annotation in GetAnnotations(method))
            {
                rootProcessor.ProcessMethod(element, method, annotation);
            }
        }

        private string GetParametersSignature(ParameterInfo[] parameters)
        {
            return "(" + string.Join(",", parameters.Select(p => p.ParameterType.FullName).ToArray()) + ")";
        }

        private XElement FindMember(XDocument doc, string name)
        {
            return doc.Descendants("member").SingleOrDefault(element => (string) element.Attribute("name") == name);
        }

        private IEnumerable<Attribute> GetAnnotations(ICustomAttributeProvider member)
        {
            return member.GetCustomAttributes(false)
                         .Cast<Attribute>()
                         .Where(attr => attr.GetType().Namespace == "JetBrains.Annotations");
        }
    }
}
