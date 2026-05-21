using System.Runtime.CompilerServices;

namespace Clet.ConfigTests;

/// <summary>
/// Disables real driver I/O so config tests never interact with the terminal.
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
