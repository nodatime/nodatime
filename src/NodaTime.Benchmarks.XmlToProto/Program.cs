using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Google.ProtocolBuffers;
using NodaTime.Benchmarks.Storage;
using BenchmarkRunProto = NodaTime.Benchmarks.Storage.Proto.BenchmarkRun;

namespace NodaTime.Benchmarks.XmlToProto
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: NodaTime.Benchmarks.XmlToProto <directory>");
                return 1;
            }
            string directory = args[0];
            string index = Path.Combine(directory, "index.txt");
            var runs = File.ReadLines(index)
                .Select(file => XDocument.Load(Path.Combine(directory, file)))
                .Select(BenchmarkRun.FromXDocument)
                .OrderByDescending(b => b.StartTime)
                .Select(b => b.ToProto())
                .ToList();

            using (var output = File.Create(Path.Combine(directory, "benchmarks.pb")))
            {
                var writer = new MessageStreamWriter<BenchmarkRunProto>(output);
                runs.ForEach(writer.Write);
                writer.Flush();
            }

            Console.WriteLine("Converted {0} runs from XML to proto", runs.Count);

            return 0;
        }
    }
}
