// NOTE: Scroll region Range semantics clarification (placeholder):
// C# Range is start..end with end exclusive. Terminal scroll region sequences typically
// use inclusive coordinates. When implementing real scroll region logic, convert Range
// to inclusive indices by treating (start, endExclusive - 1). This file exists as a
// TODO marker and will be replaced with concrete implementation details later.
