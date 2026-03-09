using System.Threading.Tasks;

/// <summary>
/// Interface for executing external processes. This abstraction allows for easier testing
/// by enabling process execution to be mocked.
/// </summary>
public interface IProcessExecutor
{
    /// <summary>
    /// Executes a command synchronously and returns its output.
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="args">Command arguments</param>
    /// <returns>The standard output from the command</returns>
    /// <exception cref="InvalidOperationException">Thrown when the command fails or produces error output</exception>
    Task<string> RunCommandAsync(string command, string args);

    /// <summary>
    /// Starts a command without waiting for it to complete (fire-and-forget).
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="args">Command arguments</param>
    void RunCommandAndForget(string command, string args);
}
