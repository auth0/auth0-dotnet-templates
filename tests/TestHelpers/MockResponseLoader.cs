using System.IO;
using System.Reflection;

/// <summary>
/// Helper class for loading mock response files used in tests.
/// </summary>
public static class MockResponseLoader
{
    private static readonly string MockResponsesPath = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "",
        "TestHelpers",
        "MockResponses"
    );

    public static string LoadAppCreateSuccess() =>
        File.ReadAllText(Path.Combine(MockResponsesPath, "apps-create-success.json"));

    public static string LoadApiCreateSuccess() =>
        File.ReadAllText(Path.Combine(MockResponsesPath, "apis-create-success.json"));

    public static string LoadTenantList() =>
        File.ReadAllText(Path.Combine(MockResponsesPath, "tenant-list-v1.17.json"));

    public static string LoadApisList() =>
        File.ReadAllText(Path.Combine(MockResponsesPath, "apis-list-v1.16.json"));

    public static string LoadCliVersion(string version) =>
        File.ReadAllText(Path.Combine(MockResponsesPath, $"cli-version-{version}.txt"));
}
