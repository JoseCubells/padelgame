#!/bin/bash
# SessionStart hook for Claude Code on the web (padelgame).
# Installs the toolchain needed to build and test the engine-free core outside Unity
# (docs/production/ENVIRONMENT_AUDIT.md, ADR-011): .NET 8 SDK and Git LFS, then restores NuGet packages.
# Unity itself cannot run in the cloud container (KI-001).
set -euo pipefail

if [ "${CLAUDE_CODE_REMOTE:-}" != "true" ]; then
  exit 0
fi

cd "${CLAUDE_PROJECT_DIR:-$(pwd)}"

need_apt=()
command -v dotnet >/dev/null 2>&1 || need_apt+=(dotnet-sdk-8.0)
command -v git-lfs >/dev/null 2>&1 || need_apt+=(git-lfs)

if [ ${#need_apt[@]} -gt 0 ]; then
  # The package index in the image can be stale (404 on old .deb versions), so refresh it first.
  apt-get update -q >/dev/null 2>&1 || true
  DEBIAN_FRONTEND=noninteractive apt-get install -y -q "${need_apt[@]}" >/dev/null
fi

git lfs install --local >/dev/null

# Warm the NuGet cache so `dotnet test` works immediately.
dotnet restore tools/CoreTests/CoreTests.sln --verbosity quiet

echo "session-start: $(dotnet --version) / $(git lfs version | cut -d' ' -f1)"
