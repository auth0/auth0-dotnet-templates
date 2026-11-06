// Verify that a template version is wired into the repo.
// Run (needs .NET 10 SDK): dotnet run --file .agents/skills/verify-template-wiring/scripts/verify-wiring.cs -- Auth0.Mvc version10
using System.Text.RegularExpressions;

const string Usage = """
    Usage: dotnet run --file verify-wiring.cs -- <TemplateFolder> <versionN> [--repo-root PATH] [--no-banner]

    Example: dotnet run --file verify-wiring.cs -- Auth0.Mvc version10

    Prints one PASS:/FAIL:/SKIP: line per check. Exit code 0 if no FAIL, 1 otherwise,
    2 on usage errors.

    Options:
      --repo-root PATH   repository root (default: nearest parent folder containing Auth0Templates.csproj)
      --no-banner        skip the ebook banner check (Console has no banner; also for new types
                         where the user declined one)
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
var positional = new List<string>();
var noBanner = false;
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
        case "--no-banner":
            noBanner = true;
            break;
        default:
            positional.Add(args[i]);
            break;
    }
}
if (positional.Count != 2)
{
    Console.Error.WriteLine(Usage);
    return 2;
}

var tpl = positional[0];
var ver = positional[1];
var num = ver.StartsWith("version") ? ver["version".Length..] : ver;
var templatesDir = Path.Combine(root, "templates");
var baseDir = Path.Combine(templatesDir, tpl);
var vdir = Path.Combine(baseDir, ver);
var csproj = Path.Combine(root, "Auth0Templates.csproj");
var tests = Path.Combine(root, "tests", "TemplateValidation", "TemplateStructureTests.cs");
var failed = false;

void Pass(string m) => Console.WriteLine($"PASS: {m}");
void Skip(string m) => Console.WriteLine($"SKIP: {m}");
void Fail(string m) { Console.WriteLine($"FAIL: {m}"); failed = true; }
string Slash(string p) => p.Replace('\\', '/');
bool IsBuildOutput(string p) => Slash(p).Split('/').Any(s => s is "bin" or "obj");

if (!Directory.Exists(vdir))
{
    Fail($"folder templates/{tpl}/{ver} does not exist");
    return 1;
}
Pass($"folder templates/{tpl}/{ver} exists");

if (File.Exists(Path.Combine(vdir, "register-with-auth0.cmd"))) Pass("register-with-auth0.cmd present");
else Fail($"templates/{tpl}/{ver}/register-with-auth0.cmd is missing");

var csprojText = File.Exists(csproj) ? File.ReadAllText(csproj) : "";
var testsText = File.Exists(tests) ? File.ReadAllText(tests) : "";

var regs = Directory.GetDirectories(vdir, "registration", SearchOption.AllDirectories)
    .Where(d => !IsBuildOutput(d))
    .OrderBy(d => d, StringComparer.Ordinal)
    .ToList();
if (regs.Count == 0) Fail($"no registration folder under templates/{tpl}/{ver}");

foreach (var reg in regs)
{
    var rel = Slash(Path.GetRelativePath(templatesDir, reg));
    var win = @"templates\" + rel.Replace('/', '\\');

    var cfg = Path.Combine(reg, "config.json");
    if (File.Exists(cfg))
    {
        var text = File.ReadAllText(cfg);
        var missing = new[] { "AppName", "AppType", "RegistrationScriptFile" }
            .Where(k => !text.Contains($"\"{k}\"")).ToList();
        if (missing.Count == 0) Pass($"{rel}/config.json has required keys");
        else Fail($"{rel}/config.json lacks: {string.Join(' ', missing)}");
    }
    else Fail($"{rel}/config.json is missing");

    var count = Regex.Count(csprojText, Regex.Escape(win));
    if (count >= 2) Pass($"{win} in Auth0Templates.csproj (list and Copy)");
    else if (count == 1) Fail($"{win} appears once in Auth0Templates.csproj; add both the RegistrationFolders entry and the Copy line");
    else Fail($"{win} not in Auth0Templates.csproj");

    var parent = rel[..^"/registration".Length];
    if (testsText.Contains($"\"{parent}\"")) Pass($"TemplateStructureTests.cs lists \"{parent}\"");
    else Fail($"TemplateStructureTests.cs has no InlineData \"{parent}\"");
}

var templateJson = Path.Combine(baseDir, ".template.config", "template.json");
var shortName = "";
if (File.Exists(templateJson))
{
    var text = File.ReadAllText(templateJson);
    shortName = Regex.Match(text, "\"shortName\"\\s*:\\s*\"([^\"]*)\"").Groups[1].Value;
    if (text.Contains($"\"net{num}.0\"")) Pass($"template.json has framework net{num}.0");
    else Fail($"template.json has no net{num}.0 framework choice");
    if (Regex.IsMatch(text, $"\"source\"\\s*:\\s*\"[^\"]*{Regex.Escape(ver)}/?\""))
        Pass($"template.json has a source for {ver}");
    else Fail($"template.json has no source for {ver}");
}
else
{
    Skip($"{tpl} has no .template.config (source folder for another template; check that template's template.json references ../{tpl}/{ver}/)");
}

if (shortName.Length > 0)
{
    var docsDir = Path.Combine(root, "docs");
    var docFile = Directory.Exists(docsDir)
        ? Directory.GetFiles(docsDir, "*.md").OrderBy(f => f, StringComparer.Ordinal)
            .FirstOrDefault(f => File.ReadAllText(f).Contains($"dotnet new {shortName}"))
        : null;
    if (docFile is null) Fail($"no docs/*.md mentions 'dotnet new {shortName}'");
    else
    {
        var name = Path.GetFileName(docFile);
        Pass($"docs page documents 'dotnet new {shortName}'");
        var readme = Path.Combine(root, "README.md");
        if (File.Exists(readme) && File.ReadAllText(readme).Contains($"docs/{name}")) Pass($"README.md links {name}");
        else Fail($"README.md does not link docs/{name}");

        // Frameworks documented on the --framework line must equal the template.json choices.
        var choices = Regex.Matches(File.ReadAllText(templateJson), "\"choice\"\\s*:\\s*\"(net\\d+\\.0)\"")
            .Select(m => m.Groups[1].Value).ToHashSet();
        var lines = File.ReadAllLines(docFile);
        var idx = Array.FindIndex(lines, l => l.Contains("--framework"));
        if (idx < 0) Fail($"docs/{name} has no --framework option line");
        else
        {
            var text = string.Join(' ', lines.Skip(idx).Take(3));
            var documented = Regex.Matches(text, @"net\d+\.0").Select(m => m.Value).ToHashSet();
            if (documented.SetEquals(choices)) Pass($"docs/{name} --framework values match template.json ({string.Join(", ", choices.OrderBy(c => c, StringComparer.Ordinal))})");
            else Fail($"docs/{name} documents [{string.Join(", ", documented.OrderBy(c => c, StringComparer.Ordinal))}] but template.json offers [{string.Join(", ", choices.OrderBy(c => c, StringComparer.Ordinal))}]");
        }
    }
}

// Ebook banner: image file plus an a0.to/dotnet-templates/<slug> link
var expectedSlugs = new Dictionary<string, string[]>
{
    ["Auth0.Mvc"] = ["mvc"],
    ["Auth0.WebAPI"] = ["webapi"],
    ["Auth0.MinimalWebAPI"] = ["webapi"],
    ["Auth0.BlazorWebApp"] = ["blazor-webapp"],
    ["Auth0.BlazorWebAssembly"] = ["blazor-wasm"],
    ["Auth0.BlazorServer"] = ["blazor-server"],
    ["Auth0.Maui"] = ["maui"],
    ["Auth0.BackendForFrontend"] = ["bff", "webapi"],
};
if (noBanner || tpl == "Auth0.Console")
{
    Skip("ebook banner check (no banner for this template)");
}
else
{
    var textExt = new HashSet<string> { ".cs", ".cshtml", ".razor", ".xaml", ".html", ".js", ".jsx", ".ts", ".tsx" };
    var images = 0;
    var slugs = new HashSet<string>();
    var link = new Regex(@"a0\.to/dotnet-templates/([a-z0-9-]+)");
    foreach (var file in Directory.EnumerateFiles(vdir, "*", SearchOption.AllDirectories))
    {
        var p = Slash(file);
        if (IsBuildOutput(file) || p.Contains("/lib/") || p.Contains("/node_modules/")) continue;
        var fn = Path.GetFileName(file);
        if (fn is "auth0-dotnet-ebook.png" or "auth0_dotnet_ebook.png") images++;
        if (!textExt.Contains(Path.GetExtension(file))) continue;
        foreach (Match m in link.Matches(File.ReadAllText(file))) slugs.Add(m.Groups[1].Value);
    }
    if (images > 0) Pass($"ebook image present ({images} file(s))");
    else Fail("ebook image auth0-dotnet-ebook.png missing (run add-auth0-assets, or pass --no-banner)");
    if (expectedSlugs.TryGetValue(tpl, out var want))
    {
        var missing = want.Where(w => !slugs.Contains(w)).ToList();
        if (missing.Count == 0) Pass($"ebook link slug(s) present: {string.Join(", ", want)}");
        else Fail($"ebook link slug(s) missing: {string.Join(", ", missing)} (found: {string.Join(", ", slugs.OrderBy(x => x, StringComparer.Ordinal))})");
    }
    else if (slugs.Count > 0) Pass($"ebook link present (slug: {string.Join(", ", slugs)}; confirm it is the slug the user gave)");
    else Fail("no a0.to/dotnet-templates/<slug> link found (run add-auth0-assets, or pass --no-banner)");
}

// Hardcoded tenant values (skips registration/, bin/, obj/ and binary files)
var domain = new Regex(@"[A-Za-z0-9-]+\.(us|eu|au|jp)?\.?auth0\.com");
var hits = new List<string>();
foreach (var file in Directory.EnumerateFiles(vdir, "*", SearchOption.AllDirectories))
{
    var p = Slash(file);
    if (IsBuildOutput(file) || p.Contains("/registration/")) continue;
    string[] lines;
    try { lines = File.ReadAllLines(file); } catch { continue; }
    for (var n = 0; n < lines.Length; n++)
    {
        foreach (Match m in domain.Matches(lines[n]))
        {
            if (m.Value.Contains("yourdomain.auth0.com") || lines[n].Contains("https://auth0.com")) continue;
            hits.Add($"{Slash(Path.GetRelativePath(root, file))}:{n + 1}: {lines[n].Trim()}");
        }
    }
}
if (hits.Count == 0) Pass("no hardcoded Auth0 domain in version folder");
else Fail("possible hardcoded Auth0 domain:\n" + string.Join('\n', hits.Take(5)));

Console.WriteLine(failed ? "RESULT: failures found" : "RESULT: all checks passed");
return failed ? 1 : 0;
