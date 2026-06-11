using System.Text.Json.Nodes;
using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Terminal.Gui.Cli;

namespace Clet;

internal sealed class PickFileClet : ICliCommand<JsonNode?>
{
    public string PrimaryAlias => "pick-file";
    public IReadOnlyList<string> Aliases => ["pick-file", "file"];
    public string Description => "Opens a file picker dialog and returns the selected file path(s).";
    public CommandKind Kind => CommandKind.Input;
    public Type ResultType => typeof (JsonNode);

    public IReadOnlyList<CommandOptionDescriptor> Options =>
    [
        new("multi", "m", typeof(bool), "Allow selecting multiple files.", false, "false"),
        new("root", "r", typeof(string), "Starting directory (not a sandbox — user can navigate freely).", false, null),
        new("filter", "f", typeof(string), "File type filter (e.g. \"*.cs\").", false, null),
    ];

    public async Task<CommandResult<JsonNode?>> RunAsync (
        IApplication app,
        string? initial,
        CommandRunOptions options,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return new (CommandStatus.Cancelled, default, null, null);
        }

        bool multi = options.CommandOptions.TryGetValue ("multi", out string? multiStr) == true
                     && string.Equals (multiStr, "true", StringComparison.OrdinalIgnoreCase);

        string? root = options.CommandOptions.TryGetValue ("root", out string? rootStr) == true ? rootStr : null;
        string? filter = options.CommandOptions.TryGetValue ("filter", out string? filterStr) == true ? filterStr : null;
        string? startPath = root ?? initial;

        OpenDialog dialog = new ()
        {
            Title = options.Title ?? "Select a file (Enter to accept, Esc to cancel)",
            Width = Dim.Fill (),
            Height = 25,
            AllowsMultipleSelection = multi,
            BorderStyle = LineStyle.Rounded,
            ShadowStyle = null,
            SchemeName = CletStyling.BaseSchemeName,
        };
        dialog.Border.Thickness = new Thickness (0, 1, 0, 0);

        dialog.IsRunningChanged += (_, _) =>
        {
            if (dialog.IsRunning)
            {
                dialog.SchemeName = CletStyling.BaseSchemeName;
            }
        };

        if (startPath is not null)
        {
            dialog.Path = startPath;
        }

        string[] extensions = FileFilterParser.ParseExtensions (filter);

        if (extensions.Length > 0)
        {
            dialog.AllowedTypes.Add (new AllowedType (filter ?? "Filtered", extensions));
            dialog.AllowedTypes.Add (new AllowedTypeAny ());
        }

        try
        {
            await app.RunAsync (dialog, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return new (CommandStatus.Cancelled, default, null, null);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return new CommandResult<JsonNode?> { Status = CommandStatus.Cancelled };
        }

        IReadOnlyList<string> paths = dialog.FilePaths;

        if (paths.Count == 0)
        {
            return new CommandResult<JsonNode?> { Status = CommandStatus.Cancelled };
        }

        if (!multi)
        {
            return new CommandResult<JsonNode?> { Status = CommandStatus.Ok, Value = JsonValue.Create (paths[0]) };
        }

        List<string> sorted = new (paths);
        sorted.Sort (StringComparer.Ordinal);
        JsonArray arr = [];

        foreach (string p in sorted)
        {
            arr.Add ((JsonNode)p);
        }

        return new (CommandStatus.Ok, arr, null, null);

    }
}
