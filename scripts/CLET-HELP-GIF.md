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
#   1. Overview page loads (shows clet table)
#   2. Scroll down to see more of the overview (PageDown)
#   3. Tab to first link + Enter → navigate to "select" help topic
#   4. Scroll within the select help page (options, examples)
#   5. Ctrl+Left (Back) → return to overview
#   6. Tab×3 + Enter → navigate to "int" help topic
#   7. Scroll within the int help page
#   8. Ctrl+Left (Back) → return to overview
#   9. Esc → quit
#
# Navigation: Tab cycles through hyperlinks in the Markdown view,
# Enter follows the focused link. The "Click for details" line after
# the clet table has one link per clet in registration order:
# select(1), text(2), int(3), decimal(4), confirm(5), ...
#
# Pacing: --keystroke-delay 60 (snappy but readable)
$ks = 'wait:1500,PageDown,wait:600,Tab,wait:200,Enter,wait:800,PageDown,wait:400,PageDown,wait:600,Ctrl+CursorLeft,wait:800,Tab,wait:100,Tab,wait:100,Tab,wait:200,Enter,wait:800,PageDown,wait:600,Ctrl+CursorLeft,wait:800,Esc'

tuirec record `
    --binary $binary `
    --args "help" `
    --name "clet-help" `
    --show-command '$ clet help' `
    --keystrokes $ks `
    --startup-delay 2000 `
    --drain 1500 `
    --cols 100 `
    --rows 30 `
    --keystroke-delay 60 `
    --max-duration 45 `
    --cast-output ./artifacts/clet-help.cast `
    --verbosity high

# Copy to final location
Copy-Item ./artifacts/clet-help.gif ./docs/images/clet-help.gif -Force
```

## Demo sequence

| Time   | Feature                | Keys / Actions                                                    |
|--------|------------------------|-------------------------------------------------------------------|
| 0-2s   | Overview loads         | `$ clet help` typed; overview page renders with clet table        |
| 2-4s   | Scroll overview        | PageDown (show more of the clet table)                            |
| 4-6s   | Navigate to "select"   | Tab (focus first link) + Enter (follow it → select help)          |
| 6-8s   | Scroll select help     | PageDown×2 (show options table and examples)                      |
| 8-9s   | Back to overview       | Ctrl+Left (Back button fires, overview re-renders)                |
| 9-10s  | Navigate to "int"      | Tab×3 + Enter (3rd link = int → int help)                         |
| 10-11s | Scroll int help        | PageDown (show options table)                                     |
| 11-12s | Back to overview       | Ctrl+Left (Back button fires again)                               |
| 12-13s | Quit                   | Esc (exits the help browser)                                      |

## Tuning tips

- **Link navigation**: Tab cycles through hyperlinks in the Markdown view. Enter follows the focused link. The "Click for details" line after the clet table lists all clets as links in registration order: select(1st Tab), text(2nd), int(3rd), decimal(4th), confirm(5th), etc.
- **Mouse clicks on links are unreliable**: Terminal.Gui's Markdown view uses OSC 8 hyperlinks, but mouse click detection depends on exact mouse protocol negotiation. Use Tab+Enter for deterministic link following.
- **Back/Forward**: Ctrl+CursorLeft = Back, Ctrl+CursorRight = Forward. These are the BrowseBar shortcuts.
- **Quit**: Esc or q both close the viewer. Use Esc for clarity.
- **Scrollbar**: The Markdown view has a horizontal scrollbar. Vertical scrolling uses PageDown/PageUp/CursorDown/CursorUp.
- **Terminal size**: 100×30 gives comfortable width for the help tables without horizontal overflow.
- **`--no-browse` is NOT used**: We want browser mode (the default) so back/forward navigation is available.
- **Tab count after Back**: After pressing Ctrl+Left (Back) to return to the overview, Tab counts reset — Tab 1 still goes to "select", Tab 2 to "text", etc.

## Troubleshooting

1. **Tab not reaching links**: The first Tab should focus the first hyperlink in the Markdown view. If it goes to the status bar instead, the focus order may have changed — try Shift+Tab or additional Tab presses.
2. **Back button not working**: Ctrl+CursorLeft must be sent as `Ctrl+CursorLeft` (not `Ctrl+Left`). Verify with `--verbosity high` that the correct escape sequence `\x1b[1;5D` is generated.
3. **Content not scrolling**: The Markdown view must have focus. If Tab moved focus to a link or status bar, PageDown may not scroll. Click on the content area or adjust keystroke order.
4. **Recording too long**: Reduce `wait:` values. The demo should be ~13s at keystroke-delay 60.
5. **GIF too large**: At 100×30, expect ~0.3-0.5 MB. Reduce `--cols` to 80 for a smaller file.
6. **Wrong navigation target**: Count Tabs carefully. Registration order is: select, text, int, decimal, confirm, date, time, duration, color, multi-select, attribute-picker, pick-file, pick-directory, linear-range, edit, md, config, help.
