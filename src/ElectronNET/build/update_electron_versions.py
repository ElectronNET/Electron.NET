#!/usr/bin/env python3
"""
Update Electron version list in ElectronNETRules.Project.xaml

This script:
1. Downloads the Electron releases feed from GitHub
2. Filters for stable releases >= 23.0.0
3. Generates the ElectronVersion enum XML
4. Updates the XAML file with the new version list
"""

import json
import re
import urllib.request
from pathlib import Path


def download_releases():
    """Download Electron releases JSON from GitHub."""
    url = "https://releases.electronjs.org/releases.json"
    print(f"Downloading releases from {url}...")
    
    with urllib.request.urlopen(url) as response:
        data = response.read()
    
    print(f"Downloaded {len(data)} bytes")
    return json.loads(data)


def filter_versions(releases, min_version="23.0.0"):
    """Filter and sort stable Electron versions."""
    print(f"Filtering versions >= {min_version}...")
    
    # Regular expression for stable versions (major.minor.patch)
    stable_pattern = re.compile(r'^\d+\.\d+\.\d+$')
    
    # Parse minimum version
    min_parts = tuple(map(int, min_version.split('.')))
    
    # Filter and collect versions
    versions = set()
    for release in releases:
        version = release.get('version', '')
        if stable_pattern.match(version):
            parts = tuple(map(int, version.split('.')))
            if parts >= min_parts:
                versions.add(version)
    
    # Sort versions
    sorted_versions = sorted(versions, key=lambda v: tuple(map(int, v.split('.'))))
    
    print(f"Found {len(sorted_versions)} stable versions")
    return sorted_versions


def generate_enum_xml(versions):
    """Generate the ElectronVersion enum property XML."""
    lines = [
        '  <EnumProperty Name="ElectronVersion"',
        '                DisplayName="Electron Version"',
        '                Description="The version of Electron to use for building (full semver)"',
        '                Category="General">',
        '    <!-- Auto-generated list of stable Electron releases -->',
    ]
    
    for version in versions:
        lines.append(f'    <EnumValue Name="{version}" DisplayName="{version}" />')
    
    lines.append('  </EnumProperty>')
    
    return '\n'.join(lines)


def update_xaml_file(xaml_path, enum_xml):
    """Update the XAML file with the new enum."""
    print(f"Updating {xaml_path}...")
    
    # Read the original file
    content = Path(xaml_path).read_text(encoding='utf-8')
    
    # Find the ElectronVersion enum markers
    start_marker = '  <EnumProperty Name="ElectronVersion"'
    end_marker = '  </EnumProperty>'
    
    start_idx = content.find(start_marker)
    if start_idx == -1:
        raise ValueError("Could not find ElectronVersion EnumProperty start marker")
    
    end_idx = content.find(end_marker, start_idx)
    if end_idx == -1:
        raise ValueError("Could not find ElectronVersion EnumProperty end marker")
    
    end_idx += len(end_marker)
    
    # Replace the enum section
    new_content = content[:start_idx] + enum_xml + '\n' + content[end_idx:]
    
    # Write the updated file
    Path(xaml_path).write_text(new_content, encoding='utf-8')
    
    print("Successfully updated XAML file")


def main():
    """Main entry point."""
    try:
        # Paths
        xaml_file = "build/ElectronNETRules.Project.xaml"
        
        # Download and process releases
        releases = download_releases()
        versions = filter_versions(releases, min_version="23.0.0")
        
        # Generate enum XML
        enum_xml = generate_enum_xml(versions)
        
        # Update XAML file
        update_xaml_file(xaml_file, enum_xml)
        
        print(f"\n✓ Successfully updated {xaml_file} with {len(versions)} Electron versions")
        print(f"  Version range: {versions[0]} to {versions[-1]}")
        
    except Exception as e:
        print(f"\n✗ Error: {e}")
        return 1
    
    return 0


if __name__ == "__main__":
    exit(main())
