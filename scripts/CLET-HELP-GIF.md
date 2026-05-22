# Hero GIF Recording Guide

Produces `docs/images/clet-help.gif` — a high-quality animated GIF demonstrating clet's help browser.

## Prerequisites

- [tuirec](https://github.com/gui-cs/tuirec) v0.3.4+ on PATH (`go install github.com/gui-cs/tuirec/cmd/tuirec@latest`)
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
#   2. Click "select" link in the table → navigate to select help
#   3. Scroll within the select help page (CursorDown)
#   4. Ctrl+CursorLeft (Back) → return to overview
#   5. Click "int" link in the table → navigate to int help
#   6. Scroll within the int help page (CursorDown)
#   7. Ctrl+CursorLeft (Back) → return to overview
#   8. Esc → quit
#
# Navigation: Mouse clicks on table links (col 3, row 12=select, 18=int).
# Coordinates assume 100×30 terminal with no prior scrolling.
#
# Pacing: --keystroke-delay 50 (snappy); wait: values add pauses for readability
$ks = 'wait:1500,wait:500,click:3:12,wait:1000,CursorDown,CursorDown,CursorDown,CursorDown,CursorDown,CursorDown,CursorDown,CursorDown,wait:800,Ctrl+CursorLeft,wait:800,click:3:18,wait:1000,CursorDown,CursorDown,CursorDown,CursorDown,CursorDown,CursorDown,wait:800,Ctrl+CursorLeft,wait:800,Esc'

tuirec record `
    --binary $binary `
    --args "help" `
    --show-command '$ clet help' `
    --keystrokes $ks `
    --startup-delay 2000 `
    --drain 500 `
    --cols 100 `
    --rows 30 `
    --keystroke-delay 50 `
    --cast-output ./artifacts/clet-help.cast

# Copy to final location
Copy-Item recording.gif ./docs/images/clet-help.gif -Force
```

## Demo sequence

| Time  | Feature              | Keys / Actions                                                       |
|-------|----------------------|----------------------------------------------------------------------|
| 0-2s  | Overview loads       | `$ clet help` typed; overview page renders with clet table           |
| 2-3s  | Navigate to "select" | Click on "select" link in table (row 12)                             |
| 3-5s  | Scroll select help   | CursorDown×8 (show options and examples)                             |
| 5-6s  | Back to overview     | Ctrl+CursorLeft (Back)                                               |
| 6-7s  | Navigate to "int"    | Click on "int" link in table (row 18)                                |
| 7-9s  | Scroll int help      | CursorDown×6 (show options table)                                    |
| 9-10s | Back to overview     | Ctrl+CursorLeft (Back)                                               |
| 10s   | Quit                 | Esc (exits the help browser)                                         |

## Tuning tips

- **Mouse clicks on table links**: The clet table renders alias names as clickable links. Click coordinates are column 3 (left edge of cell content), at the row where each alias appears. Coordinates depend on terminal size (100×30) and no prior viewport scrolling.
- **Click coordinates (100×30)**: select=row 12, text=row 15, int=row 18, decimal=row 20.
- **Back/Forward**: Ctrl+CursorLeft = Back, Ctrl+CursorRight = Forward (BrowseBar shortcuts).
- **Quit**: Esc or q both close the viewer.
- **Scrolling**: CursorDown/CursorUp for line-by-line, PageDown/PageUp for page-at-a-time.
- **Terminal size**: 100×30 gives comfortable width for help tables without horizontal overflow.
- **`--no-browse` is NOT used**: Browser mode (the default) enables back/forward navigation.

## Troubleshooting

1. **Click not navigating**: Verify click coordinates haven't shifted. Re-check by recording a single `click:3:12` and inspecting the cast output for navigation confirmation.
2. **Back button not working**: Ctrl+CursorLeft must be sent as `Ctrl+CursorLeft` (not `Ctrl+Left`). The escape sequence is `\x1b[1;5D`.
3. **Content not scrolling**: The Markdown view must have focus. After a click navigates to a topic, focus should remain on the content view.
4. **Recording too long**: Reduce `wait:` values. The demo should be ~10s at keystroke-delay 50.
5. **GIF too large**: At 100×30, expect ~0.3-0.5 MB. Reduce `--cols` to 80 for a smaller file.
6. **Viewport starts at bottom**: Known TG bug (gui-cs/Terminal.Gui#5365). The workaround in HelpClet resets viewport on initial render.
