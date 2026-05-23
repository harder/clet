using System.Runtime.CompilerServices;

namespace Clet.UITests;

/// <summary>
/// Disables real driver I/O so UI tests use the test driver path.
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
