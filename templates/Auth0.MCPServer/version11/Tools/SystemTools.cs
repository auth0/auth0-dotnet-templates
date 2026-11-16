using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;

internal enum SystemState
{
    Stopped,
    Running,
    Paused
}

/// <summary>
/// Sample MCP tools that require specific Auth0 permissions in the access token.
/// </summary>
internal class SystemTools
{
    private static SystemState _state = SystemState.Stopped;

    [McpServerTool]
    [Authorize(Policy = McpPermissions.GetSystemStatePolicy)]
    [Description("Gets the current state of the system. Requires the tool:getsystemstate permission.")]
    public string GetSystemState() => _state.ToString();

    [McpServerTool]
    [Authorize(Policy = McpPermissions.SetSystemStatePolicy)]
    [Description("Sets the state of the system. Requires the tool:setsystemstate permission.")]
    public string SetSystemState([Description("The new state of the system")] SystemState state)
    {
        _state = state;
        return $"System state set to {_state}";
    }
}
