using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

using BenchmarkTypeResultProto = NodaTime.Benchmarks.Storage.Proto.BenchmarkTypeResult;

namespace NodaTime.Benchmarks.Storage
{
    public sealed class BenchmarkTypeResult
    {
        private readonly string type;
        private readonly string clrNamespace;
        private readonly ImmutableList<BenchmarkResult> results;

        public string Type { get { return type; } }
        public string Namespace { get { return clrNamespace; } }
        public ImmutableList<BenchmarkResult> Results { get { return results; } }

        public BenchmarkTypeResult(string clrNamespace, string type, ImmutableList<BenchmarkResult> results)
        {
            this.type = type;
            this.clrNamespace = clrNamespace;
            this.results = results;
        }

        public static BenchmarkTypeResult FromXElement(XElement element)
        {
            string clrNamespace = element.Attribute("namespace").Value;
            string type = element.Attribute("name").Value;
            string qualifiedType = string.Format("{0}.{1}", clrNamespace, type);
            return new BenchmarkTypeResult(
                clrNamespace: clrNamespace,
                type: type,
                results: element.Elements("test")
                                .Select(x => BenchmarkResult.FromXElement(qualifiedType, x))
                                .OrderBy(r => r.Method)
                                .ToImmutableList()
            );
        }

        public static BenchmarkTypeResult FromProto(BenchmarkTypeResultProto proto)
        {
            string qualifiedType = string.Format("{0}.{1}", proto.ClrNamespace, proto.Type);
            return new BenchmarkTypeResult(proto.ClrNamespace, proto.Type,
                proto.ResultsList.Select(r => BenchmarkResult.FromProto(qualifiedType, r)).ToImmutableList());
        }

        public BenchmarkTypeResultProto ToProto()
        {
            return new BenchmarkTypeResultProto.Builder
            {
                ClrNamespace = clrNamespace,
                Type = type,
                ResultsList = { results.Select(x => x.ToProto()) }
            }.Build();
        }
    }
}
