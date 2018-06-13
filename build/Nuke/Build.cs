using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DocFx;
using Nuke.Common.Tools.DotCover;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Utilities.Collections;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers;
using static System.IO.File;
using static DocfxAnnotationGenerator.DocfxAnnotationGenerator;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Logger;
using static Nuke.Common.Tools.DocFx.DocFxTasks;
using static Nuke.Common.Tools.DotCover.DotCoverTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.Git.GitTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;
using static SnippetExtractor.SnippetExtractor;

class Build : NukeBuild
{
    // Console application entry point. Also defines the default target.
    public static int Main () => Execute<Build>(x => x.Compile);

    string HistoryDirectory => BuildRootDirectory / "history";
    AbsolutePath BuildRootDirectory => RootDirectory / "build";
    AbsolutePath TemporaryDocFxDirectory => BuildRootDirectory / "tmp" / "docfx";

    AbsolutePath ReleaseDirectory => BuildRootDirectory / "releasebuild";

    //Todo: NodaTime and NodaTime.Tests project props??
    //Todo: All Solution

    [Parameter("The version to release. It is expected that a git tag will already exist.")] readonly string ReleaseVersion;
    [Parameter("If --skip-api is set, it is assumed the API docs already exist.")] readonly bool SkipApi;
    [Parameter("It is expected that the directory already exists and is set up for git.")]readonly string WebDirectory;



    // Auto-injection fields:

    // [GitVersion] readonly GitVersion GitVersion;
    // Semantic versioning. Must have 'GitVersion.CommandLine' referenced.

    // [GitRepository] readonly GitRepository GitRepository;
    // Parses origin, branch name and head from git config.

    // [Parameter] readonly string MyGetApiKey;
    // Returns command-line arguments and environment variables.

    [Solution] readonly Solution Solution;

    Target Clean => _ => _
            .OnlyWhen(() => false) // Disabled for safety.
            .Executes(() =>
            {
                DeleteDirectories(GlobDirectories(SourceDirectory, "**/bin", "**/obj"));
                EnsureCleanDirectory(TemporaryDocFxDirectory);
                EnsureCleanDirectory(OutputDirectory);
                EnsureCleanDirectory(ArtifactsDirectory);
            });
    Target Restore => _ => _
            .DependsOn(Clean)
            .Executes(() =>
            {
                DotNetRestore(s => DefaultDotNetRestore);
            });
    Target Compile => _ => _
            .DependsOn(Restore)
            .Executes(() =>
            {
                DotNetBuild(s => DefaultDotNetBuild);
            });

    //Todo use Configuration instead and ensure that the value is set to release

