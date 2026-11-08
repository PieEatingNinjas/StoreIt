#!/usr/bin/env bash
# Valideert release-labels (android:<versie>, ios:<versie>) en bepaalt versie, publicatiemodus en release notes.
# Gedeeld door release.yml en release-check.yml.
#
# Env:   LABELS (JSON array van labelnamen), INPUT_ANDROID / INPUT_IOS (optioneel, workflow_dispatch),
#        REQUIRE_RELEASE (true = fout als er geen platform geselecteerd is), DEFAULT_NOTES
# Output: notes/<platform>.txt en (indien GITHUB_OUTPUT gezet) <platform>-version, <platform>-auto, sha
set -euo pipefail

mkdir -p notes
OUT="${GITHUB_OUTPUT:-/dev/null}"
ERRORS=0
SELECTED=0

fail() {
  echo "::error::$1"
  ERRORS=$((ERRORS+1))
}

for P in android ios; do
  if [ "$P" = android ]; then INPUT="${INPUT_ANDROID:-}"; else INPUT="${INPUT_IOS:-}"; fi

  VERSIONS=$(echo "$LABELS" | jq -r --arg p "$P:" '.[] | select(startswith($p)) | ltrimstr($p)')
  COUNT=$(printf '%s' "$VERSIONS" | grep -c . || true)

  if [ -n "$INPUT" ]; then
    VERSION="$INPUT"
  elif [ "$COUNT" -gt 1 ]; then
    fail "Meer dan één $P-label gevonden ($(echo $VERSIONS | tr ' ' ',')); gebruik er maximaal één"
    continue
  else
    VERSION="$VERSIONS"
  fi
  [ -z "$VERSION" ] && continue

  if ! [[ "$VERSION" =~ ^[0-9]+\.[0-9]+\.[0-9]+(-beta\.[0-9]+)?$ ]]; then
    fail "Ongeldige versie voor $P: '$VERSION' (verwacht x.y.z of x.y.z-beta.n)"
    continue
  fi
  if git tag --list "$P-v$VERSION" | grep -q .; then
    fail "Tag $P-v$VERSION bestaat al; die versie is al uitgebracht"
    continue
  fi

  # De stores kennen enkel x.y.z; het beta-suffix zit alleen in tag en notes
  STORE_VERSION="${VERSION%%-*}"
  BETA=false
  [ "$STORE_VERSION" != "$VERSION" ] && BETA=true

  # Alleen patch releases (z > 0) worden automatisch gepubliceerd; betas gaan enkel naar testtracks
  if [ "$BETA" = false ] && [ "${STORE_VERSION##*.}" != "0" ]; then AUTO=true; else AUTO=false; fi

  if [ -s "releasenotes/$P/$VERSION.md" ]; then
    cp "releasenotes/$P/$VERSION.md" "notes/$P.txt"
  elif [ -s "releasenotes/$VERSION.md" ]; then
    cp "releasenotes/$VERSION.md" "notes/$P.txt"
  elif [ "$BETA" = true ]; then
    printf '%s\n' "Testversie" > "notes/$P.txt"
  elif [ "$AUTO" = true ]; then
    printf '%s\n' "$DEFAULT_NOTES" > "notes/$P.txt"
  else
    fail "$P $VERSION vereist releasenotes: releasenotes/$P/$VERSION.md of releasenotes/$VERSION.md"
    continue
  fi

  # Play Store limiet
  if [ "$P" = android ] && [ "$(wc -c < notes/android.txt)" -gt 500 ]; then
    fail "Release notes voor android $VERSION zijn langer dan 500 tekens (Google Play limiet)"
    continue
  fi

  echo "$P-version=$VERSION" >> "$OUT"
  echo "$P-store-version=$STORE_VERSION" >> "$OUT"
  echo "$P-beta=$BETA" >> "$OUT"
  echo "$P-auto=$AUTO" >> "$OUT"
  echo "📋 $P $VERSION auto-publish=$AUTO beta=$BETA"
  SELECTED=$((SELECTED+1))
done

if [ "$ERRORS" -gt 0 ]; then
  exit 1
fi
if [ "$SELECTED" -eq 0 ]; then
  if [ "${REQUIRE_RELEASE:-false}" = true ]; then
    echo "::error::Geen platform geselecteerd"
    exit 1
  fi
  echo "Geen release-labels; niets te controleren."
fi
echo "sha=$(git rev-parse HEAD)" >> "$OUT"
