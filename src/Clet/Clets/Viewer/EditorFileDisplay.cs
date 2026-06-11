namespace Clet;

internal static class EditorFileDisplay
{
    public static string FitPath (string displayName, int maxColumns)
    {
        if (maxColumns <= 0)
        {
            return string.Empty;
        }

        if (displayName.Length <= maxColumns)
        {
            return displayName;
        }

        if (maxColumns == 1)
        {
            return "…";
        }

        string filename = Path.GetFileName (displayName);

        if (string.IsNullOrEmpty (filename))
        {
            return FitFileName (displayName, maxColumns);
        }

        string separator = displayName.Contains ('\\', StringComparison.Ordinal) ? "\\" : "/";
        string root = Path.GetPathRoot (displayName) ?? string.Empty;
        string relativePath = string.IsNullOrEmpty (root) ? displayName : displayName[root.Length..];
        string[] parts = relativePath.Split (
            ['\\', '/'],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (filename.Length + 2 > maxColumns)
        {
            return string.Concat ("…", separator, FitFileName (filename, maxColumns - 2));
        }

        for (int i = 0; i < parts.Length; i++)
        {
            string suffix = string.Join (separator, parts.Skip (i));
            string candidate = string.IsNullOrEmpty (root)
                ? string.Concat ("…", separator, suffix)
                : string.Concat (root, "…", separator, suffix);

            if (candidate.Length <= maxColumns)
            {
                return candidate;
            }
        }

        string fallback = string.IsNullOrEmpty (root)
            ? string.Concat ("…", separator, filename)
            : string.Concat (root, "…", separator, filename);

        return fallback.Length <= maxColumns ? fallback : string.Concat ("…", separator, filename);
    }

    public static List<string> FitPaths (IEnumerable<string> displayNames, int maxColumns)
    {
        string[] sourceNames = [.. displayNames];
        List<string> labels = [.. sourceNames.Select (displayName => FitPath (displayName, maxColumns))];

        foreach (IGrouping<string, (string label, int index)> collision in labels
                     .Select ((label, index) => (label, index))
                     .GroupBy (item => item.label, StringComparer.Ordinal)
                     .Where (group => group.Count () > 1))
        {
            int ordinal = 1;

            foreach ((_, int index) in collision)
            {
                string suffix = $" [{ordinal++}]";
                int labelColumns = Math.Max (1, maxColumns - suffix.Length);
                labels[index] = string.Concat (FitPath (sourceNames[index], labelColumns), suffix);
            }
        }

        return labels;
    }

    private static string FitFileName (string fileName, int maxColumns)
    {
        if (maxColumns <= 0)
        {
            return string.Empty;
        }

        if (fileName.Length <= maxColumns)
        {
            return fileName;
        }

        if (maxColumns == 1)
        {
            return "…";
        }

        string extension = Path.GetExtension (fileName);

        if (!string.IsNullOrEmpty (extension) && extension.Length + 2 <= maxColumns)
        {
            int stemColumns = maxColumns - extension.Length - 1;
            return string.Concat (fileName.AsSpan (0, stemColumns), "…", extension);
        }

        return string.Concat (fileName.AsSpan (0, maxColumns - 1), "…");
    }
}