    Target BuildRelease => _ => _
        .Requires(() => ReleaseVersion)
        .Executes(() =>
        {
            DeleteDirectory(ReleaseDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);

            Git($"clone https://github.com/nodatime/nodatime.git {ReleaseDirectory} -c core.autocrlf=input");
            Git($"checkout {ReleaseVersion}", ReleaseDirectory);

            var releaseSourceDirectory = ReleaseDirectory / "src";


            var buildSettings = new DotNetBuildSettings()
                .SetConfiguration("release")
                .SetProperty("SourceLinkCreate",true);

            var packSettings = new DotNetPackSettings()
                .SetConfiguration("release")
                .SetOutputDirectory(ArtifactsDirectory);

            var restoreFlag = string.Empty;
                

            var suffixSplit = ReleaseVersion.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            var suffix = suffixSplit.Length > 1 ? suffixSplit[1] : null;
            if (suffix != null)
            {
                buildSettings = buildSettings.SetVersionSuffix(suffix);
                packSettings = packSettings.SetVersionSuffix(suffix);
                restoreFlag = $"-p:VersionSuffix={suffix}";
            }

            var buildProjects = new[] {"NodaTime", "NodaTime.Testing", "NodaTime.Test"};
            buildProjects.ForEach(x => DotNet($"restore {restoreFlag} {releaseSourceDirectory / x}"));
            buildProjects.ForEach(x => DotNetBuild(y => buildSettings.SetProjectFile(releaseSourceDirectory / x)));


            var runSettings = new DotNetRunSettings()
                .SetConfiguration("release")
                .SetProjectFile(releaseSourceDirectory / "NodaTime.Test" / "NodaTime.Test.csproj");

            //Todo Run nunit directly?
            //Todo failing on system with en-us culture but modified datetime format.
            DotNetRun(x => runSettings.SetFramework("netcoreapp1.0"));
            DotNetRun(x => runSettings.SetFramework("net451"));

            DotNetPack(x => packSettings.SetProject(releaseSourceDirectory / "NodaTime"));
            DotNetPack(x => packSettings.SetProject(releaseSourceDirectory / "NodaTime.Testing"));

            var releaseName = $"NodaTime-{ReleaseVersion}";
            var releaseZip = $"{releaseName}-src.zip";
            var archivePrefix = $"{releaseName}-src/";
            Git( $"archive {ReleaseVersion} -o {ArtifactsDirectory / releaseZip} --prefix={archivePrefix}", ReleaseDirectory);

            var versionDirectory = ReleaseDirectory / releaseName;
            var portableVersionDirectory = versionDirectory / "Portable";
            Directory.CreateDirectory(versionDirectory);

            //Copy LICENSE.txt, AUTHORS.txt ...
            Directory.GetFiles(RootDirectory, "*.txt", SearchOption.TopDirectoryOnly)
               .ForEach(x => Copy(x,versionDirectory / Path.GetFileName(x)));
            Copy(RootDirectory / "NodaTime Release Public Key.snk", versionDirectory / "NodaTime Release Public Key.snk");
            Copy(BuildRootDirectory / "zip-readme.txt", versionDirectory / "readme.txt");

            
            Directory.CreateDirectory(portableVersionDirectory);


            //Todo naming
            var releaseProjects = new[] { "NodaTime", "NodaTime.Testing" };
            //Doc files
            void CopyReleaseFiles(string project, DirectoryInfo franeworkDirectory)
            {
                
                var targetDir = franeworkDirectory.Name.StartsWith("netstandard") ? portableVersionDirectory : versionDirectory;
;                GlobFiles( franeworkDirectory.FullName, $"{project}.dll", $"{project}.xml", $"{project}.pdb")
                    .ForEach(x => Copy(x, targetDir / Path.GetFileName(x)));

            }

            foreach (var releaseProject in releaseProjects)
            {
                new DirectoryInfo(releaseSourceDirectory / releaseProject / "bin" / "Release")
                    .GetDirectories("net*")
                    .ForEach(x => CopyReleaseFiles(releaseProject, x));
            }

            var gitResult = Git("show -s --format=%cI", ReleaseDirectory, redirectOutput: true);
            var buildDate = DateTimeOffset.Parse(gitResult.Single());
            Info("Date: " + buildDate);

            //# see https://wiki.debian.org/ReproducibleBuilds/TimestampsInZip.
            var releaseFiles = GlobFiles(versionDirectory, "*").ToArray();

            releaseFiles.ForEach(x => SetLastWriteTimeUtc(x, buildDate.UtcDateTime));

            using (Stream stream = OpenWrite(ArtifactsDirectory / $"{releaseName}.zip"))
            using (var writer = WriterFactory.Open(stream, ArchiveType.Tar, new WriterOptions(CompressionType.GZip)
                                                                            {
                                                                                LeaveStreamOpen = true
                                                                            }))
            {
                foreach (var x in releaseFiles) writer.Write(x.Replace(versionDirectory, string.Empty).Trim(new[] { '/', '\\' }), new FileInfo(x));
            }

        });

    Target TestSnippets => _ => _
        .Executes(() =>
        {
            var testDirectory = TemporaryDirectory / "snippet_test";
            EnsureCleanDirectory(testDirectory);

            DotNetPublish(x => x.SetProject(SourceDirectory / "NodaTime.Demo"));
            ExtractSnippets(SourceDirectory / "NodaTime-All.sln","NodaTime.Demo", testDirectory);
        });

    [Parameter] readonly bool Report;

    Target Coverage => _ => _
        .Executes(() =>
        {
            var coverageDirectory = RootDirectory / "coverage";
            
            EnsureCleanDirectory(coverageDirectory);

            var testProjectDirectory = Solution.GetProject("NodaTime.Test").NotNull().Directory;
            DotCoverCover(s => s
                .SetConfiguration(testProjectDirectory / "coverageparams.xml")
                .EnableReturnTargetExitCode()
                .SetWorkingDirectory(testProjectDirectory));

            var coverageFile = coverageDirectory / "coverage.xml";
            DotCoverReport(s => s
                .AddSource(coverageDirectory / "NodaTime.dvcr")
                .SetOutputFile(coverageFile)
                .SetReportType(DotCoverReportType.DetailedXml));

            if (Report)
            {
                ReportGenerator(s => s
                    .SetReports(coverageFile)
                    .SetTargetDirectory(coverageDirectory / "report")
                    .SetVerbosity(ReportGeneratorVerbosity.Error));
            }
        });

