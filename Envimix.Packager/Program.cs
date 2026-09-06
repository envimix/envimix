using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

const string sourceText = "file://Media/Images";
const string replacementText = "https://envimix.gbx.tools/img";
const string turboImageReplacementText = "https://envimix.gbx.tools/img/EnvimixTurbo.jpg";

var repositoryDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
var packagerDirectory = Path.Combine(repositoryDirectory, "Envimix.Packager");
var envimixProjectDirectory = Path.Combine(repositoryDirectory, "Envimix");
if (args.Length > 0)
{
    envimixProjectDirectory = Path.GetFullPath(args[0]);
}

var buildLabel = DateTime.Now.ToString("yyyy-MM-dd-HH_mm");
if (args.Length > 1)
{
    buildLabel = args[1];
}

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
var settingDefaults = new Dictionary<string, string>
{
    ["S_EnableTM2Cars"] = "True",
    ["S_EnableTrafficCar"] = "False",
    ["S_EnableUnitedCars"] = "False",
    ["S_EnableCustomCars"] = "False",
    ["S_EnableDefaultCar"] = "False",
    ["S_EnableStadiumEnvimix"] = "False",
    ["S_EnableTrafficCarInStadium"] = "False",
    ["S_UseUnitedModels"] = "False",
    ["S_AlwaysUseVehicleItems"] = "False",
    ["S_VehicleFolder"] = "\"Vehicles/\"",
    ["S_VehicleFileFormat"] = "\"%1.Item.Gbx\"",
    ["S_CarsFile"] = "\"\"",
    ["S_SkinsFile"] = "\"\"",
    ["S_EnvimixWebAPI"] = "\"https://api.envimix.gbx.tools\"",
    ["S_EnvimixXmlRpc"] = "False",
    ["S_EnableEnvimaniaSessions"] = "False"
};
var titlePacks = new[]
{
    new TitlePack("TM2U_Island@adamkooo", "file://Media/Images/Graphics/LoadscreenCurrent.png"),
    new TitlePack("TMOneAlpine@unbitn", ""),
    new TitlePack("TMOneSpeed@unbitn", ""),
    //new TitlePack("TMOneBay@unbitn", ""),
    new TitlePack("TMAll@domino54", ""),
    new TitlePack("Nadeo_Envimix@bigbang1112", ""),
    new TitlePack("TMCanyon@nadeo", ""),
    new TitlePack("TMValley@nadeo", ""),
    new TitlePack("TMLagoon@nadeo", "")
};

foreach (var titlePack in titlePacks)
{
    var titlePackDirectory = Path.Combine(packagerDirectory, titlePack.Id);
    if (!Directory.Exists(titlePackDirectory))
    {
        Console.Error.WriteLine($"Title pack directory does not exist: {titlePackDirectory}");
        return 1;
    }
}

File.Delete("Envimix.zip");

var stagingDirectory = Path.Combine(Path.GetTempPath(), $"Envimix.Packager-{Guid.NewGuid():N}");

try
{
    Directory.CreateDirectory(stagingDirectory);

    foreach (var titlePack in titlePacks)
    {
        var titlePackDirectory = Path.Combine(packagerDirectory, titlePack.Id);
        var titleStagingDirectory = Path.Combine(stagingDirectory, titlePack.Id);

        foreach (var root in roots)
        {
            var sourceDirectory = Path.Combine(outputDirectory, root);
            if (!Directory.Exists(sourceDirectory))
            {
                Console.Error.WriteLine($"Required package directory does not exist: {sourceDirectory}");
                return 1;
            }

            CopyDirectory(sourceDirectory, Path.Combine(titleStagingDirectory, root), root, ignoredFilePaths);
        }

        CopyDirectory(titlePackDirectory, titleStagingDirectory, string.Empty, new HashSet<string>(), includeAllFiles: true);
        ApplySettingDefaults(
            Path.Combine(titleStagingDirectory, "Scripts", "Modes", "TrackMania", "Envimix.Script.txt"),
            settingDefaults);
            
        ReplaceInFiles(titleStagingDirectory, sourceText, replacementText);

        if (!string.IsNullOrEmpty(titlePack.LoadingImageUrl))
        {
            ReplaceInFiles(titleStagingDirectory, turboImageReplacementText, titlePack.LoadingImageUrl);
        }

        var archivePath = $"Envimix.{titlePack.Id}.{buildLabel}.zip";
        File.Delete(archivePath);
        ZipFile.CreateFromDirectory(titleStagingDirectory, archivePath, CompressionLevel.Fastest, includeBaseDirectory: false);

        Console.WriteLine($"Created {archivePath}");
    }

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

static void ApplySettingDefaults(string scriptPath, IReadOnlyDictionary<string, string> settingDefaults)
{
    var contents = File.ReadAllText(scriptPath);

    foreach (var (settingName, defaultValue) in settingDefaults)
    {
        var pattern = $"^(#Setting\\s+{Regex.Escape(settingName)}\\s+)(?:\"(?:\\\\.|[^\"])*\"|\\S+)";
        var matches = Regex.Matches(contents, pattern, RegexOptions.Multiline);
        if (matches.Count != 1)
        {
            throw new InvalidDataException(
                $"Expected exactly one declaration for {settingName} in {scriptPath}, found {matches.Count}.");
        }

        contents = Regex.Replace(
            contents,
            pattern,
            match => match.Groups[1].Value + defaultValue,
            RegexOptions.Multiline);
    }

    File.WriteAllText(scriptPath, contents);
}

static void CopyDirectory(
    string sourceDirectory,
    string destinationDirectory,
    string relativeDirectory,
    IReadOnlySet<string> ignoredFilePaths,
    bool includeAllFiles = false)
{
    Directory.CreateDirectory(destinationDirectory);

    foreach (var filePath in Directory.EnumerateFiles(sourceDirectory).Where(path => includeAllFiles || IsTextFile(path)))
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
            ignoredFilePaths,
            includeAllFiles);
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

internal sealed record TitlePack(string Id, string LoadingImageUrl);
