using Xunit;

namespace Clet.Tests;

public class EditorFileDisplayTests
{
    [Fact]
    public void FitPaths_KeepsTruncatedPathLabelsUnique ()
    {
        string first = Path.Combine (
            Path.GetTempPath (),
            "clet-one",
            "same-parent-name",
            "this-is-a-very-long-markdown-file-name.md");
        string second = Path.Combine (
            Path.GetTempPath (),
            "clet-two",
            "same-parent-name",
            "this-is-a-very-long-markdown-file-name.md");

        List<string> labels = EditorFileDisplay.FitPaths ([first, second], maxColumns: 37);

        Assert.Equal (2, labels.Distinct (StringComparer.Ordinal).Count ());
        Assert.All (labels, label => Assert.True (label.Length <= 37));
    }
}
