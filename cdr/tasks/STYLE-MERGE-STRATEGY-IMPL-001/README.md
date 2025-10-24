# STYLE-MERGE-STRATEGY-IMPL-001

Status: planned

## Overview
Implement initial MergeStrategy behavior for borders/symbol placement: Replace and Preserve. Provide scaffolding for future Combine (box drawing union).

## Goals
- Define `MergeStrategy` enum
- Integrate into Block / border rendering code

## Scope
- Replace: overwrite existing non-space cell symbol and style
- Preserve: only write when target cell is space or skip flag set
- Logging at debug for unexpected conflicts (optional)

## Implementation Notes
- Provide utility `SymbolMerger.Merge(Cell existing, Cell incoming, MergeStrategy strategy)`
- Combine left unimplemented with NotSupportedException placeholder

## See Also
- IMPLEMENTATION-DECISIONS-001 (#12)
- SPEC-WIDGET-003 (Block & border discussions)

## Acceptance Criteria
- Unit tests for Replace and Preserve behaviors
- Block widget uses configured strategy (default Replace)

## Testing Approach
- Construct buffer with pre-filled border cells; apply new border under both strategies; assert results
