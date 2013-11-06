// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Reflection;
using System.Xml.Linq;

namespace NodaTime.Tools.AnnotationDocumentor
{
    /// <summary>
    /// Base class for processors. This class is abstract despite all the methods being concrete; all
    /// methods have no-op implementations which can be selectively overridden by derived classes.
    /// </summary>
    internal abstract class ProcessorBase
    {
        internal virtual void ProcessParameter(XElement parent, ParameterInfo parameter, Attribute annotation) { }
        internal virtual void ProcessMethod(XElement element, MethodInfo method, Attribute annotation) { }
        internal virtual void ProcessProperty(XElement element, PropertyInfo property, Attribute annotation) { }
        internal virtual void ProcessType(XElement element, Type type, Attribute annotation) { }
    }
}
