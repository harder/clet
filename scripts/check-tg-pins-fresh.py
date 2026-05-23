#!/usr/bin/env python3
"""Ensure Terminal.Gui package pins are not older than NuGet stable releases."""

from __future__ import annotations

import argparse
import json
import os
import re
import sys
import urllib.error
import urllib.request
import xml.etree.ElementTree as ET
from pathlib import Path


PACKAGES = (
    ("Terminal.Gui", "TerminalGuiVersion", "terminal_gui_version"),
    ("Terminal.Gui.Editor", "TerminalGuiEditorVersion", "terminal_gui_editor_version"),
)

VERSION_PATTERN = re.compile (
    r"^(?P<main>[0-9]+(?:\.[0-9]+){0,3})(?:-(?P<pre>[0-9A-Za-z.-]+))?(?:\+[0-9A-Za-z.-]+)?$"
)


def main () -> int:
    parser = argparse.ArgumentParser (
        description="Fail if Terminal.Gui package versions are older than the latest stable versions on NuGet."
    )
    parser.add_argument (
        "--props",
        default="Directory.Build.props",
        help="MSBuild props file to read when explicit versions are omitted.",
    )
    parser.add_argument ("--terminal-gui-version", help="Resolved Terminal.Gui version.", default=None)
    parser.add_argument ("--terminal-gui-editor-version", help="Resolved Terminal.Gui.Editor version.", default=None)
    args = parser.parse_args ()

    props_path = Path (args.props)
    props = read_props (props_path)

    failed = False
    for package_id, property_name, arg_name in PACKAGES:
        pinned_version = getattr (args, arg_name) or props.get (property_name)
        if not pinned_version:
            report_error (f"{property_name} was not found in {props_path}.")
            failed = True
            continue

        try:
            latest_version = latest_stable_version (package_id)
        except RuntimeError as ex:
            report_error (str (ex))
            failed = True
            continue

        try:
            comparison = compare_versions (pinned_version, latest_version)
        except ValueError as ex:
            report_error (f"{property_name} has an invalid or non-comparable version '{pinned_version}': {ex}")
            failed = True
            continue

        if comparison < 0:
            report_error (
                f"{property_name} is stale: {pinned_version} is older than the latest stable "
                f"{package_id} release {latest_version}. Update {property_name} or pass an explicit release override."
            )
            failed = True
        else:
            print (f"{property_name} OK: {pinned_version} >= latest stable {package_id} {latest_version}")

    return 1 if failed else 0


def read_props (props_path: Path) -> dict[str, str]:
    if not props_path.exists ():
        return {}

    root = ET.parse (props_path).getroot ()
    values: dict[str, str] = {}
    for _, property_name, _ in PACKAGES:
        element = root.find (f".//{property_name}")
        if element is not None and element.text:
            values[property_name] = element.text.strip ()

    return values


def latest_stable_version (package_id: str) -> str:
    package_url_id = package_id.lower ()
    url = f"https://api.nuget.org/v3-flatcontainer/{package_url_id}/index.json"
    try:
        with urllib.request.urlopen (url, timeout=30) as response:
            data = json.load (response)
    except (urllib.error.URLError, TimeoutError) as ex:
        raise RuntimeError (f"Could not query NuGet for {package_id}: {ex}") from ex

    stable_versions = [version for version in data.get ("versions", []) if "-" not in version]
    if not stable_versions:
        raise RuntimeError (f"NuGet returned no stable versions for {package_id}.")

    return max (stable_versions, key=VersionKey)


class VersionKey:
    def __init__ (self, version: str) -> None:
        self.version = version

    def __lt__ (self, other: VersionKey) -> bool:
        return compare_versions (self.version, other.version) < 0


def compare_versions (left: str, right: str) -> int:
    left_main, left_pre = parse_version (left)
    right_main, right_pre = parse_version (right)

    if left_main != right_main:
        return -1 if left_main < right_main else 1

    return compare_prerelease (left_pre, right_pre)


def parse_version (version: str) -> tuple[tuple[int, int, int, int], list[str] | None]:
    if "*" in version:
        raise ValueError ("wildcard versions cannot be freshness-checked")

    match = VERSION_PATTERN.match (version)
    if match is None:
        raise ValueError ("expected a NuGet-style semantic version")

    main_parts = [int (part) for part in match.group ("main").split (".")]
    main_parts.extend ([0] * (4 - len (main_parts)))
    prerelease = match.group ("pre")
    return (main_parts[0], main_parts[1], main_parts[2], main_parts[3]), (
        prerelease.split (".") if prerelease else None
    )


def compare_prerelease (left: list[str] | None, right: list[str] | None) -> int:
    if left is None and right is None:
        return 0
    if left is None:
        return 1
    if right is None:
        return -1

    for left_part, right_part in zip (left, right):
        left_numeric = left_part.isdigit ()
        right_numeric = right_part.isdigit ()

        if left_numeric and right_numeric:
            left_value = int (left_part)
            right_value = int (right_part)
            if left_value != right_value:
                return -1 if left_value < right_value else 1
            continue

        if left_numeric != right_numeric:
            return -1 if left_numeric else 1

        left_text = left_part.lower ()
        right_text = right_part.lower ()
        if left_text != right_text:
            return -1 if left_text < right_text else 1

    if len (left) == len (right):
        return 0

    return -1 if len (left) < len (right) else 1


def report_error (message: str) -> None:
    if os.environ.get ("GITHUB_ACTIONS") == "true":
        print (f"::error::{message}", file=sys.stderr)
    else:
        print (f"error: {message}", file=sys.stderr)


if __name__ == "__main__":
    sys.exit (main ())
