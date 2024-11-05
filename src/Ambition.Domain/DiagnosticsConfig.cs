using System.Diagnostics;

namespace Ambition.Domain
{
    public static class DiagnosticsConfig
    {
        public const string ServiceName = "Ambition";

        public static ActivitySource Source { get; } = new(ServiceName);
    }
}
