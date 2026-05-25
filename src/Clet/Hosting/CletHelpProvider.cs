using System.Reflection;
using System.Text;
using Terminal.Gui.Cli;

namespace Clet;

/// <summary>
/// Custom <see cref="IHelpProvider"/> for clet. Reads embedded markdown resources from the
/// <c>Help/</c> directory, applies template substitutions ({{CLET_TABLE}}, {{VERSION}}),
/// and falls back to <see cref="MetadataHelpProvider"/> for command-level help.
/// </summary>
internal sealed class CletHelpProvider : IHelpProvider
{
    private readonly MetadataHelpProvider _fallback = new ();

    /// <inheritdoc />
    public string? GetRootHelp (ICommandRegistry registry)
    {
        string? markdown = ReadEmbeddedHelp ("overview.md");

        if (markdown is null)
        {
            return _fallback.GetRootHelp (registry);
        }

        // Inject dynamic content into the template
        string cletTable = BuildCletTableMarkdown (registry).TrimEnd ();
        markdown = markdown.Replace ("{{CLET_TABLE}}", cletTable);
        markdown = markdown.Replace ("{{VERSION}}", $"v{VersionInfo.GetCletVersion ()} (Terminal.Gui {VersionInfo.GetTerminalGuiVersion ()})");

        return markdown;
    }

    /// <inheritdoc />
    public string? GetCommandHelp (ICliCommand command)
    {
        // Try embedded resource first, then fall back to metadata-generated help
        string? embedded = ReadEmbeddedHelp ($"{command.PrimaryAlias}.md");

        if (embedded is not null)
        {
            return embedded;
        }

        return _fallback.GetCommandHelp (command);
    }

    private static string? ReadEmbeddedHelp (string resourceSuffix)
    {
        Assembly assembly = typeof (CletHelpProvider).Assembly;
        string? resourceName = assembly.GetManifestResourceNames ()
            .FirstOrDefault (n => n.EndsWith (resourceSuffix, StringComparison.Ordinal));

        if (resourceName is null)
        {
            return null;
        }

        using Stream stream = assembly.GetManifestResourceStream (resourceName)!;
        using StreamReader reader = new (stream);

        return reader.ReadToEnd ();
    }

    private static string BuildCletTableMarkdown (ICommandRegistry registry)
    {
        StringBuilder sb = new ();
        sb.AppendLine ("## Available Clets");
        sb.AppendLine ();
        sb.AppendLine ("| Alias | Description | Options |");
        sb.AppendLine ("|-------|-------------|---------|");

        foreach (ICliCommand command in registry.All)
        {
            string aliases = command.Aliases.Count <= 1
                ? $"[{command.PrimaryAlias}](help:{command.PrimaryAlias})"
                : string.Join (", ", command.Aliases.Select (a => $"[{a}](help:{a})"));

            string options = BuildOptionsColumn (command);

            sb.AppendLine ($"| {aliases} | {command.Description} | {options} |");
        }

        return sb.ToString ();
    }

    private static string BuildOptionsColumn (ICliCommand command)
    {
        List<string> parts = [];

        foreach (CommandOptionDescriptor opt in command.Options)
        {
            parts.Add ($"`--{opt.Name}`");
        }

        if (command.AcceptsPositionalArgs)
        {
            parts.Add ("`args...`");
        }

        return parts.Count == 0 ? "" : string.Join (", ", parts);
    }
}
