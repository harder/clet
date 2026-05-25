using Xunit;

using Terminal.Gui.Cli;

namespace Clet.UnitTests;

public class CletHelpProviderTests
{
    [Fact]
    public void GetRootHelp_NormalizesLegacyCletHelpScheme ()
    {
        CletHelpProvider provider = new ();
        ICommandRegistry registry = new CommandRegistry ();
        BuiltInCommands.RegisterAll (registry);

        string? help = provider.GetRootHelp (registry);

        Assert.NotNull (help);

        // The overview.md contains "clet:help:help" which must be normalized to "help:help"
        Assert.DoesNotContain ("clet:help:", help);
        Assert.Contains ("help:", help);
    }
}
