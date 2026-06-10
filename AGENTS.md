# What This Repository Is

A NuGet template pack (`Auth0.Templates`) that provides `dotnet new` templates for .NET applications pre-configured with Auth0 authentication. Templates support .NET 8, 9, and 10.

# Commands

## Build and install

```bash
dotnet pack  -o ./output                             # builds NuGet package into ./output
dotnet new install ./output/Auth0.Templates.*.nupkg  # install the packed template
npm run pack-install                                 # uninstall + pack + install in one step
```

## Testing

```bash
cd tests && dotnet test                              # all fast tests (unit + integration)
dotnet test --filter "FullyQualifiedName~Unit"
dotnet test --filter "FullyQualifiedName~Integration"
dotnet test --filter "FullyQualifiedName~TemplateValidation.TemplateStructureTests"
dotnet test --filter "FullyQualifiedName~CliWrapperTests.CreateAudience_TransformsAppNameToValidUrl"  # single test method
dotnet test --collect:"XPlat Code Coverage"
```

Template instantiation tests require building the package first:
```bash
dotnet pack && cd tests && dotnet test --filter "FullyQualifiedName~TemplateInstantiationTests"
```

# Architecture

## Project layout

- `Auth0Templates.csproj` — NuGet template pack definition (targets `netstandard2.0`)
- `templates/` — one subfolder per template, each with `.template.config/template.json` and `versionX/` subdirectories for each supported .NET version
- `cli-wrapper/` — a `net7.0` helper binary bundled into every template
- `tests/` — xUnit v3 test suite (targets `net10.0`)
- `assets` - Images and other assets

## Build-time wiring (Auth0Templates.csproj)

The `AddCliWrapper` MSBuild target runs before `GenerateNuspec`. It:

1. Publishes `cli-wrapper` in Release mode
2. Copies the resulting binaries to every template's `registration/` folder

This means **cli-wrapper binaries in `templates/*/registration/` are generated artifacts** — do not edit them directly. The gitignore excludes `**/registration/cli-wrapper*`.

## CLI wrapper (`cli-wrapper/`)

The CLI wrapper is executed at project-creation time to register the new Auth0 application. It calls the Auth0 CLI (`auth0` binary) via `IProcessExecutor`/`ProcessExecutor`. Key files:

- `CliWrapper.cs` — orchestrates Auth0 app/API registration
- `IProcessExecutor.cs` / `ProcessExecutor.cs` — dependency-injectable process execution (enables mocking in tests)
- `ConfigData.cs`, `RegistrationData.cs`, `TenantData.cs` — data models for Auth0 CLI responses

## Test suite (`tests/`)

| Category | Filter | Notes |
|---|---|---|
| Unit | `~Unit` | Pure logic, no I/O |
| Integration | `~Integration` | Mocked `IProcessExecutor` via Moq |
| TemplateStructureTests | `~TemplateValidation.TemplateStructureTests` | Scans template files |
| TemplateInstantiationTests | `~TemplateInstantiationTests` | Generates real projects; needs `dotnet pack` first |

Test naming convention: `Method_Does_WhenCondition`. Use FluentAssertions (`.Should()`) for assertions. Mock responses are JSON/text files in `tests/TestHelpers/MockResponses/`.

## Adding a new template version

When adding support for a new .NET version (e.g., version11):

1. Create `templates/<TemplateName>/version11/` with the project files and a `registration/` subfolder
2. Add the new registration path to the `<RegistrationFolders>` list in `Auth0Templates.csproj`
3. Add template validation test cases in `tests/TemplateValidation/TemplateStructureTests.cs`

# Documentation

- The documentation lives in the `docs` folder
- IMPORTANT: The documentation in `docs` must be in sync with the actual implementation

  ## Security

- Do **not** hardcode Auth0 credentials (domain, client ID, client secret) anywhere in templates or source code
- Templates use placeholder values that are replaced at instantiation time or via the Auth0 CLI registration flow. The placeholder to use are:
  - `{DOMAIN}` for the Auth0 domain
  - `{CLIENT_ID}` for the client ID
  - `{CLIENT_SECRET}` for the client secret
  - `{AUDIENCE}` for the API audience identifier