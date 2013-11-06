// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace NodaTime.Tools.AnnotationDocumentor
{
    /// <summary>
    /// Processor which simply delegates to other processors based on the annotation name.
    /// </summary>
    internal sealed class DelegatingProcessor : ProcessorBase, IEnumerable
    {
        private readonly Dictionary<string, ProcessorBase> processors = new Dictionary<string, ProcessorBase>();

        public void Add(string name, ProcessorBase processor)
        {
            processors.Add(name, processor);
        }

        internal override void ProcessMethod(XElement element, MethodInfo method, Attribute annotation)
        {
            ProcessorBase processor;
            if (processors.TryGetValue(annotation.GetType().Name, out processor))
            {
                processor.ProcessMethod(element, method, annotation);
            }
        }

        internal override void ProcessParameter(XElement parent, ParameterInfo parameter, Attribute annotation)
        {
            ProcessorBase processor;
            if (processors.TryGetValue(annotation.GetType().Name, out processor))
            {
                processor.ProcessParameter(parent, parameter, annotation);
            }
        }

        internal override void ProcessProperty(XElement element, PropertyInfo property, Attribute annotation)
        {
            ProcessorBase processor;
            if (processors.TryGetValue(annotation.GetType().Name, out processor))
            {
                processor.ProcessProperty(element, property, annotation);
            }
        }

        internal override void ProcessType(XElement element, Type type, Attribute annotation)
        {
            ProcessorBase processor;
            if (processors.TryGetValue(annotation.GetType().Name, out processor))
            {
                processor.ProcessType(element, type, annotation);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return processors.Values.GetEnumerator();
        }
    }
}
