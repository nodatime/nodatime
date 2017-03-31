using System;
using System.Collections.Generic;
using System.Text;

namespace DocfxYamlLoader
{
    public class DocfxMember
    {
        public string Uid { get; set; }
        public string Parent { get; set; }
        public DocfxMember ParentMember { get; set; }
        public TypeKind Type { get; set; }

        public enum TypeKind
        {
            Class,
            Enum,
            Interface,
            Property,
            Constructor,
            Method,
            Operator,
            Delegate,
            Field,
            Namespace,
            Struct
        }
    }
}
