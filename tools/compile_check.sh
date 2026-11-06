#!/bin/zsh
# Compiles every Starflare assembly with Roslyn while the editor has the project open.
# See compile_check.py; it reads the .asmdef files, so new modules need no change here.
exec python3 -I "$(dirname "$0")/compile_check.py" "$@"
