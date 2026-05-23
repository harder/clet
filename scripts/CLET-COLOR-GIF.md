# Color Picker GIF Recording Guide

Produces `docs/images/clet-color.gif` — an animated GIF demonstrating clet's color picker.

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
#   1. Color picker opens with initial green (#00cc00)
#   2. CursorRight×12 on Hue slider (sweep green → purple)
#   3. Tab to Saturation slider, CursorLeft×3 (slight desaturation)
#   4. Enter to accept — prints hex result on command line
#
# The ColorPicker has H/S/V sliders. Focus starts on H (hue).
# Tab moves between sliders. CursorRight/Left adjusts values.
#
# Theme: Anders theme is activated via ~/.tui/clet.config.json before recording.
# This is also used in the README "Q: Theming?" section.
#
# Pacing: --keystroke-delay 70 (smooth slider movement)
$ks = 'wait:1500,CursorRight,CursorRight,CursorRight,CursorRight,CursorRight,CursorRight,CursorRight,CursorRight,CursorRight,CursorRight,CursorRight,CursorRight,wait:500,Tab,CursorLeft,CursorLeft,CursorLeft,wait:500,Enter'

# Temporarily enable Anders theme
$configPath = "$env:USERPROFILE\.tui\clet.config.json"
$backup = Get-Content $configPath -Raw
$themed = $backup -replace '// "Theme": "Anders"', '"Theme": "Anders"'
Set-Content $configPath $themed -Encoding utf8

tuirec record `
    --binary $binary `
    --args "color,-i,#00cc00" `
    --show-command '$ clet color -i "#00cc00"' `
    --keystrokes $ks `
    --startup-delay 2000 `
    --drain 1500 `
    --cols 80 `
    --rows 20 `
    --keystroke-delay 70 `
    --cast-output ./artifacts/clet-color.cast

# Copy to final location
Copy-Item recording.gif ./docs/images/clet-color.gif -Force

# Restore original config
Set-Content $configPath $backup -Encoding utf8
```

## Demo sequence

| Time  | Feature              | Keys / Actions                                              |
|-------|----------------------|-------------------------------------------------------------|
| 0-2s  | Picker loads         | `$ clet color -i "#00cc00"` — picker opens showing green    |
| 2-4s  | Sweep hue            | CursorRight×12 (H slider: green → cyan → blue → purple)    |
| 4-5s  | Adjust saturation    | Tab + CursorLeft×3 (S slider: slight desaturation)          |
| 5-6s  | Accept               | Enter (exits, prints #a300cc on command line)               |

## Tuning tips

- **Initial color**: Use `-i "#00cc00"` (bright green) to start with good S/V values. Starting from black (#000000) means H changes aren't visible.
- **Slider navigation**: Focus starts on H slider. Tab cycles H→S→V. CursorRight/Left adjusts the focused slider.
- **Keystroke-delay 70**: Gives a smooth "sliding" feel as the hue sweeps through the spectrum.
- **Terminal size**: 80×20 is compact enough for the picker while showing the color bar clearly.
- **Args format**: tuirec `--args` uses comma separation: `"color,-i,#00cc00"`.

## Troubleshooting

1. **Output is #000000**: Starting from black means V=0 and S=0 — hue changes won't be visible. Always use `-i` with a saturated color.
2. **Color didn't change**: Verify the slider has focus. If Tab moved past the sliders, the arrows may not affect the color.
3. **No hex output**: Ensure `--drain 1500` captures the post-exit output.
