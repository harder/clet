using System.Diagnostics.CodeAnalysis;
using Terminal.Gui.Configuration;

namespace Clet.ConfigTestProxies;

[SuppressMessage ("ReSharper", "UnusedMember.Global",
    Justification = "Terminal.Gui ConfigurationManager discovers these properties by reflection in the test entry assembly.")]
[SuppressMessage ("ReSharper", "UnusedType.Global",
    Justification = "Terminal.Gui ConfigurationManager discovers this type by reflection in the test entry assembly.")]
internal static class EditorSettings
{
    [ConfigurationProperty (Scope = typeof (SettingsScope))]
    public static bool LineNumbers
    {
        get => Clet.EditorSettings.LineNumbers;
        set => Clet.EditorSettings.LineNumbers = value;
    }

    [ConfigurationProperty (Scope = typeof (SettingsScope))]
    public static bool FoldIndicators
    {
        get => Clet.EditorSettings.FoldIndicators;
        set => Clet.EditorSettings.FoldIndicators = value;
    }

    [ConfigurationProperty (Scope = typeof (SettingsScope))]
    public static bool WordWrap
    {
        get => Clet.EditorSettings.WordWrap;
        set => Clet.EditorSettings.WordWrap = value;
    }

    [ConfigurationProperty (Scope = typeof (SettingsScope))]
    public static bool ShowTabs
    {
        get => Clet.EditorSettings.ShowTabs;
        set => Clet.EditorSettings.ShowTabs = value;
    }

    [ConfigurationProperty (Scope = typeof (SettingsScope))]
    public static bool Scrollbars
    {
        get => Clet.EditorSettings.Scrollbars;
        set => Clet.EditorSettings.Scrollbars = value;
    }

    [ConfigurationProperty (Scope = typeof (SettingsScope))]
    public static int IndentSize
    {
        get => Clet.EditorSettings.IndentSize;
        set => Clet.EditorSettings.IndentSize = value;
    }

    [ConfigurationProperty (Scope = typeof (SettingsScope))]
    public static bool ConvertTabsToSpaces
    {
        get => Clet.EditorSettings.ConvertTabsToSpaces;
        set => Clet.EditorSettings.ConvertTabsToSpaces = value;
    }

    [ConfigurationProperty (Scope = typeof (SettingsScope))]
    public static bool AutoIndent
    {
        get => Clet.EditorSettings.AutoIndent;
        set => Clet.EditorSettings.AutoIndent = value;
    }

    [ConfigurationProperty (Scope = typeof (SettingsScope))]
    public static bool AutoComplete
    {
        get => Clet.EditorSettings.AutoComplete;
        set => Clet.EditorSettings.AutoComplete = value;
    }
}

[SuppressMessage ("ReSharper", "UnusedMember.Global",
    Justification = "Terminal.Gui ConfigurationManager discovers this property by reflection in the test entry assembly.")]
[SuppressMessage ("ReSharper", "UnusedType.Global",
    Justification = "Terminal.Gui ConfigurationManager discovers this type by reflection in the test entry assembly.")]
internal static class FileAccessSettings
{
    [ConfigurationProperty (Scope = typeof (SettingsScope))]
    public static List<string> AllowedPaths
    {
        get => Clet.FileAccessSettings.AllowedPaths;
        set => Clet.FileAccessSettings.AllowedPaths = value;
    }
}
