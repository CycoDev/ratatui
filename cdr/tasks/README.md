# Task Documents

This folder contains implementation-focused guidance for specific development work items for CycoTui.

## Purpose

Task documents provide practical guidance for implementing specific components or functionality. They bridge the gap between what should be built (features) and how it should be built (specifications).

## Contents

Each task has its own folder containing:
- `README.md` - Main task document with implementation guidance
- Optional supporting files specific to the task

Task documents typically cover:
- Implementation approach and guidance
- Key challenges and considerations
- Technical decisions and alternatives
- References to specifications and related work
- Verification and acceptance criteria
- Testing approach when relevant

## Document Philosophy

Task documents are designed with a human-first approach while remaining useful for AI agents. The template provides structure and guidance but emphasizes flexibility and practical value over strict compliance.

### Core Principles
- **Practical over prescriptive**: Include sections that genuinely help complete the task
- **Flexible structure**: Adapt the template to what the task actually needs
- **Value-driven content**: Focus on information that helps implementers succeed
- **Human judgment**: Use experience to determine appropriate detail level

## Document Structure

### Required Sections
All task documents should include:
- **Overview**: What will be implemented and why
- **See Also**: Key specifications and related documents to reference
- **Acceptance Criteria**: Clear, measurable completion criteria

### Use When Relevant Sections
Include these sections when they add genuine value:
- **Implementation Notes**: Detailed guidance on how to implement
- **Key Challenges**: Significant obstacles and how to address them
- **Implementation Approach**: Overall strategy and methodology
- **Testing Approach**: How to verify the implementation works
- **Related Components**: Files and components that will be affected

### Optional Sections
Add these when they provide specific value for the task:
- **Integration Points**: How this connects with other system parts
- **Performance Considerations**: When performance is a concern
- **Platform-Specific Details**: When implementation varies by platform
- **Security Considerations**: When security aspects are relevant
- **Migration Notes**: When updating existing functionality

## Writing Guidelines

### Detail Level
Adjust detail based on:
- Task complexity and risk
- Team experience and context
- Time constraints and priorities
- Dependencies and integration points

### Section Inclusion
Include sections when they:
- Help someone unfamiliar complete the task
- Address non-obvious challenges or approaches
- Connect to important specifications or dependencies
- Provide verification or testing guidance

Skip sections that:
- Repeat information available elsewhere
- State obvious implementation steps
- Add bureaucratic overhead without value
- Duplicate specification content unnecessarily

## Task Categories

Tasks use prefixes to indicate their general type:
- **SETUP-**: Project infrastructure and tooling
- **CORE-**: Foundational components and architecture
- **WIDGET-**: Widget implementation and rendering
- **BACKEND-**: Terminal backend implementation
- **INPUT-**: Input handling and events
- **STYLE-**: Styling and visual enhancements
- **LAYOUT-**: Layout engine and constraints
- **BUFFER-**: Buffer and rendering model
- **TEST-**: Testing infrastructure and test cases
- **DOC-**: Documentation and examples
- **PERF-**: Performance optimizations

Different categories may naturally need different levels of detail and different approaches to implementation guidance.

## Relationship to Other Documents

Task documents:
- Implement features described in feature documents
- Follow technical guidance from specification documents
- Align with timeline in roadmap documents
- Reference other tasks they depend on
- Connect to architectural decisions and patterns

## Folder Structure

Task folders follow this ID pattern:
```
[CATEGORY]-[FEATURE]-[NUMBER]/
```

For example:
- `SETUP-PROJ-STRUCTURE-001/` - Set up project structure and organization
- `CORE-BACKEND-INTERFACE-001/` - Implement backend interface
- `WIDGET-PARAGRAPH-001/` - Implement paragraph widget

Each task folder contains at minimum a README.md file that documents the task.

## Maintenance

Task documents should evolve with the work:
- Update status as implementation progresses
- Add discoveries and decisions made during implementation
- Reference new specifications or dependencies that emerge
- Document any deviations from the original plan