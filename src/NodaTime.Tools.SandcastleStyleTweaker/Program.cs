// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace NodaTime.Tools.SandcastleStyleTweaker
{
    class Program
    {
        static int Main(string[] args)
        {            
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: NodaTime.Tools.SandcastleStyleTweaker <style directory>");
                return 1;
            }
            XDocument snippets;
            using (var stream = typeof(Program).Assembly.GetManifestResourceStream(typeof(Program).Namespace + ".snippets.xml"))
            {
                snippets = XDocument.Load(stream);
            }

            string styleDirectory = args[0];

            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform");

            foreach (var element in snippets.Root.Elements())
            {
                string id = element.Attribute("id").Value;
                string relativeFile = element.Attribute("file").Value;
                string absoluteFile = Path.Combine(styleDirectory, relativeFile);
                string xpath = element.Attribute("xpath").Value;
                XDocument document = XDocument.Load(absoluteFile);
                XElement elementBefore = document.XPathSelectElement(xpath, namespaceManager);
                if (elementBefore == null)
                {
                    // No need for a decent exception type here...
                    throw new Exception(string.Format("xpath failed for snippet {0}", id));
                }
                elementBefore.AddAfterSelf(element.Nodes());
                Console.WriteLine("Applied snippet {0}", id);
                document.Save(absoluteFile);
            }
            return 0;
        }
    }
}
