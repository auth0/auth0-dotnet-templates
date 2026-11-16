// List the templates in this repo as JSON.
// Run (needs .NET 10 SDK): dotnet run --file .agents/skills/add-template-dotnet-version/scripts/list-templates.cs -- [--repo-root PATH]
#:property PublishAot=false
using System.Text.Json;
using System.Text.RegularExpressions;

const string Usage = """
    Usage: dotnet run --file list-templates.cs -- [--repo-root PATH]

    Prints a JSON array with one object per folder in templates/:
      name               folder name, e.g. Auth0.Mvc
      shortName          dotnet new short name ("" when the folder has no .template.config)
      versions           versionN folders found directly under the template
      hasTemplateConfig  true when .template.config/template.json exists
      projects           number of registration folders (one per project that gets registered)

    Options:
      --repo-root PATH   repository root (default: nearest parent folder containing Auth0Templates.csproj)
      -h, --help         show this help
    """;

// `dotnet run` may start in the script's folder, so look upwards for the repo root.
static string FindRoot()
{
    for (var dir = new DirectoryInfo(Directory.GetCurrentDirectory()); dir != null; dir = dir.Parent)
        if (File.Exists(Path.Combine(dir.FullName, "Auth0Templates.csproj"))) return dir.FullName;
    return ".";
}

var root = FindRoot();
for (var i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "-h" or "--help":
            Console.WriteLine(Usage);
            return 0;
        case "--repo-root" when i + 1 < args.Length:
            root = args[++i];
            break;
        default:
            Console.Error.WriteLine($"Unknown argument: {args[i]}\n{Usage}");
            return 2;
    }
}

var templates = Path.Combine(root, "templates");
if (!Directory.Exists(templates))
{
    Console.Error.WriteLine($"Error: '{templates}' not found. Run from the repo root or pass --repo-root.");
    return 1;
}

static bool IsBuildOutput(string path) =>
    path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Any(p => p is "bin" or "obj");

var result = Directory.GetDirectories(templates)
    .OrderBy(d => d, StringComparer.Ordinal)
    .Select(dir =>
    {
        var cfg = Path.Combine(dir, ".template.config", "template.json");
        var hasConfig = File.Exists(cfg);
        var shortName = hasConfig
            ? Regex.Match(File.ReadAllText(cfg), "\"shortName\"\\s*:\\s*\"([^\"]*)\"").Groups[1].Value
            : "";
        var versions = Directory.GetDirectories(dir, "version*")
            .Select(Path.GetFileName)
            .OrderBy(v => v, StringComparer.Ordinal)
            .ToArray();
        var projects = Directory.GetDirectories(dir, "registration", SearchOption.AllDirectories)
            .Count(d => !IsBuildOutput(d));
        return new { name = Path.GetFileName(dir), shortName, versions, hasTemplateConfig = hasConfig, projects };
    });

Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
return 0;
