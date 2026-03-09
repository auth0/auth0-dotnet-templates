using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Unit
{
public class TenantDataTests
{
    [Fact]
    public void TenantData_Deserializes_FromTenantListResponse()
    {
        // Arrange
        var json = MockResponseLoader.LoadTenantList();

        // Act
        var tenants = JsonSerializer.Deserialize<TenantData[]>(json);

        // Assert
        tenants.Should().NotBeNull();
        tenants.Should().HaveCount(2);
        
        tenants![0].name.Should().Be("test-tenant.auth0.com");
        tenants[0].active.Should().BeTrue();
        
        tenants[1].name.Should().Be("other-tenant.auth0.com");
        tenants[1].active.Should().BeFalse();
    }

    [Fact]
    public void TenantData_Serializes_ToValidJson()
    {
        // Arrange
        var tenant = new TenantData
        {
            name = "my-tenant.auth0.com",
            active = true
        };

        // Act
        var json = JsonSerializer.Serialize(tenant);
        var deserialized = JsonSerializer.Deserialize<TenantData>(json);

        // Assert
        deserialized.Should().BeEquivalentTo(tenant);
    }

    [Fact]
    public void TenantData_HasDefaultValues()
    {
        // Act
        var tenant = new TenantData();

        // Assert
        tenant.name.Should().BeEmpty();
        tenant.active.Should().BeFalse();
    }

    [Fact]
    public void TenantData_FindsActiveTenant_InArray()
    {
        // Arrange
        var tenants = new[]
        {
            new TenantData { name = "tenant1.auth0.com", active = false },
            new TenantData { name = "tenant2.auth0.com", active = true },
            new TenantData { name = "tenant3.auth0.com", active = false }
        };

        // Act
        var activeTenant = System.Array.Find(tenants, t => t.active);

        // Assert
        activeTenant.Should().NotBeNull();
        activeTenant!.name.Should().Be("tenant2.auth0.com");
    }
}
}
