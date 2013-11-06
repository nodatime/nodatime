// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace NodaTime.Tools.AnnotationDocumentor
{
    /// <summary>
    /// Processor for NotNull annotations.
    /// </summary>
    internal sealed class NotNullProcessor : ProcessorBase
    {
        internal override void ProcessParameter(XElement parent, ParameterInfo parameter, Attribute annotation)
        {
            var existingElements = parent.Elements("exception")
                                         .Where(x => (string)x.Attribute("cref") == "T:System.ArgumentNullException")
                                         .ToList();
            var parameterNames = existingElements.Descendants("paramref")
                                                 .Select(x => (string) x.Attribute("name"))
                                                 .Concat(new[] { parameter.Name })
                                                 .Distinct()
                                                 .ToList();
            existingElements.Remove();
            // Just to avoid the repetition later...
            Func<string, XElement> exceptionElement = name => new XElement("paramref", new XAttribute("name", name));

            XElement newElement = new XElement("exception", new XAttribute("cref", "T:System.ArgumentNullException"));
            if (parameterNames.Count == 1)
            {
                newElement.Add(exceptionElement(parameterNames[0]), " is null.");
            }
            else
            {
                for (int i = 0; i < parameterNames.Count - 2; i++)
                {
                    newElement.Add(exceptionElement(parameterNames[i]), ", ");
                }
                newElement.Add(exceptionElement(parameterNames[parameterNames.Count - 2]), " or ",
                    exceptionElement(parameterNames.Last()),
                    " is null.");
            }
            parent.Add(newElement);
        }
    }
}
