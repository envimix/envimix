using System.IO.Compression;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

const string sourceText = "file://Media/Images";
const string replacementText = "https://envimix.gbx.tools/img";

var repositoryDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
var envimixProjectDirectory = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.Combine(repositoryDirectory, "Envimix");
const string archivePath = "Envimix.zip";

if (!Directory.Exists(envimixProjectDirectory))
{
    Console.Error.WriteLine($"Envimix project directory does not exist: {envimixProjectDirectory}");
    return 1;
}

var outputDirectory = ResolveOutputDirectory(envimixProjectDirectory);
if (!Directory.Exists(outputDirectory))
{
    Console.Error.WriteLine($"Script build output does not exist: {outputDirectory}");
    Console.Error.WriteLine("Build the Envimix project before running the packager.");
    return 1;
}

var roots = new[]
{
    "Scripts/Libs",
    "Scripts/Modes",
    "Media/Manialinks/Universe2"
};
var ignoredFilePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
{
    "Scripts/Modes/TrackMania/EnvimixSolo.Script.txt",
    "Scripts/Modes/TrackMania/ViewGhost.Script.txt"
};

var stagingDirectory = Path.Combine(Path.GetTempPath(), $"Envimix.Packager-{Guid.NewGuid():N}");

try
{
    Directory.CreateDirectory(stagingDirectory);

    foreach (var root in roots)
    {
        var sourceDirectory = Path.Combine(outputDirectory, root);
        if (!Directory.Exists(sourceDirectory))
        {
            Console.Error.WriteLine($"Required package directory does not exist: {sourceDirectory}");
            return 1;
        }

        CopyDirectory(sourceDirectory, Path.Combine(stagingDirectory, root), root, ignoredFilePaths);
    }

    ReplaceInFiles(stagingDirectory, sourceText, replacementText);

    File.Delete(archivePath);
    ZipFile.CreateFromDirectory(stagingDirectory, archivePath, CompressionLevel.Fastest, includeBaseDirectory: false);

    return 0;
}
finally
{
    if (Directory.Exists(stagingDirectory))
    {
        Directory.Delete(stagingDirectory, recursive: true);
    }
}

static string ResolveOutputDirectory(string projectDirectory)
{
    var settingsPath = new[] { "buildsettings.yml", "buildsettings.yaml" }
        .Select(fileName => Path.Combine(projectDirectory, fileName))
        .FirstOrDefault(File.Exists);

    if (settingsPath is null)
    {
        return Path.Combine(projectDirectory, "out");
    }

    var deserializer = new DeserializerBuilder()
        .WithNamingConvention(PascalCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();
    var settings = deserializer.Deserialize<BuildSettings>(File.ReadAllText(settingsPath));

    if (string.IsNullOrWhiteSpace(settings.OutputDir))
    {
        return Path.Combine(projectDirectory, "out");
    }

    return Path.GetFullPath(settings.OutputDir, projectDirectory);
}

static bool IsTextFile(string path)
{
    return Path.GetExtension(path).ToLowerInvariant() is ".txt" or ".xml" or ".json" or ".yml" or ".yaml";
}

static void CopyDirectory(
    string sourceDirectory,
    string destinationDirectory,
    string relativeDirectory,
    IReadOnlySet<string> ignoredFilePaths)
{
    Directory.CreateDirectory(destinationDirectory);

    foreach (var filePath in Directory.EnumerateFiles(sourceDirectory).Where(IsTextFile))
    {
        var relativeFilePath = $"{relativeDirectory}/{Path.GetFileName(filePath)}";
        if (ignoredFilePaths.Contains(relativeFilePath))
        {
            continue;
        }

        File.Copy(filePath, Path.Combine(destinationDirectory, Path.GetFileName(filePath)), overwrite: true);
    }

    foreach (var childDirectory in Directory.EnumerateDirectories(sourceDirectory))
    {
        var childDirectoryName = Path.GetFileName(childDirectory);
        CopyDirectory(
            childDirectory,
            Path.Combine(destinationDirectory, childDirectoryName),
            $"{relativeDirectory}/{childDirectoryName}",
            ignoredFilePaths);
    }
}

static void ReplaceInFiles(string directory, string source, string replacement)
{
    var sourceBytes = Encoding.ASCII.GetBytes(source);
    var replacementBytes = Encoding.ASCII.GetBytes(replacement);

    foreach (var filePath in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
    {
        var contents = File.ReadAllBytes(filePath);
        var firstMatch = contents.AsSpan().IndexOf(sourceBytes);
        if (firstMatch < 0)
        {
            continue;
        }

        using var output = new MemoryStream(contents.Length + replacementBytes.Length - sourceBytes.Length);
        var offset = 0;
        while (firstMatch >= 0)
        {
            output.Write(contents, offset, firstMatch);
            output.Write(replacementBytes);
            offset += firstMatch + sourceBytes.Length;
            firstMatch = contents.AsSpan(offset).IndexOf(sourceBytes);
        }

        output.Write(contents, offset, contents.Length - offset);
        File.WriteAllBytes(filePath, output.ToArray());
    }
}

internal sealed class BuildSettings
{
    public string? OutputDir { get; init; }
}
