# Pick-File GIF Recording Guide

Produces `docs/images/clet-pick-file.gif` — an animated GIF demonstrating clet's file picker.

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
#   1. File picker loads (shows directory listing in table)
#   2. Tab×3 to move focus to the Find/filter field
#   3. Type "rea" (with 250ms spacing) to filter to README files
#   4. Shift+Tab to move focus back up to the table
#   5. "r" to jump-select the first README.md entry
#   6. Enter to accept and show the result on the command line
#
# Pacing: --keystroke-delay 80; wait: values add pauses for readability
$ks = 'wait:1500,Tab,Tab,Tab,wait:300,`r`,wait:250,`e`,wait:250,`a`,wait:600,Shift+Tab,wait:400,`r`,wait:400,Enter'

# Use 80×20 for a compact look (closer to inline feel).
# True inline rendering requires tuirec support — see gui-cs/tuirec#49.
tuirec record `
    --binary $binary `
    --args "pick-file" `
    --show-command '$ clet pick-file' `
    --keystrokes $ks `
    --startup-delay 2000 `
    --drain 1500 `
    --cols 80 `
    --rows 20 `
    --keystroke-delay 80 `
    --cast-output ./artifacts/clet-pick-file.cast

# Copy to final location
Copy-Item recording.gif ./docs/images/clet-pick-file.gif -Force
```

## Demo sequence

| Time  | Feature              | Keys / Actions                                           |
|-------|----------------------|----------------------------------------------------------|
| 0-2s  | File picker loads    | `$ clet pick-file` typed; directory listing renders      |
| 2-3s  | Focus to Find field  | Tab×3 (move focus to the filter/find text field)         |
| 3-4s  | Type filter          | "rea" typed with 250ms spacing (filters to README files) |
| 4-5s  | Focus back to table  | Shift+Tab (return focus to file table)                   |
| 5-6s  | Jump-select README   | "r" key (jump to first match starting with 'r')         |
| 6-7s  | Accept selection     | Enter (exits picker, prints result to command line)      |

## Tuning tips

- **Tab order**: The pick-file view has multiple focusable areas. Tab×3 reaches the filter field from the initial table focus.
- **Filter field**: Typing in the filter field narrows the file list in real-time.
- **Jump-select**: In the table, typing a character jumps to the first entry starting with that character.
- **Terminal size**: 80×20 for a compact look that's closer to inline rendering. True inline support depends on gui-cs/tuirec#49.
- **Drain**: Use `--drain 1500` to capture the command-line output after the app exits.

## Troubleshooting

1. **Tab count wrong**: The number of Tabs to reach the filter field depends on the view layout. Adjust if the picker UI changes.
2. **Filter not matching**: The filter is case-insensitive substring match. "rea" should match README.md.
3. **No output after Enter**: Ensure `--drain 1500` gives enough time for the result to print after the process exits.
4. **Wrong file selected**: The "r" jump-select picks the first visible entry starting with 'r'. If the filter narrowed correctly, this should be README.md.
