import datetime
import argparse
from pathlib import Path

parser = argparse.ArgumentParser()
parser.add_argument("-c", "--changelog", help="Changelog file", default="CHANGELOG.md")
parser.add_argument("-v", "--version", help="Release version", required=True)
parser.add_argument("--extract", help="Extract changes from changelog", required=False, default=None)

args = parser.parse_args()

changelog_path = Path(args.changelog)
if not changelog_path.exists():
    raise Exception("Changelog file does not exist")
version = args.version


with changelog_path.open('r+') as file_handle:
    lines = file_handle.readlines()
    for index, line in enumerate(lines):
        if line.startswith('## '):
            starindex = line.index('[')
            endindex = line.index(']')
            substr = line[starindex + 1:endindex]
            if substr.lower() == 'unreleased' or substr == version:
                today = datetime.date.today()
                lines[index] = f"## [{version}] - {today.year}-{today.month:02d}-{today.day}\n"
                break
    else:
        raise Exception(f"Failed to find changes with either Unreleased or {version} tag.")
    file_handle.seek(0)
    file_handle.writelines(lines)
    file_handle.truncate()

if args.extract is not None:
    with Path(args.extract).open('w') as file_handle:
        for line in lines[index+1:]:
            if line.startswith('## '): # next changelog record
                break
            file_handle.write(line)