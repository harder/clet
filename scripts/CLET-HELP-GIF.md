# Hero GIF Recording Guide

Produces `docs/images/clet-help.gif` — a high-quality animated GIF demonstrating clet's help browser.

## Prerequisites

- [tuirec](https://github.com/tui-cs/tuirec) v0.3.4+ on PATH (`go install github.com/tui-cs/tuirec/cmd/tuirec@latest`)
- .NET 10 SDK (for building clet)
- `agg` is auto-downloaded by tuirec on first use

## Build clet

```powershell
dotnet build src/Clet -c Debug --nologo
```

## Record

```powershell
$binary = "./src/Clet/bin/Debug/net10.0/Clet.exe"

# Keystroke script — demonstrates:
#   1. Overview page loads (shows clet table with clickable links)
#   2. PageDown×4 to scroll through the full overview to the bottom
#   3. PageUp×4 to scroll back to the top
#   4. Click "select" link in the table → navigate to select help
#   5. Ctrl+CursorLeft (Back) → return to overview
#   6. Esc → quit
#
# Navigation: PageDown/PageUp for viewport scrolling (CursorDown navigates
# links, not the viewport). Mouse clicks on table links (col 3, row 12=select).
# Coordinates assume 100×30 terminal with viewport at top.
#
# Pacing: --keystroke-delay 80 (readable scrolling); wait: values add pauses
$ks = 'wait:1500,PageDown,PageDown,PageDown,PageDown,wait:600,PageUp,PageUp,PageUp,PageUp,wait:600,click:3:12,wait:1200,Ctrl+CursorLeft,wait:800,Esc'

tuirec record `
    --binary $binary `
    --args "help" `
    --show-command '$ clet help' `
    --keystrokes $ks `
    --startup-delay 2000 `
    --drain 500 `
    --cols 100 `
    --rows 30 `
    --keystroke-delay 80 `
    --cast-output ./artifacts/clet-help.cast

# Copy to final location
Copy-Item recording.gif ./docs/images/clet-help.gif -Force
```

## Demo sequence

| Time  | Feature              | Keys / Actions                                                       |
|-------|----------------------|----------------------------------------------------------------------|
| 0-2s  | Overview loads       | `$ clet help` typed; overview page renders with clet table           |
| 2-4s  | Scroll to bottom     | PageDown×4 (scroll through entire overview document)                 |
| 4-5s  | Scroll back to top   | PageUp×4 (return to top of overview)                                 |
| 5-6s  | Navigate to "select" | Click on "select" link in table (row 12)                             |
| 6-8s  | View select help     | Pause to view the select help page                                   |
| 8-9s  | Back to overview     | Ctrl+CursorLeft (Back)                                               |
| 9s    | Quit                 | Esc (exits the help browser)                                         |

## Tuning tips

- **Viewport scrolling**: PageDown/PageUp scroll the Markdown view's viewport. CursorDown/CursorUp navigate between links (NOT viewport scrolling).
- **Mouse clicks on table links**: The clet table renders alias names as clickable links. Click coordinates are column 3 (left edge of cell content), at the row where each alias appears. Coordinates depend on terminal size (100×30) and viewport being at top.
- **Click coordinates (100×30, viewport at top)**: select=row 12, text=row 15, int=row 18, decimal=row 20.
- **Back/Forward**: Ctrl+CursorLeft = Back, Ctrl+CursorRight = Forward (BrowseBar shortcuts).
- **Quit**: Esc or q both close the viewer.
- **Terminal size**: 100×30 gives comfortable width for help tables without horizontal overflow.
- **`--no-browse` is NOT used**: Browser mode (the default) enables back/forward navigation.

## Troubleshooting

1. **Click not navigating**: Verify click coordinates haven't shifted. Re-check by recording a single `click:3:12` and inspecting the cast output for navigation confirmation.
2. **Back button not working**: Ctrl+CursorLeft must be sent as `Ctrl+CursorLeft` (not `Ctrl+Left`). The escape sequence is `\x1b[1;5D`.
3. **Viewport not scrolling**: Use PageDown/PageUp. CursorDown does NOT scroll — it cycles focus between links.
4. **Recording too long**: Reduce `wait:` values. The demo should be ~9s at keystroke-delay 80.
5. **GIF too large**: At 100×30, expect ~0.3-0.5 MB. Reduce `--cols` to 80 for a smaller file.
6. **Viewport starts at bottom**: Known TG bug (tui-cs/Terminal.Gui#5365). The workaround in HelpClet resets viewport on initial render.
