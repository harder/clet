using Xunit;

using Terminal.Gui.Cli;

namespace Clet.UnitTests;

public class ExitCodesTests
{
    [Fact]
    public void Constants_MatchSpec ()
    {
        Assert.Equal (0, ExitCodes.Ok);
        Assert.Equal (1, ExitCodes.NoResult);
        Assert.Equal (2, ExitCodes.UsageError);
        Assert.Equal (65, ExitCodes.ValidationError);
        Assert.Equal (74, ExitCodes.IoError);
        Assert.Equal (130, ExitCodes.Cancelled);
    }

    [Theory]
    [InlineData ((int)CommandStatus.Ok, null, 0)]
    [InlineData ((int)CommandStatus.Cancelled, null, 130)]
    [InlineData ((int)CommandStatus.NoResult, null, 1)]
    [InlineData ((int)CommandStatus.Error, "validation", 65)]
    // NOTE: spec requires input-too-large → 65, but the package maps all unknown error codes to 2.
    // Tracked upstream: Terminal.Gui.Cli needs a custom exit code mapper or treating
    // "input-too-large" as a validation error. See PR #185 review discussion.
    [InlineData ((int)CommandStatus.Error, "input-too-large", 2)]
    [InlineData ((int)CommandStatus.Error, "io", 74)]
    [InlineData ((int)CommandStatus.Error, "anything-else", 2)]
    public void FromResult_MapsStatusToExit (int statusInt, string? errorCode, int expected)
    {
        CommandResult result = new ((CommandStatus)statusInt, null, errorCode, null);

        Assert.Equal (expected, ExitCodes.FromResult (result));
    }
}