    Target BuildApiDocs => _ => _
        //.DependsOn()
        .OnlyWhen(() => !SkipApi)
        .Executes(() =>
        {
            if (!Directory.Exists(HistoryDirectory))
            {
                Info("Cloning history branch");
                Git($"clone https://github.com/nodatime/nodatime.git -q --depth 1 -b history {HistoryDirectory}");
            }
            
            DeleteDirectory(TemporaryDocFxDirectory);
            
            Info("Copying metadata for previous versions");
            var versionsDirectories = new DirectoryInfo(HistoryDirectory)
                .GetDirectories()
                .Where(x => Regex.IsMatch(x.Name, @"^(\d\.){2}x$"))
                .ToList();
            var versions = versionsDirectories.Select(x => x.Name).ToList();

            var objDirectory = TemporaryDocFxDirectory / "obj";
            foreach (var versionDirectory in versionsDirectories)
            {
                // TODO: use AbsolutePath ?
                var version = versionDirectory.Name;
                var versionTempDirectory = objDirectory / version;
                CopyRecursively(Combine(versionDirectory.FullName, "api"), versionTempDirectory / "api");

                var overwritePath = Combine(version, "overwrite");
                if (Directory.Exists(overwritePath))
                    CopyRecursively(overwritePath, versionTempDirectory);

                Copy(
                    sourceFileName: BuildRootDirectory / "docfx" / "toc.yml",
                    destFileName: versionTempDirectory / "toc.yml",
                    overwrite: true);
            }

            Info("Building metadata for current branch");

            var serializationDirectory = TemporaryDocFxDirectory / "serialization";
            Git($"clone https://github.com/nodatime/nodatime.serialization.git -q --depth 1 {serializationDirectory}");
            var unstableDirectory = TemporaryDocFxDirectory / "unstable";
            var unstableSourceDirectory = unstableDirectory / "src";
            
            versions.Add("unstable");

            // TODO: foreach
            Directory.GetDirectories(SourceDirectory)
                .Select(Path.GetFileName)
                .Where(x => x == "NodaTime" || x == "NodaTime.Testing")
                .ForEach(x => CopyRecursively(SourceDirectory / x, unstableSourceDirectory / x));

            Directory.CreateDirectory(unstableDirectory);
            //Todo move
            Directory.GetFiles(RootDirectory, "*.snk", SearchOption.TopDirectoryOnly)
                .ForEach(x => Copy(x, TemporaryDocFxDirectory / "unstable" / Path.GetFileName(x), overwrite: true));
            CopyRecursively(
                source: serializationDirectory / "src" / "NodaTime.Serialization.JsonNet",
                target: unstableSourceDirectory / "NodaTime.Serialization.JsonNet");

            Info("Do the build for unstable so we can get annotations");
            Directory.GetDirectories(unstableSourceDirectory).ForEach(x => DotNet($"build {x}"));

            var docFxSourceDir = BuildRootDirectory / "docfx";
            var docFxPath = TemporaryDocFxDirectory / "docfx.json";
            var unstableObjDirectory = objDirectory / "unstable";
            CopyRecursively(docFxSourceDir / "template", TemporaryDocFxDirectory / "template");
            Copy(docFxSourceDir / "docfx-unstable.json",docFxPath);

            var visualStudioDirectory =
                new DirectoryInfo(MSBuildToolPathResolver.Resolve(MSBuildVersion.VS2017).NotNull())
                    .Descendants(x => x.Parent)
                    .FirstOrDefault(x => x.Parent?.Name == "2017");
            SetVariable("VSINSTALLDIR", visualStudioDirectory.NotNull("visualStudioDirectory != null").FullName);

            DocFxMetadata(s => s
                .EnableForce()
                .SetConfigPath(docFxPath)
                .SetWorkingDirectory(TemporaryDocFxDirectory));

            Copy(sourceFileName: docFxSourceDir / "toc.yml", destFileName: unstableObjDirectory / "toc.yml");

            Info("Create diffs between versions and other annotations");
            for (var i = 0; i < versions.Count - 1; i++)
                ReleaseDiffGenerator.GenerateReleaseDiff(objDirectory / versions[i], objDirectory / versions[i + 1]);

            Info("Extract annotations");
            GenerateDocfxAnnotations(
                docfxRoot: TemporaryDocFxDirectory,
                packagesDirectory: Combine(HistoryDirectory, "packages"),
                sourceRoot: unstableSourceDirectory,
                versions: versions.ToArray());

            ExtractSnippets(
                solutionFile: SourceDirectory / "NodaTime-All.sln",
                projectName: "NodaTime.Demo",
                outputDirectory: unstableObjDirectory / "overwrite");
            
            Info("Running main docfx build");
            DocFxBuild(x => x
                .SetConfigPath(TemporaryDocFxDirectory / "docfx.json")
                .SetWorkingDirectory(BuildRootDirectory));

            Copy(
                sourceFileName: docFxSourceDir / "logo.svg",
                destFileName: TemporaryDocFxDirectory / "_site" / "logo.svg",
                overwrite: true);
        });


