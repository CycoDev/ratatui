# SETUP-FEATURE-FLAGS-001

Status: planned

## Overview
Introduce unified feature flag infrastructure combining compile-time symbols (e.g., `FEATURE_CALENDAR`) with a runtime registry for dynamic behavior toggles (e.g., enabling experimental rendering optimizations).

## Goals
- Consistent naming and discovery of build-time feature symbols
- Runtime `FeatureFlags` service (immutable snapshot) accessible across subsystems
- Documentation of currently recognized feature flags

## Scope
In scope: compile-time symbol conventions, runtime flag container, helper methods.
Out of scope: persistent configuration storage or CLI toggling.

## Implementation Notes
- Provide static `FeatureFlags.Current` initialized from (a) environment variables (optional), (b) explicit builder
- Use partial classes or `#if FEATURE_X` to exclude code blocks physically
- Provide `FeatureFlags.IsEnabled(string name)` fast lookup (Dictionary or hash set)
- Generate a developer doc section listing active flags at build time (optional future)

## See Also
- IMPLEMENTATION-DECISIONS-001 (#18 Feature Flag Mechanism, #30 Calendar flag)

## Acceptance Criteria
- Defining `FEATURE_CALENDAR` includes Calendar widget code; omitting excludes it from compilation
- FeatureFlags accessible in runtime code for non-compilation toggles
- Unit test verifying excluded code not present when symbol absent (reflection assumption)

## Testing Approach
- Build two configurations: with and without `FEATURE_CALENDAR`; assert type existence
- Runtime test: construct `FeatureFlags` with custom set and query

## Related Components
- Build scripts / project files (.csproj conditions)
- Widgets assembly (Calendar)

## Performance Considerations
- Lookup O(1); minimal static initialization overhead
