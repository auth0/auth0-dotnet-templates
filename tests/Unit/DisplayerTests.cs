using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace Unit
{
public class DisplayerTests : IDisposable
{
    private readonly StringWriter _stringWriter;
    private readonly TextWriter _originalOutput;

    public DisplayerTests()
    {
        _stringWriter = new StringWriter();
        _originalOutput = Console.Out;
        Console.SetOut(_stringWriter);
    }

    public void Dispose()
    {
        Console.SetOut(_originalOutput);
        _stringWriter.Dispose();
    }

    [Fact]
    public void DisplayVerbose_OutputsText_WhenVerboseIsTrue()
    {
        // Arrange
        Displayer.Verbose = true;

        // Act
        Displayer.DisplayVerbose("Test message");

        // Assert
        var output = _stringWriter.ToString();
        output.Should().Contain("Test message");
    }

    [Fact]
    public void DisplayVerbose_DoesNotOutput_WhenVerboseIsFalse()
    {
        // Arrange
        Displayer.Verbose = false;

        // Act
        Displayer.DisplayVerbose("Test message");

        // Assert
        var output = _stringWriter.ToString();
        output.Should().BeEmpty();
    }

    [Fact]
    public void DisplayError_AlwaysOutputsText()
    {
        // Arrange
        Displayer.Verbose = false;

        // Act
        Displayer.DisplayError("Error message");

        // Assert
        var output = _stringWriter.ToString();
        output.Should().Contain("ERROR: ************");
        output.Should().Contain("Error message");
        output.Should().Contain("*******************");
    }

    [Fact]
    public void DisplayErrorVerbose_OutputsText_WhenVerboseIsTrue()
    {
        // Arrange
        Displayer.Verbose = true;

        // Act
        Displayer.DisplayErrorVerbose("Verbose error");

        // Assert
        var output = _stringWriter.ToString();
        output.Should().Contain("ERROR: ************");
        output.Should().Contain("Verbose error");
    }

    [Fact]
    public void DisplayErrorVerbose_DoesNotOutput_WhenVerboseIsFalse()
    {
        // Arrange
        Displayer.Verbose = false;

        // Act
        Displayer.DisplayErrorVerbose("Verbose error");

        // Assert
        var output = _stringWriter.ToString();
        output.Should().BeEmpty();
    }

    [Fact]
    public void DisplayRegistrationData_OutputsClientId()
    {
        // Arrange
        var registrationData = new RegistrationData
        {
            client_id = "test-client-123",
            signing_keys = Array.Empty<SigningKeys>()
        };

        // Act
        Displayer.DisplayRegistrationData(registrationData);

        // Assert
        var output = _stringWriter.ToString();
        output.Should().Contain("Client ID: test-client-123");
    }

    [Fact]
    public void DisplayRegistrationData_OutputsAudience_ForApiType()
    {
        // Arrange
        var registrationData = new RegistrationData
        {
            identifier = "https://my-api.com",
            signing_keys = Array.Empty<SigningKeys>()
        };

        // Act
        Displayer.DisplayRegistrationData(registrationData);

        // Assert
        var output = _stringWriter.ToString();
        output.Should().Contain("Audience: https://my-api.com");
    }

    [Fact]
    public void DisplayRegistrationData_OutputsDomain_FromSigningKeys()
    {
        // Arrange
        var registrationData = new RegistrationData
        {
            signing_keys = new[] { new SigningKeys("/CN=test-tenant.auth0.com") }
        };

        // Act
        Displayer.DisplayRegistrationData(registrationData);

        // Assert
        var output = _stringWriter.ToString();
        output.Should().Contain("Auth0 domain: test-tenant.auth0.com");
    }

    [Fact]
    public void DisplayAppSettings_OutputsContent_WhenVerboseIsTrue()
    {
        // Arrange
        Displayer.Verbose = true;
        var content = @"{ ""Auth0"": { ""Domain"": ""test.auth0.com"" } }";

        // Act
        Displayer.DisplayAppSettings(content);

        // Assert
        var output = _stringWriter.ToString();
        output.Should().Contain("Appsettings content: ---------");
        output.Should().Contain(content);
        output.Should().Contain("---------------------------------");
    }

    [Fact]
    public void DisplayAppSettings_DoesNotOutput_WhenVerboseIsFalse()
    {
        // Arrange
        Displayer.Verbose = false;
        var content = @"{ ""Auth0"": { ""Domain"": ""test.auth0.com"" } }";

        // Act
        Displayer.DisplayAppSettings(content);

        // Assert
        var output = _stringWriter.ToString();
        output.Should().BeEmpty();
    }

    [Fact]
    public void DisplayCommandOutput_OutputsContent_WhenVerboseIsTrue()
    {
        // Arrange
        Displayer.Verbose = true;
        var commandOutput = "Command executed successfully";

        // Act
        Displayer.DisplayCommandOutput(commandOutput);

        // Assert
        var output = _stringWriter.ToString();
        output.Should().Contain("Command output: ---------");
        output.Should().Contain(commandOutput);
        output.Should().Contain("---------------------------------");
    }
}
}
