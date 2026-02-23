using System;
using System.Diagnostics;
using System.Threading.Tasks;

/// <summary>
/// Default implementation of IProcessExecutor that executes real system processes.
/// </summary>
public class ProcessExecutor : IProcessExecutor
{
    public async Task<string> RunCommandAsync(string command, string args)
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = command,
            Arguments = args,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        Displayer.DisplayVerbose($"About to run command: {command} {args}");

        using var proc = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start process");
        string output = await proc.StandardOutput.ReadToEndAsync();
        string errorText = await proc.StandardError.ReadToEndAsync();
        await proc.WaitForExitAsync();

        if (!string.IsNullOrEmpty(errorText) && errorText.Contains("error"))
        {
            throw new InvalidOperationException(errorText);
        }

        Displayer.DisplayCommandOutput(output);

        return output;
    }

    public void RunCommandAndForget(string command, string args)
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = command,
            Arguments = args,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        Displayer.DisplayVerbose($"About to launch command: {command} {args}");

        Process.Start(startInfo);
    }
}
