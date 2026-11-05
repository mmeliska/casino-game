#!/bin/zsh
# Compiles the four Starflare assemblies with Roslyn against the Unity install's own DLLs,
# without opening Unity. It can't run the tests (that needs the editor, closed, headless; see
# CLAUDE.md), but it catches every compile error while the editor has the project open.
# Needs the dotnet SDK (for csc) and a Library/ folder from having opened the project once.
set -u
cd "$(dirname "$0")/../game" || exit 1

EDITOR_VERSION=$(sed -n 's/^m_EditorVersion: //p' ProjectSettings/ProjectVersion.txt)
U="/Applications/Unity/Hub/Editor/$EDITOR_VERSION/Unity.app/Contents/Resources/Scripting"
CSC=$(find "$(dirname "$(readlink -f "$(which dotnet)")")/sdk" -name csc.dll -path "*Roslyn/bincore*" | sort | tail -1)
NUNIT=$(find Library/PackageCache -name nunit.framework.dll -path "*unity-custom*" | head -1)
SA=Library/ScriptAssemblies
# A package the editor has not installed yet can be pointed at from a local build of it.
INPUTSYSTEM_DLL=${INPUTSYSTEM_DLL:-$SA/Unity.InputSystem.dll}
OUT=$(mktemp -d)
trap 'rm -rf "$OUT"' EXIT

for need in "$U" "$CSC" "$NUNIT" "$SA/UnityEngine.UI.dll"; do
  [ -e "$need" ] || { echo "missing: $need"; exit 1; }
done

NS=(); for f in "$U"/NetStandard/ref/2.1.0/*.dll; do NS+=("-r:$f"); done
FW=(); for f in "$U"/MonoBleedingEdge/lib/mono/4.7.1-api/*.dll "$U"/MonoBleedingEdge/lib/mono/4.7.1-api/Facades/*.dll; do FW+=("-r:$f"); done
ENG=(); for f in "$U"/Managed/UnityEngine/UnityEngine.*.dll; do ENG+=("-r:$f"); done
EDI=(); for f in "$U"/Managed/UnityEngine/UnityEditor.*.dll; do EDI+=("-r:$f"); done
COMMON=(-nologo -nostdlib -noconfig -langversion:9.0 -target:library -warn:0)

failed=0
step() {
  local name=$1; shift
  if dotnet "$CSC" "${COMMON[@]}" "$@"; then echo "ok   $name"; else echo "FAIL $name"; failed=1; fi
}

step Starflare.Core -out:"$OUT/Starflare.Core.dll" "${NS[@]}" Assets/Scripts/Core/**/*.cs
step Starflare.Game -out:"$OUT/Starflare.Game.dll" "${NS[@]}" "${ENG[@]}" -r:"$OUT/Starflare.Core.dll" \
  -r:$SA/UnityEngine.UI.dll -r:$SA/Ink.dll -r:$SA/Ink-Libraries.dll -r:$SA/Unity.TextMeshPro.dll -r:"$INPUTSYSTEM_DLL" \
  Assets/Scripts/Game/**/*.cs
step Starflare.Editor -out:"$OUT/Starflare.Editor.dll" -define:UNITY_EDITOR "${NS[@]}" "${ENG[@]}" "${EDI[@]}" \
  -r:"$OUT/Starflare.Core.dll" -r:"$OUT/Starflare.Game.dll" \
  -r:$SA/Unity.RenderPipelines.Core.Runtime.dll -r:$SA/Unity.RenderPipelines.Universal.Runtime.dll \
  -r:$SA/Unity.TextMeshPro.dll Assets/Scripts/Editor/**/*.cs
step Starflare.Tests.EditMode -out:"$OUT/Starflare.Tests.EditMode.dll" -define:UNITY_EDITOR -define:UNITY_INCLUDE_TESTS \
  "${FW[@]}" "${ENG[@]}" "${EDI[@]}" -r:"$OUT/Starflare.Core.dll" -r:"$OUT/Starflare.Game.dll" \
  -r:"$NUNIT" -r:$SA/Ink-Libraries.dll -r:$SA/UnityEngine.TestRunner.dll -r:$SA/UnityEditor.TestRunner.dll \
  Assets/Tests/EditMode/**/*.cs
step Starflare.Tests.PlayMode -out:"$OUT/Starflare.Tests.PlayMode.dll" -define:UNITY_INCLUDE_TESTS \
  "${FW[@]}" "${ENG[@]}" -r:"$OUT/Starflare.Core.dll" -r:"$OUT/Starflare.Game.dll" \
  -r:"$NUNIT" -r:$SA/Ink-Libraries.dll -r:$SA/UnityEngine.TestRunner.dll -r:$SA/UnityEngine.UI.dll \
  Assets/Tests/PlayMode/**/*.cs
exit $failed
