using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Unit
{
public class RegistrationDataTests
{
    [Fact]
    public void RegistrationData_Deserializes_FromAppCreateResponse()
    {
        // Arrange
        var json = MockResponseLoader.LoadAppCreateSuccess();

        // Act
        var registration = JsonSerializer.Deserialize<RegistrationData>(json);

        // Assert
        registration.Should().NotBeNull();
        registration!.name.Should().Be("Test Application");
        registration.client_id.Should().Be("test-client-id-12345");
        registration.signing_keys.Should().HaveCount(1);
        registration.signing_keys[0].subject.Should().Be("/CN=test-tenant.auth0.com");
    }

    [Fact]
    public void RegistrationData_Deserializes_FromApiCreateResponse()
    {
        // Arrange
        var json = MockResponseLoader.LoadApiCreateSuccess();

        // Act
        var registration = JsonSerializer.Deserialize<RegistrationData>(json);

        // Assert
        registration.Should().NotBeNull();
        registration!.name.Should().Be("Test API");
        registration.identifier.Should().Be("https://test-api.com");
    }

    [Fact]
    public void RegistrationData_Constructor_InitializesProperties()
    {
        // Act
        var registration = new RegistrationData(
            "MyApp",
            "client-123",
            "https://my-api.com",
            new[] { new SigningKeys("/CN=test.auth0.com") }
        );

        // Assert
        registration.name.Should().Be("MyApp");
        registration.client_id.Should().Be("client-123");
        registration.identifier.Should().Be("https://my-api.com");
        registration.signing_keys.Should().HaveCount(1);
        registration.signing_keys[0].subject.Should().Be("/CN=test.auth0.com");
    }

    [Fact]
    public void RegistrationData_DefaultConstructor_HasEmptyValues()
    {
        // Act
        var registration = new RegistrationData();

        // Assert
        registration.name.Should().BeEmpty();
        registration.client_id.Should().BeEmpty();
        registration.identifier.Should().BeEmpty();
        registration.signing_keys.Should().BeEmpty();
    }

    [Fact]
    public void SigningKeys_Constructor_InitializesSubject()
    {
        // Act
        var signingKey = new SigningKeys("/CN=test.auth0.com");

        // Assert
        signingKey.subject.Should().Be("/CN=test.auth0.com");
    }

    [Fact]
    public void SigningKeys_DefaultConstructor_HasEmptySubject()
    {
        // Act
        var signingKey = new SigningKeys();

        // Assert
        signingKey.subject.Should().BeEmpty();
    }
}
}
