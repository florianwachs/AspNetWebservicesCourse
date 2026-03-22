#!/usr/bin/env bash
set -euo pipefail

export PATH="$HOME/.dotnet/tools:$PATH"

tool_is_installed() {
  local package_id
  package_id="$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"

  dotnet tool list --global \
    | awk 'NR > 2 { print tolower($1) }' \
    | grep -Fxq "$package_id"
}

ensure_dotnet_tool() {
  local package_id="$1"
  local display_name="$2"

  if tool_is_installed "$package_id"; then
    echo "Updating ${display_name}..."
    dotnet tool update --global "$package_id"
  else
    echo "Installing ${display_name}..."
    dotnet tool install --global "$package_id"
  fi
}

ensure_dotnet_tool "dotnet-ef" "dotnet-ef"
ensure_dotnet_tool "Aspire.Cli" "Aspire CLI"

echo
echo "Installed tool versions:"
dotnet --version
docker --version
dotnet ef --version
aspire --version
