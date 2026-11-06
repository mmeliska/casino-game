"""Compiles every Starflare assembly with Roslyn against the Unity install's own DLLs, without
opening Unity. It reads the .asmdef files, so a new module needs no change here. Each assembly
gets only the references its .asmdef names, as Unity gives it, so a missing reference fails here
too. It can't run the tests (that needs the editor closed, headless; see CLAUDE.md), but it
catches every compile error while the editor has the project open.

Needs the dotnet SDK (for csc) and a Library/ folder from having opened the project once.
Run through tools/compile_check.sh.
"""
import glob
import json
import os
import shutil
import subprocess
import sys
import tempfile

GAME = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "game")
os.chdir(GAME)

version = next(l.split(": ", 1)[1].strip() for l in open("ProjectSettings/ProjectVersion.txt")
               if l.startswith("m_EditorVersion: "))
U = f"/Applications/Unity/Hub/Editor/{version}/Unity.app/Contents/Resources/Scripting"
dotnet = shutil.which("dotnet")
sdk = os.path.join(os.path.dirname(os.path.realpath(dotnet)), "sdk") if dotnet else ""
CSC = sorted(glob.glob(f"{sdk}/*/Roslyn/bincore/csc.dll"))[-1:] or [""]
CSC = CSC[0]
NUNIT = next(iter(glob.glob("Library/PackageCache/**/nunit.framework.dll", recursive=True)), "")
NUNIT = next((p for p in glob.glob("Library/PackageCache/**/nunit.framework.dll", recursive=True)
              if "unity-custom" in p), NUNIT)
SA = "Library/ScriptAssemblies"
# A package the editor has not installed yet can be pointed at from a local build of it.
PACKAGE_DLL = {"Unity.InputSystem": os.environ.get("INPUTSYSTEM_DLL", f"{SA}/Unity.InputSystem.dll")}

for need in (U, CSC, NUNIT, f"{SA}/UnityEngine.UI.dll"):
    if not need or not os.path.exists(need):
        sys.exit(f"missing: {need or 'csc.dll / nunit.framework.dll'}")

NS = glob.glob(f"{U}/NetStandard/ref/2.1.0/*.dll")
FW = glob.glob(f"{U}/MonoBleedingEdge/lib/mono/4.7.1-api/*.dll") + \
     glob.glob(f"{U}/MonoBleedingEdge/lib/mono/4.7.1-api/Facades/*.dll")
ENG = glob.glob(f"{U}/Managed/UnityEngine/UnityEngine.*.dll")
EDI = glob.glob(f"{U}/Managed/UnityEngine/UnityEditor.*.dll")

# Every assembly under Assets, and the scripts each owns (not those under a nested .asmdef).
asms = {}
for path in sorted(glob.glob("Assets/**/*.asmdef", recursive=True)):
    d = json.load(open(path))
    d["dir"] = os.path.dirname(path)
    asms[d["name"]] = d
dirs = {a["dir"]: n for n, a in asms.items()}
for path in glob.glob("Assets/**/*.cs", recursive=True):
    d = os.path.dirname(path)
    while d not in dirs and d not in ("", "Assets"):
        d = os.path.dirname(d)
    if d in dirs:
        asms[dirs[d]].setdefault("sources", []).append(path)

order, seen = [], set()
def visit(name, stack=()):
    if name in seen or name not in asms:
        return
    if name in stack:
        sys.exit(f"reference cycle: {' -> '.join(stack + (name,))}")
    for r in asms[name]["references"]:
        visit(r, stack + (name,))
    seen.add(name)
    order.append(name)
for name in asms:
    visit(name)

out = tempfile.mkdtemp()
failed = False
for name in order:
    a = asms[name]
    editor = a.get("includePlatforms") == ["Editor"]
    args = ["-nologo", "-nostdlib", "-noconfig", "-langversion:9.0", "-target:library", "-warn:0",
            f"-out:{out}/{name}.dll"]
    if editor:
        args.append("-define:UNITY_EDITOR")
    args += [f"-define:{c}" for c in a.get("defineConstraints", [])]
    refs = FW if a.get("precompiledReferences") else NS
    if not a.get("noEngineReferences"):
        refs = refs + ENG + (EDI if editor else [])
    for r in a["references"]:
        refs = refs + [f"{out}/{r}.dll" if r in asms else PACKAGE_DLL.get(r, f"{SA}/{r}.dll")]
    refs += [NUNIT for p in a.get("precompiledReferences", []) if p == "nunit.framework.dll"]
    args += [f"-r:{r}" for r in refs]
    if not a.get("sources"):
        print(f"skip {name} (no scripts)")
        continue
    ok = subprocess.run(["dotnet", CSC, *args, *sorted(a["sources"])]).returncode == 0
    print(f"{'ok  ' if ok else 'FAIL'} {name}")
    failed |= not ok
shutil.rmtree(out)
sys.exit(1 if failed else 0)