    Target BuildWeb => _ => _
        .DependsOn(BuildApiDocs)
        .Requires(() => WebDirectory)
        .Executes(() =>
        {
            var webProjectDirectory = SourceDirectory / "NodaTime.Web";

            if (!SkipApi)
            {
                var webDocFxDirectory = webProjectDirectory / "docfx";
                DeleteDirectory(webDocFxDirectory);
                CopyRecursively(TemporaryDocFxDirectory / "_site", webDocFxDirectory);
            }

            //# Build the web site ASP.NET Core
                //Todo move?
                DeleteDirectory(webProjectDirectory / "bin" / "Release");
                DotNetBuild(x => x.SetConfiguration("release").SetProjectFile(webProjectDirectory));
                DotNetPublish(x => x.SetConfiguration("release").SetProject(webProjectDirectory));

                // Retain just the .git directory, but nuke the rest from orbit.
                var oldDirectory = TemporaryDirectory / "old_nodatetime.org";
                DeleteDirectory(oldDirectory);
                Directory.Move(WebDirectory,oldDirectory);

                //Copy the new site into place
                CopyRecursively(webProjectDirectory / "bin" / "Release" / "netcoreapp2.1" / "publish",WebDirectory);

                // Fix up blazor.config to work in Unix
                // (Blazor is currently disabled.)
                //var blazorPath = Combine(WebDirectory , "NodaTime.Web.Blazor.blazor.config");
                //TextTasks.WriteAllText(blazorPath, TextTasks.ReadAllText(blazorPath).Replace(oldChar: '\\',newChar: ','));
                           


                //Run smoke test
                DotNet($"{Combine(WebDirectory, "NodaTime.Web.dll")} --smoke-test", WebDirectory);
        });

    /*
    static void PruneDuplicateMarkdown(string root)
    {
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

    /*
    
    private const string Bucket = "nodatime";
    private const string Prefix = "releases/";
    private const string Sha256Key = "SHA-256";

    public static void HashStorageFiles(string[] args)
    {

        bool validate = args.Length == 1 && args[0] == "--validate";
        var client = StorageClient.Create();
        var files = client.ListObjects(Bucket, Prefix).Where(x => !x.Name.EndsWith("/")).ToList();

        using (SHA256 sha = SHA256.Create())
        {
            foreach (var file in files)
            {
                if (file.Metadata == null)
                {
                    file.Metadata = new Dictionary<string, string>();
                }
                bool hashExists = file.Metadata.TryGetValue(Sha256Key, out string existingHash);
                if (hashExists && !validate)
                {
                    Console.WriteLine($"Skipping {file.Name}");
                    continue;
                }
                Console.WriteLine($"{(hashExists ? "Validating" : "Hashing")} {file.Name}");
                var stream = new MemoryStream();
                client.DownloadObject(file, stream, new DownloadObjectOptions { IfGenerationMatch = file.Generation });
                stream.Position = 0;
                byte[] hash = sha.ComputeHash(stream);
                string hex = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                if (hashExists)
                {
                    if (hex != existingHash)
                    {
                        Console.WriteLine($"ERROR: Existing hash: {existingHash}; computed hash: {hex}");
                    }
                }
                else
                {
                    file.Metadata[Sha256Key] = hex;
                    client.UpdateObject(file);
                }
            }
        }
    }
     */
}

