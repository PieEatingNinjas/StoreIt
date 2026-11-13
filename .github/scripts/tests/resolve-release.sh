#!/usr/bin/env bash
set -euo pipefail

SCRIPT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)/resolve-release.sh"
WORK="$(mktemp -d)"
trap 'rm -rf "$WORK"' EXIT
cd "$WORK"
git init -q -b main
git -c user.name=Test -c user.email=test@example.invalid commit -q --allow-empty -m fixture
git switch -q -c feature/test
git -c user.name=Test -c user.email=test@example.invalid commit -q --allow-empty -m feature
SHA="$(git rev-parse HEAD)"

run_case() {
  local name="$1" event="$2" ref="$3" android="$4" ios="$5" expected="$6"
  local status=0
  : > outputs
  LABELS='[]' INPUT_ANDROID="$android" INPUT_IOS="$ios" \
    GITHUB_EVENT_NAME="$event" GITHUB_REF="$ref" GITHUB_OUTPUT="$WORK/outputs" \
    REQUIRE_RELEASE=true DEFAULT_NOTES=Bugfixes \
    bash "$SCRIPT" > log 2>&1 || status=$?
  if [ "$status" -ne "$expected" ]; then
    echo "FAIL: $name (exit $status, expected $expected)"
    cat log
    exit 1
  fi
  if [ "$expected" -eq 0 ]; then
    grep -qx "sha=$SHA" outputs
  else
    grep -q '::error::' log
    if grep -q '^sha=' outputs; then
      echo "FAIL: $name returned a build SHA despite failing"
      exit 1
    fi
  fi
  echo "PASS: $name"
}

run_case "feature Android beta" workflow_dispatch refs/heads/feature/test 1.2.0-beta.1 '' 0
grep -qx 'android-beta=true' outputs
grep -qx 'android-auto=false' outputs
run_case "feature iOS beta" workflow_dispatch refs/heads/feature/test '' 1.2.0-beta.2 0
grep -qx 'ios-beta=true' outputs
grep -qx 'ios-auto=false' outputs
run_case "feature both betas" workflow_dispatch refs/heads/feature/test 1.2.0-beta.3 1.2.0-beta.3 0
run_case "feature Android stable" workflow_dispatch refs/heads/feature/test 1.2.1 '' 1
run_case "feature iOS stable" workflow_dispatch refs/heads/feature/test '' 1.2.1 1
run_case "feature mixed beta/stable" workflow_dispatch refs/heads/feature/test 1.2.0-beta.4 1.2.1 1
run_case "feature mixed stable/beta" workflow_dispatch refs/heads/feature/test 1.2.1 1.2.0-beta.4 1
run_case "feature malformed beta" workflow_dispatch refs/heads/feature/test 1.2.0-beta '' 1
run_case "main stable" workflow_dispatch refs/heads/main 1.2.1 1.2.1 0
run_case "main beta" workflow_dispatch refs/heads/main 1.2.0-beta.5 '' 0
run_case "PR validation unchanged" pull_request refs/pull/1/merge 1.2.1 '' 0
run_case "merged PR unchanged" pull_request_target refs/heads/main 1.2.1 '' 0
run_case "tag beta rejected" workflow_dispatch refs/tags/test 1.2.0-beta.6 '' 1
run_case "no platform selected" workflow_dispatch refs/heads/feature/test '' '' 1
