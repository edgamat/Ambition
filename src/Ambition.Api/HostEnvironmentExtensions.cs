#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class HostEnvironmentExtensions
{
    /// <summary>
    /// Checks if the current host environment name is "Local".
    /// </summary>
    /// <param name="hostEnvironment">An instance of <see cref="IHostEnvironment"/>.</param>
    /// <returns>True if the environment name is "Local", otherwise false.</returns>
    public static bool IsLocal(this IHostEnvironment hostEnvironment)
    {
        ArgumentNullException.ThrowIfNull(hostEnvironment);

        return hostEnvironment.IsEnvironment("Local");
    }
}

