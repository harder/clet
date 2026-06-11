using Xunit;

namespace Clet.Tests;

public class EditorCletSourceTests
{
    [Fact]
    public void EditorViewportSync_UsesLiveMarkdownPreviewReference ()
    {
        string source = File.ReadAllText (FindRepoFile (Path.Combine ("src", "Clet", "Clets", "Viewer", "EditorClet.cs")));

        Assert.DoesNotContain ("Markdown? preview = markdownPreview;", source);
        Assert.DoesNotContain ("if (preview is null || syncingScroll)", source);
        Assert.Contains ("if (markdownPreview is null || syncingScroll)", source);
    }

    private static string FindRepoFile (string relativePath)
    {
        DirectoryInfo? directory = new (AppContext.BaseDirectory);

        while (directory is not null)
        {
            string candidate = Path.Combine (directory.FullName, relativePath);
            if (File.Exists (candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException ($"Could not find repository file '{relativePath}'.");
    }
}
