# BUILD-README-VALIDATION-001

Status: planned

## Overview
Add CI validation step ensuring auto-generated package README files are current with XML documentation extraction pipeline.

## Goals
- Prevent stale API representation in distributed packages
- Fail builds when README divergence detected

## Scope
- CI workflow step (GitHub Actions) invoking tooling with `--check`
- Tool exit codes integrated into pipeline

## Implementation Notes
- Extend existing docs generation tool or create a new `dotnet tool` command `readme --check`
- Provide ignore list (e.g., main CycoTui root README)
- Normalize whitespace before comparison

## See Also
- SPEC-DOCS-001 (documentation generation)
- IMPLEMENTATION-DECISIONS-001 (#27 README CI Behavior)

## Acceptance Criteria
- CI fails when a README is out-of-date
- Developer can run local command to regenerate
- Documentation updates pass after regeneration

## Testing Approach
- Deliberately modify one README; confirm failing job
- Regenerate and confirm success

## Related Components
- Documentation generation utility
- GitHub Actions workflow
