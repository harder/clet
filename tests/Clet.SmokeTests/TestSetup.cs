using System.Runtime.CompilerServices;

namespace Clet.SmokeTests;

/// <summary>
/// Disables real driver I/O for the smoke-test host process.
/// </summary>
internal static class TestSetup
{
    [ModuleInitializer]
    internal static void Init ()
    {
        Environment.SetEnvironmentVariable ("DisableRealDriverIO", "1");
        Console.SetIn (TextReader.Null);
    }
}
