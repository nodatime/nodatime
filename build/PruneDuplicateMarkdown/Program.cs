using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace PruneDuplicateMarkdown
{
    /// <summary>
    /// Removes duplicate files from a given directory hierarchy.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Arguments: <directory>");
                return;
            }
            var root = args[0];
            var allFiles = Directory.GetFiles(root, "*", SearchOption.AllDirectories).OrderBy(f => f).ToList();

            var filesToDelete = new List<string>();

            using (var hasher = SHA256.Create())
            {
                var groups = allFiles.Select(f => new { File = f, Hash = BitConverter.ToString(hasher.ComputeHash(File.ReadAllBytes(f))) })
                    .GroupBy(p => new { RelativeFile = Path.GetFileName(p.File), p.Hash }, p => p.File)
                    .Where(g => g.Count() > 1)
                    .ToList();

                foreach (var group in groups)
                {
                    // Skip the "earliest" file - fortunately versions always come before "developer" or "unstable".
                    filesToDelete.AddRange(group.Skip(1));
                }
            }

            if (filesToDelete.Count == 0)
            {
                Console.WriteLine("No duplicates found");
                return;
            }

            Console.WriteLine("Files to delete:");
            foreach (var file in filesToDelete)
            {
                Console.WriteLine(file.StartsWith(root) ? file.Substring(root.Length) : file);
            }
            Console.WriteLine();
            Console.Write("Delete files? ");
            string response = Console.ReadLine();
            if (response == "y")
            {
                filesToDelete.ForEach(f => File.Delete(f));
            }
        }
    }
}