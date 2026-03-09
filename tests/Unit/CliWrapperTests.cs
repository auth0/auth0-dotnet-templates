using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace Unit
{
    public class CliWrapperTests
    {
        [Theory]
        [InlineData("My App", "https://my-app.com")]
        [InlineData("Test API", "https://test-api.com")]
        [InlineData("Test@API#123", "https://test-api-123.com")]
        [InlineData("App With Spaces", "https://app-with-spaces.com")]
        [InlineData("Special!Chars$Here%", "https://special-chars-here-.com")]
        [InlineData("UPPERCASE", "https://uppercase.com")]
        [InlineData("mixed-CASE-app", "https://mixed-case-app.com")]
        public void CreateAudience_TransformsAppNameToValidUrl(string appName, string expected)
        {
            // Arrange
            var mockExecutor = new Mock<IProcessExecutor>();
            var wrapper = CreateCliWrapperWithMockConfig(mockExecutor.Object);

            // Act
            var result = wrapper.CreateAudience(appName);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("1.20.0", true)]
        [InlineData("1.17.0", true)]
        [InlineData("1.0.1", true)]
        [InlineData("1.0.0", false)]
        [InlineData("0.9.9", false)]
        public async Task IsAuth0CliInstalled_ChecksVersionCorrectly(string version, bool expected)
        {
            // Arrange
            var mockExecutor = new Mock<IProcessExecutor>();
            mockExecutor
                .Setup(e => e.RunCommandAsync("auth0", "--version"))
                .ReturnsAsync($"auth0 version {version} abc123");

            var wrapper = CreateCliWrapperWithMockConfig(mockExecutor.Object);

            // Act
            var result = await wrapper.IsAuth0CliInstalled();

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public async Task IsAuth0CliInstalled_ReturnsFalse_WhenCliNotFound()
        {
            // Arrange
            var mockExecutor = new Mock<IProcessExecutor>();
            mockExecutor
                .Setup(e => e.RunCommandAsync("auth0", "--version"))
                .ThrowsAsync(new InvalidOperationException("Command not found"));

            var wrapper = CreateCliWrapperWithMockConfig(mockExecutor.Object);

            // Act
            var result = await wrapper.IsAuth0CliInstalled();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsAuth0CliInstalled_ReturnsFalse_WhenVersionStringMalformed()
        {
            // Arrange
            var mockExecutor = new Mock<IProcessExecutor>();
            mockExecutor
                .Setup(e => e.RunCommandAsync("auth0", "--version"))
                .ReturnsAsync("invalid output");

            var wrapper = CreateCliWrapperWithMockConfig(mockExecutor.Object);

            // Act
            var result = await wrapper.IsAuth0CliInstalled();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Register_CreatesApplication_WhenAppTypeIsRegular()
        {
            // Arrange
            var mockExecutor = new Mock<IProcessExecutor>();
            var expectedJson = @"{
            ""client_id"": ""test-client-id"",
            ""client_secret"": ""test-secret"",
            ""name"": ""TestApp"",
            ""signing_keys"": [{""subject"": ""/CN=test.auth0.com""}]
        }";

            mockExecutor
                .Setup(e => e.RunCommandAsync("auth0", It.IsAny<string>()))
                .ReturnsAsync((string cmd, string args) =>
                {
                    if (args.Contains("--version"))
                        return "auth0 version 1.20.0 abc";
                    if (args.Contains("apps create"))
                        return expectedJson;
                    return "";
                });

            var wrapper = CreateCliWrapperWithMockConfig(mockExecutor.Object);

            // Act
            var result = await wrapper.Register();

            // Assert
            result.client_id.Should().Be("test-client-id");
            result.client_secret.Should().Be("test-secret");
            mockExecutor.Verify(
                e => e.RunCommandAsync("auth0", It.Is<string>(s => s.Contains("apps create"))),
                Times.Once
            );
        }

        [Fact]
        public async Task Register_CreatesApi_WhenAppTypeIsApi()
        {
            // Arrange
            var mockExecutor = new Mock<IProcessExecutor>();
            var expectedJson = @"{
            ""identifier"": ""https://test-api.com"",
            ""name"": ""TestAPI""
        }";

            mockExecutor
                .Setup(e => e.RunCommandAsync("auth0", It.IsAny<string>()))
                .ReturnsAsync((string cmd, string args) =>
                {
                    if (args.Contains("--version"))
                        return "auth0 version 1.20.0 abc";
                    if (args.Contains("apis create"))
                        return expectedJson;
                    return "";
                });

            var wrapper = CreateCliWrapperWithMockConfig(
                mockExecutor.Object,
                appType: "api"
            );

            // Act
            var result = await wrapper.Register();

            // Assert
            result.identifier.Should().Be("https://test-api.com");
            mockExecutor.Verify(
                e => e.RunCommandAsync("auth0", It.Is<string>(s => s.Contains("apis create"))),
                Times.Once
            );
        }

        [Fact]
        public async Task Register_HandlesAuth0CliError_Gracefully()
        {
            // Arrange
            var mockExecutor = new Mock<IProcessExecutor>();
            mockExecutor
                .Setup(e => e.RunCommandAsync("auth0", It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Auth0 CLI error"));

            var wrapper = CreateCliWrapperWithMockConfig(mockExecutor.Object);

            // Act
            var result = await wrapper.Register();

            // Assert
            result.client_id.Should().BeEmpty();
        }

        /// <summary>
        /// Helper method to create a CliWrapper with a mock config file.
        /// This is needed because CliWrapper constructor reads config.json from AppContext.BaseDirectory.
        /// </summary>
        private CliWrapper CreateCliWrapperWithMockConfig(
            IProcessExecutor executor,
            string appType = "regular")
        {
            var configJson = $@"{{
            ""AppName"": ""TestApp"",
            ""AppDescription"": ""Test Description"",
            ""AppType"": ""{appType}"",
            ""Callbacks"": ""https://localhost:5001/callback"",
            ""LogoutUrls"": ""https://localhost:5001/"",
            ""AppSettingsFiles"": [],
            ""RegistrationScriptFile"": ""register.cmd"",
            ""Verbose"": false
             }}";

            var testConfigPath = System.IO.Path.Combine(AppContext.BaseDirectory, "config.json");
            System.IO.File.WriteAllText(testConfigPath, configJson);

            return new CliWrapper(executor);

        }
    }
}