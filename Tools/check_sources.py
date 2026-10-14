#!/usr/bin/env python3
"""Static consistency checks for the Endless Rails C# source."""
import os
import re
import sys

ROOT = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), "Assets")

files = []
for dirpath, _, filenames in os.walk(ROOT):
    for name in filenames:
        if name.endswith(".cs"):
            files.append(os.path.join(dirpath, name))

all_sources = {}
for path in files:
    with open(path) as f:
        all_sources[path] = f.read()

errors = []
warnings = []

# --- 1. Brace / paren balance ---
for path, src in all_sources.items():
    # Strip strings and comments crudely
    stripped = re.sub(r'"(?:\\.|[^"\\])*"', '""', src)
    stripped = re.sub(r"//[^\n]*", "", stripped)
    stripped = re.sub(r"/\*.*?\*/", "", stripped, flags=re.S)
    for open_ch, close_ch, label in (("{", "}", "braces"), ("(", ")", "parens"), ("[", "]", "brackets")):
        if stripped.count(open_ch) != stripped.count(close_ch):
            errors.append(f"{os.path.relpath(path, ROOT)}: unbalanced {label} "
                          f"({stripped.count(open_ch)} vs {stripped.count(close_ch)})")

# --- 2. GameEvents.RaiseX calls must exist ---
events_src = all_sources.get(os.path.join(ROOT, "Scripts", "Core", "GameEvents.cs"), "")
defined_raises = set(re.findall(r"public static void (Raise\w+)\s*\(", events_src))
for path, src in all_sources.items():
    for call in re.findall(r"GameEvents\.(Raise\w+)\s*\(", src):
        if call not in defined_raises:
            errors.append(f"{os.path.relpath(path, ROOT)}: GameEvents.{call} is not defined")

# --- 3. AudioManager Play* calls must exist ---
audio_src = all_sources.get(os.path.join(ROOT, "Scripts", "Audio", "AudioManager.cs"), "")
defined_plays = set(re.findall(r"public void (Play\w+)\s*\(", audio_src))
defined_plays.add("PlayClipAt")
for path, src in all_sources.items():
    if path.endswith("AudioManager.cs"):
        continue
    for call in re.findall(r"AudioManager\.Instance\??\.(Play\w+)\s*\(", src):
        if call not in defined_plays:
            errors.append(f"{os.path.relpath(path, ROOT)}: AudioManager.{call} is not defined")
    for call in re.findall(r"AudioManager\.Instance\.Play(\w+)\s*\(", src):
        pass

# --- 4. Serialized field names referenced by editor scripts ---
def class_body(src, class_name):
    m = re.search(rf"class {class_name}\b[^{{]*\{{", src)
    if not m:
        return None
    depth = 0
    start = m.end() - 1
    for i in range(start, len(src)):
        if src[i] == "{":
            depth += 1
        elif src[i] == "}":
            depth -= 1
            if depth == 0:
                return src[start:i + 1]
    return None

editor_files = {p: s for p, s in all_sources.items() if "/Editor/" in p.replace(os.sep, "/")}
runtime_by_class = {}
for path, src in all_sources.items():
    if "/Editor/" in path.replace(os.sep, "/"):
        continue
    for cls in re.findall(r"class (\w+)", src):
        runtime_by_class[cls] = src

# FindProperty("field") usages paired with nearby component types is complex;
# instead verify each FindProperty name exists as a field SOMEWHERE in runtime code.
all_field_names = set()
for path, src in all_sources.items():
    if "/Editor/" in path.replace(os.sep, "/"):
        continue
    all_field_names.update(re.findall(r"\[SerializeField\]\s+(?:private\s+|protected\s+|internal\s+)?[\w<>\[\],. ]+\s+(\w+)\s*[;=]", src))
    all_field_names.update(re.findall(r"\[SerializeField\]\s+(?:private\s+|protected\s+)?\s*[\w<>\[\],. ]+\s+(\w+)\s*=", src))

for path, src in editor_files.items():
    for field in re.findall(r'FindProperty\("(\w+)"\)', src):
        if field not in all_field_names:
            # may be a local serializable class field like 'cell'/'prefab'/'name'/'panel'
            if field in {"cell", "prefab", "name", "panel", "cell"}:
                continue
            warnings.append(f"{os.path.relpath(path, ROOT)}: FindProperty(\"{field}\") - no matching [SerializeField] found")

# --- 5. Namespace consistency ---
for path, src in all_sources.items():
    if "/Editor/" in path.replace(os.sep, "/"):
        continue
    if "namespace EndlessRails" not in src:
        warnings.append(f"{os.path.relpath(path, ROOT)}: missing EndlessRails namespace")

# --- 6. CompareTag strings must be built-in or ensured by ProjectSetup.EnsureTags ---
tagmanager_path = os.path.join(os.path.dirname(ROOT), "ProjectSettings", "TagManager.asset")
tagmanager = ""
if os.path.exists(tagmanager_path):
    with open(tagmanager_path) as f:
        tagmanager = f.read()

# Tags registered programmatically in ProjectSetup.EnsureTags.
setup_src = all_sources.get(os.path.join(ROOT, "Editor", "ProjectSetup.cs"), "")
ensured_tags = set()
m = re.search(r'public static void EnsureTags\(\)\s*\{(.*?)\n        \}', setup_src, re.S)
if m:
    ensured_tags = set(re.findall(r'"(\w+)"', m.group(1)))

# Built-in Unity tags that are always defined implicitly.
BUILTIN_TAGS = {"Untagged", "Respawn", "Finish", "EditorOnly", "MainCamera", "Player", "GameController"}

for path, src in all_sources.items():
    for tag in re.findall(r'CompareTag\("(\w+)"\)', src):
        if tag in BUILTIN_TAGS or tag in ensured_tags:
            continue
        if f"- {tag}" not in tagmanager:
            errors.append(f"{os.path.relpath(path, ROOT)}: tag '{tag}' is neither built-in, nor in TagManager.asset, nor in EnsureTags()")

print(f"Checked {len(files)} C# files.")
for e in errors:
    print("ERROR:", e)
for w in warnings:
    print("WARN :", w)
if errors:
    sys.exit(1)
print("All consistency checks passed." if not warnings else f"Done with {len(warnings)} warnings.")
