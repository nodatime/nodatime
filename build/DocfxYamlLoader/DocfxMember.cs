using System.Collections.Generic;
using System.Linq;

namespace DocfxYamlLoader
{
    public class DocfxMember
    {
        private static readonly Dictionary<string, string> OperatorNames = new Dictionary<string, string>
        {
            { "GreaterThan", ">" },
            { "LessThan", "<" },
            { "GreaterThanOrEqual", ">=" },
            { "LessThanOrEqual", "<=" },
            { "Inequality", "!=" },
            { "Equality", "==" },
            { "Addition", "+" },
            { "Subtraction", "-" },
            { "UnaryNegation", "-" },
            { "Multiply", "*" },
            { "Division", "/" },
            // TODO: Conversions, unary addition, true/false. Anything else?
        };

        public string YamlFile { get; set; }
        public string Uid { get; set; }
        public string Parent { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public DocfxMember ParentMember { get; set; }
        public TypeKind Type { get; set; }
        public List<DocfxAttribute> Attributes { get; set; }

        public bool Obsolete => Attributes?.Any(attr => attr.Type == "System.ObsoleteAttribute") ?? false;

        // TODO: This is far from elegant...
        public string DisplayName
        {
            get
            {
                if (!IsTypeMember)
                {
                    return FullName ?? Uid;
                }
                if (Type == TypeKind.Operator)
                {
                    var op = Name.Split('(').First();
                    var symbolicOp = OperatorNames[op];
                    return $"operator {symbolicOp}{Name.Substring(op.Length)}";
                }
                return Name;
            }
        }

        public bool IsTypeMember =>
            Type == TypeKind.Property ||
            Type == TypeKind.Constructor ||
            Type == TypeKind.Method ||
            Type == TypeKind.Operator ||
            Type == TypeKind.Field;

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

        public class DocfxAttribute
        {
            public string Type { get; set; }
            // Add extra properties if necessary...
        }
    }
}
