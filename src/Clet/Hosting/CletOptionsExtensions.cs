using Terminal.Gui.Cli;

namespace Clet;

/// <summary>
/// Extension methods for accessing clet-specific global options from <see cref="CommandRunOptions.Extensions"/>.
/// These options are registered in Program.cs as <see cref="GlobalOptionDescriptor"/> entries.
/// </summary>
internal static class CletOptionsExtensions
{
    /// <summary>Paths explicitly allowed for file reading (bypasses extension + cwd checks).</summary>
    public static IReadOnlyList<string> GetAllowedFiles (this CommandRunOptions options)
        => options.GetExtensionList ("allow-file");

    /// <summary>When true, binary file content (NUL bytes) is permitted.</summary>
    public static bool GetAllowBinary (this CommandRunOptions options)
        => options.HasExtension ("allow-binary");

    /// <summary>When true, disables browser-mode navigation (back/forward) for viewer clets.</summary>
    public static bool GetNoBrowse (this CommandRunOptions options)
        => options.HasExtension ("no-browse");
}
